using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;


[RequireMatchingQueriesForUpdate, UpdateAfter(typeof(ColonySystem)), UpdateBefore(typeof(ColonyPhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class CellCleanupSystem : SystemBase
{
    public static CellCleanupSystem handle;
    EntityQuery query;


    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<CellComponent>().Build(this);
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();        
        EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.TempJob);
        NativeArray<int> count = new NativeArray<int>(ColonySystem.handle.ColonyCount(), Allocator.TempJob);

        for (int i = 0; i < count.Length; i++)
        {
            count[i] = 0;
        }

        new BufferDeadCells { buffer = buffer, count = count}.Run();

        for(int i = 0; i < count.Length; i++)
        {
            if (count[i] > 0)
                GameLevel.GenerateCells(i + 1, count[i]);
        }

        count.Dispose();
        buffer.Playback(EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    public partial struct BufferDeadCells : IJobEntity
    {
        public EntityCommandBuffer buffer;
        public NativeArray<int> count;

        public void Execute(Entity e, [EntityIndexInQuery] int entityInQueryIndex, RefRW<CellComponent> cell)
        {
            if (cell.ValueRO.health <= 0.0f)
            {
                buffer.DestroyEntity(e);
            } else if(cell.ValueRO.health >= 1.0f && cell.ValueRO.consume >= 1.0f)
            {
                cell.ValueRW.consume -= 1.0f;
                count[cell.ValueRO.belongs_to - 1]++;
            } 
        }
    }
}
