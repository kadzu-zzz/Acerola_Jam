using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;


[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ColonySystem : SystemBase
{
    public static ColonySystem handle;
    EntityQuery query;

    ComponentTypeHandle<LocalTransform> read_only_transform_handle;

    Dictionary<int, CoreData> cores;
    int core_id;

    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, CellComponent, CoreComponent>().Build(this);
        core_id = 0;
        cores = new();

        read_only_transform_handle = GetComponentTypeHandle<LocalTransform>(true);
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();
        NativeList<CoreComponent> unique_colonies;
        EntityManager.GetAllUniqueSharedComponents(out unique_colonies, Allocator.Temp);
        int entity_count;

        NativeArray<float2> accum = new NativeArray<float2>(1, Allocator.TempJob);
        foreach (var component in unique_colonies)
        {
            query.ResetFilter();
            query.SetSharedComponentFilter(component);

            if ((entity_count = query.CalculateEntityCount()) > 0)
            {
                CoreData data = GetCore(component.id);

                accum[0] = float2.zero;
                read_only_transform_handle.Update(this);
                var cell = new UpdateCells { aggregate_position = accum, ReadOnlyTransformHandle = read_only_transform_handle };
                cell.Run(query);

                data.center = accum[0] / entity_count;
                data.cells = entity_count;

                UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 dir = ray.direction.normalized;
                float t = (20.0f - ray.origin.z) / dir.z;
                Vector3 point = ray.origin + (dir * t);
                data.target = new float2(point.x, point.y);

                UpdateCore(component.id, data);
            }
        }
        accum.Dispose();
        unique_colonies.Dispose();
    }

    public int NewCore(CoreData core)
    {
        cores.Add(++core_id, core);
        return core_id;
    }

    public CoreData GetCore(int id)
    {
        return cores[id];
    }

    public void UpdateCore(int id, CoreData core)
    {
        cores[id] = core;
    }

    [BurstCompile]
    public partial struct UpdateCells : IJobChunk
    {
        public NativeArray<float2> aggregate_position;

        [ReadOnly]
        public ComponentTypeHandle<LocalTransform> ReadOnlyTransformHandle;    

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalTransform> translations = chunk.GetNativeArray(ref ReadOnlyTransformHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while (enumerator.NextEntityIndex(out var i))
            {
                aggregate_position[0] = new float2(aggregate_position[0].x + translations[i].Position.x, aggregate_position[0].y + translations[i].Position.y);
            }
        }
    }
}