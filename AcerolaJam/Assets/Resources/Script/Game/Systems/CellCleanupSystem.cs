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
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;


[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(ColonySystem))]
public partial class CellCleanupSystem : SystemBase
{
    public static CellCleanupSystem handle;
    EntityQuery query;

    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<CellComponent>().Build(this);

        RequireForUpdate<CellComponent>();
    }

    protected override void OnUpdate()
    {
        if (ColonySystem.handle.ColonyCount() <= 0)
            return;    

        EntityCommandBuffer destroy_buffer = new EntityCommandBuffer(Allocator.TempJob);
        EntityCommandBuffer spawn_buffer = new EntityCommandBuffer(Allocator.TempJob);
        NativeArray<int> count = new NativeArray<int>(ColonySystem.handle.ColonyCount(), Allocator.TempJob);

        for (int i = 0; i < count.Length; i++)
        {
            count[i] = 0;
        }

        Dependency = new BufferDeadCells { buffer = destroy_buffer.AsParallelWriter(), count = count}.Schedule(Dependency);
        Dependency.Complete();

        for(int i = 0; i < count.Length; i++)
        {
            if (count[i] > 0)
                GameLevel.GenerateCells(spawn_buffer, i + 1, count[i]);
        }

        count.Dispose();
        destroy_buffer.Playback(EntityManager);
        destroy_buffer.Dispose();
        spawn_buffer.Playback(EntityManager);
        spawn_buffer.Dispose();
    }

    [BurstCompile]
    public partial struct BufferDeadCells : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter buffer;
        public NativeArray<int> count;

        public void Execute(Entity e, [EntityIndexInQuery] int entityInQueryIndex, RefRW<CellComponent> cell)
        {
            if (cell.ValueRO.health <= 0.0f)
            {
                buffer.DestroyEntity(entityInQueryIndex, e);
            } 
            else if(cell.ValueRO.health >= 1.0f && cell.ValueRO.consume >= 1.0f)
            {
                cell.ValueRW.consume -= 1.0f;
                count[cell.ValueRO.belongs_to - 1]++;
            } 
        }
    }
}
