using System;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CellCleanupSystem))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial class ColonySystem : SystemBase
{
    public static ColonySystem handle;
    EntityQuery query;

    ComponentTypeHandle<LocalTransform> read_write_transform_handle;
    ComponentTypeHandle<CellComponent> read_write_cell_handle;
    public static bool player_follow_mouse;

    Dictionary<int, CoreData> cores;
    int core_id;

    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, CellComponent, CoreComponent>().Build(this);
        core_id = 0;
        cores = new();

        read_write_transform_handle = GetComponentTypeHandle<LocalTransform>(false);
        read_write_cell_handle = GetComponentTypeHandle<CellComponent>(false);
        SceneManager.sceneUnloaded += ResetCoreId;
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneUnloaded -= ResetCoreId;
    }

    public void ResetCoreId(Scene s)
    {
        cores.Clear();
        core_id = 0;
    }

    protected override void OnUpdate()
    {
        NativeList<CoreComponent> unique_colonies;
        EntityManager.GetAllUniqueSharedComponents(out unique_colonies, Allocator.Temp);
        int entity_count;

        HashSet<int> hits = new();

        NativeReference<float2> accum = new NativeReference<float2>(float2.zero, Allocator.TempJob);
        foreach (var component in unique_colonies)
        {
            if (component.id == 0)
                continue;

            query.ResetFilter();
            query.SetSharedComponentFilter(component);

            if ((entity_count = query.CalculateEntityCount()) > 0)
            {
                hits.Add(component.id);
                CoreData data = GetCore(component.id);

                accum.Value = float2.zero;
                read_write_transform_handle.Update(this);
                read_write_cell_handle.Update(this);
                var cell = new UpdateCells { aggregate_position = accum, ReadWriteTransformHandle = read_write_transform_handle, ReadWriteCellHandle = read_write_cell_handle,
                can_burn = !data.fire_immunity, can_uv = !data.uv_immunity};
                cell.Run(query);

                float2 new_center = accum.Value / entity_count;
                if(math.distancesq(data.center, new_center) > 0.1f)
                {
                    data.center = math.lerp(data.center, new_center, 0.1f);
                } else
                {
                    data.center = new_center;
                }
                data.cells = entity_count;

                if (component.player_colony && player_follow_mouse)
                {
                    UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector3 dir = ray.direction.normalized;
                    float t = (20.0f - ray.origin.z) / dir.z;
                    Vector3 point = ray.origin + (dir * t);
                    data.target = new float2(point.x, point.y);
                }

                UpdateCore(component.id, data);
            }
        }

        List<int> remove = new();
        foreach(var v in cores.Keys.ToArray())
        {
            if(!hits.Contains(v))
            {
                remove.Add(v);

                cores[v].OnDestroy?.Invoke();
            }
        }
        foreach(var v in remove)
        {
            cores.Remove(v);
        };
        accum.Dispose();
        unique_colonies.Dispose();
    }

    public int NewCore(CoreData core)
    {
        core.id = core_id + 1;
        cores.Add(++core_id, core);
        return core_id;
    }

    public int ColonyCount()
    {
        return core_id;
    }

    public CoreData GetCore(int id)
    {
        return cores[id];
    }

    public bool HasCore(int id)
    {
        return cores.ContainsKey(id);
    }

    public List<CoreData> GetColonies()
    {
        return cores.Values.ToList();
    }
    public void UpdateCore(int id, CoreData core)
    {
        cores[id] = core;
    }

    [BurstCompile]
    public partial struct UpdateCells : IJobChunk
    {
        public NativeReference<float2> aggregate_position;

        public ComponentTypeHandle<LocalTransform> ReadWriteTransformHandle;
        public ComponentTypeHandle<CellComponent> ReadWriteCellHandle;

        public bool can_burn;
        public bool can_uv;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalTransform> translations = chunk.GetNativeArray(ref ReadWriteTransformHandle);
            NativeArray<CellComponent> cells = chunk.GetNativeArray(ref ReadWriteCellHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while (enumerator.NextEntityIndex(out var i))
            {
                aggregate_position.Value = new float2(aggregate_position.Value.x + translations[i].Position.x, aggregate_position.Value.y + translations[i].Position.y);

                var cell = cells[i];
                if(can_burn)
                    cell.health -= cell.fire;
                if(can_uv)
                    cell.health -= cell.uv / 1000.0f;
                cell.health -= cell.death;
                if (cell.health < cell.max_health)
                {
                    if(cell.consume > 0)
                    {
                        cell.health += math.min(cell.consume, 0.01f);
                        cell.consume -= math.min(cell.consume, 0.01f);
                    }
                }
                cell.health = math.max(0, cell.health);
                cell.was_burning = cell.fire > 0;
                cell.was_uv = cell.uv > 0;
                cell.fire = 0;
                cell.uv = 0;
                cell.death = 0;
                cells[i] = cell;

                var translation = translations[i];
                translation.Scale = 0.4f + (0.6f * (cells[i].health / cells[i].max_health));
                translations[i] = translation;
            }
        }
    }
}