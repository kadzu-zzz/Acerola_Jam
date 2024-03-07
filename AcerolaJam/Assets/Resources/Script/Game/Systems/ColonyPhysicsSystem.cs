using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[RequireMatchingQueriesForUpdate]
[UpdateAfter(typeof(ColonySystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ColonyPhysicsSystem : SystemBase
{
    public static ColonyPhysicsSystem handle;
    EntityQuery query;

    ComponentTypeHandle<LocalTransform> read_only_transform_handle;
    ComponentTypeHandle<CellComponent> read_write_cell_handle;
    ComponentTypeHandle<PhysicsVelocity> read_write_velocity_handle;
    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, CoreComponent>().WithAllRW<PhysicsVelocity>().Build(this);

        read_only_transform_handle = GetComponentTypeHandle<LocalTransform>(true);
        read_write_cell_handle = GetComponentTypeHandle<CellComponent>(false);
        read_write_velocity_handle = GetComponentTypeHandle<PhysicsVelocity>(false);
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();
        NativeList<CoreComponent> unique_colonies;
        EntityManager.GetAllUniqueSharedComponents(out unique_colonies, Allocator.Temp);
        int entity_count;

        foreach (var component in unique_colonies)
        {
            if (component.id == 0)
                continue;
            query.ResetFilter();
            query.SetSharedComponentFilter(component);

            if ((entity_count = query.CalculateEntityCount()) > 0)
            {
                CoreData core = ColonySystem.handle.GetCore(component.id);
                read_only_transform_handle.Update(this);
                read_write_cell_handle.Update(this);
                read_write_velocity_handle.Update(this);

                new ApplyForces {
                    center = new float3(core.center.x, core.center.y, 0.0f),
                    target = new float3(core.target.x, core.target.y, 0.0f),
                    cohesion = core.cohesion,
                    movement = core.speed,
                    repulsion = core.repel,
                    repulsion_dist = core.repel_r,
                    time = SystemAPI.Time.DeltaTime,
                    ReadOnlyTransformHandle = read_only_transform_handle,
                    ReadWriteCellHandle = read_write_cell_handle, 
                    ReadWriteVelocityHandle = read_write_velocity_handle}.Run(query);
            }
        }
        unique_colonies.Dispose();
    }

    [BurstCompile]
    public partial struct ApplyForces : IJobChunk
    {
        [ReadOnly] public float3 center;
        [ReadOnly] public float3 target;

        [ReadOnly] public float cohesion;
        [ReadOnly] public float movement;
        [ReadOnly] public float repulsion;
        [ReadOnly] public float repulsion_dist;
        [ReadOnly] public float time;

        [ReadOnly] public bool uv_immune;

        [ReadOnly]
        public ComponentTypeHandle<LocalTransform> ReadOnlyTransformHandle;
        public ComponentTypeHandle<CellComponent> ReadWriteCellHandle;
        public ComponentTypeHandle<PhysicsVelocity> ReadWriteVelocityHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalTransform> translations = chunk.GetNativeArray(ref ReadOnlyTransformHandle);
            NativeArray<CellComponent> cells = chunk.GetNativeArray(ref ReadWriteCellHandle);
            NativeArray<PhysicsVelocity> velocities = chunk.GetNativeArray(ref ReadWriteVelocityHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while (enumerator.NextEntityIndex(out var i))
            {
                var inner = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                float3 attraction_force = math.normalizesafe(center - translations[i].Position, float3.zero) * cohesion;
                float3 movement_force = math.normalizesafe(target - translations[i].Position, float3.zero) * movement; ;
                float3 repulsion_force = float3.zero;
                int count = 0;
                while (inner.NextEntityIndex(out var j))
                {
                    if (i == j)
                        continue;
                    float3 dist = translations[i].Position - translations[j].Position;
                    float mag = math.length(dist);
                    if(mag < repulsion_dist)
                    {
                        if(mag != 0)
                        {
                            repulsion_force += (math.normalizesafe(dist, float3.zero) / mag) * translations[j].Scale;
                        }
                    }

                    count++;
                }
                //repulsion_force /= math.max(1, count);

                var vel = velocities[i];
                vel.Linear =  (( attraction_force + movement_force + (repulsion_force * (repulsion * translations[i].Scale))) * 1);
                if (!cells[i].impulse.Equals(float2.zero))
                {
                    vel.Linear += new float3(cells[i].impulse.x, cells[i].impulse.y, 0.0f);
                    var cell = cells[i];
                    cell.impulse = float2.zero;
                    cell.was_impulse = false;
                    cells[i] = cell;
                }
                else if (cells[i].was_impulse)
                {
                    var cell = cells[i];
                    cell.was_impulse = false;
                    cells[i] = cell;
                }
                if (!uv_immune)
                    vel.Linear *= (1.0f - cells[i].uv);
                vel.Angular = float3.zero;

                velocities[i] = vel;
            }
        }
    }
}