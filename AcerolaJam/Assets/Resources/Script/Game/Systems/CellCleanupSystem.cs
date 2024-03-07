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

        new BufferDeadCells { buffer = buffer }.Run();

        buffer.Playback(EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    public partial struct BufferDeadCells : IJobEntity
    {
        public EntityCommandBuffer buffer;

        public void Execute(Entity e, [EntityIndexInQuery] int entityInQueryIndex, RefRO<CellComponent> cell)
        {
            if (cell.ValueRO.health <= 0.0f)
            {
                buffer.DestroyEntity(e);
            }
        }
    }
}
