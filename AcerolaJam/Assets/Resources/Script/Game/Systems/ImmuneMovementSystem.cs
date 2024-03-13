using NUnit.Framework;
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
using Unity.VisualScripting;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[UpdateAfter(typeof(ColonyPhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ImmuneMovementSystem : SystemBase
{
    public static ImmuneMovementSystem handle;
    EntityQuery query;

    ComponentTypeHandle<LocalTransform> read_only_transform_handle;
    ComponentTypeHandle<PlateletComponent> read_only_platelet_handle;
    ComponentTypeHandle<CellComponent> read_write_cell_handle;
    ComponentTypeHandle<PhysicsVelocity> read_write_velocity_handle;

    protected override void OnCreate()
    {
        handle = this;
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, ImmuneComponent>().WithAllRW<PhysicsVelocity>().Build(this);

        read_only_transform_handle = GetComponentTypeHandle<LocalTransform>(true);
        read_only_platelet_handle = GetComponentTypeHandle<PlateletComponent>(true);
        read_write_cell_handle = GetComponentTypeHandle<CellComponent>(false);
        read_write_velocity_handle = GetComponentTypeHandle<PhysicsVelocity>(false);
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();
        NativeList<ImmuneComponent> unique_colonies;
        EntityManager.GetAllUniqueSharedComponents(out unique_colonies, Allocator.Temp);
        int entity_count;

        var colonies = ColonySystem.handle.GetColonies();
        var colony_pos_array = new float2[colonies.Count];
        int index = 0;
        foreach (var v in colonies)
            colony_pos_array[index++] = v.center;

        NativeArray<float2> colony_positions = new NativeArray<float2>(colony_pos_array, Allocator.TempJob);

        foreach (var component in unique_colonies)
        {
            if (component.behaviour_type == 0)
                continue;
            query.ResetFilter();
            query.SetSharedComponentFilter(component);

            if ((entity_count = query.CalculateEntityCount()) > 0)
            {
                int behaviour_type = component.behaviour_type;

                read_only_transform_handle.Update(this);
                read_write_cell_handle.Update(this);
                read_write_velocity_handle.Update(this);

                switch(behaviour_type)
                {
                    case 1:
                        new ApplyForces_WhiteCell {
                            colony_positions = colony_positions,
                            attack_range_squared = 40 * 40,
                            bunching_distance = 25,
                            bunching_strength = 35,
                            movement_speed = GameMap.Instance?.delay <= 0 ? 75 : 0,
                            ReadOnlyTransformHandle = read_only_transform_handle,
                            ReadWriteCellHandle = read_write_cell_handle, 
                            ReadWriteVelocityHandle = read_write_velocity_handle}.Run(query);
                        break;
                    case 2:
                        new ApplyForces_Adipose
                        {
                            min_distance = 8,
                            max_distance = 45,
                            bunching_strength = 74,
                            ReadOnlyTransformHandle = read_only_transform_handle,
                            ReadWriteVelocityHandle = read_write_velocity_handle
                        }.Run(query);
                        break;
                    case 3:
                        read_only_platelet_handle.Update(this);
                        new ApplyForces_Platelet
                        {
                            bunching_distance = 40,
                            bunching_strength = 110,
                            attraction_strength = 60,
                            ReadOnlyTransformHandle = read_only_transform_handle,
                            ReadOnlyPlateletHandle = read_only_platelet_handle,
                            ReadWriteVelocityHandle = read_write_velocity_handle
                        }.Run(query);
                        break;
                }
            }
        }
        colony_positions.Dispose();
        unique_colonies.Dispose();
    }

    [BurstCompile]
    public partial struct ApplyForces_WhiteCell : IJobChunk
    {
        [ReadOnly]
        public NativeArray<float2> colony_positions;

        [ReadOnly]
        public float attack_range_squared;
        [ReadOnly]
        public float movement_speed;
        [ReadOnly]
        public float bunching_distance;
        [ReadOnly]
        public float bunching_strength;

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
                float2 pos = new float2(translations[i].Position.x, translations[i].Position.y);
                int closest_index = 0;
                float closest_dist = math.distancesq(colony_positions[0], pos);

                for (int colony = 1; colony < colony_positions.Length; colony++)
                {
                    float test_dist = math.distancesq(colony_positions[colony], pos);
                    if (test_dist < closest_dist)
                    {
                        closest_index = colony;
                        closest_dist = test_dist;
                    }
                }
                float3 cluster_force = float3.zero;
                var inner_enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                while (inner_enumerator.NextEntityIndex(out var j))
                {
                    if (i == j)
                        continue;
                    float3 dist = (translations[j].Position - translations[i].Position);
                    float mag = math.length(dist);
                    if (mag < bunching_distance && mag > 0)
                    {
                        float repulsion = 1 - (mag / bunching_distance);
                        cluster_force += math.normalizesafe(dist, float3.zero) * -1 * repulsion * bunching_strength;
                    } 
                    else if(mag > bunching_distance && mag < bunching_distance * 2)
                    {
                        float repulsion = (mag / bunching_distance) - 1;
                        cluster_force -= math.normalizesafe(dist, float3.zero) * repulsion * bunching_strength;
                    }
                }

                var vel = velocities[i];
                float2 movement_force = float2.zero;
                if (closest_dist < attack_range_squared || true)
                {
                    movement_force = math.normalizesafe(colony_positions[closest_index] - pos, float2.zero) * movement_speed;
                }
                else
                {
                    movement_force = math.normalizesafe(new float2(vel.Linear.x, vel.Linear.y), float2.zero) * (.25f * movement_speed);
                }
                vel.Linear = cluster_force + new float3((movement_force), 0.0f);
                vel.Linear.z = -translations[i].Position.z;
                vel.Angular = float3.zero;
                velocities[i] = vel;
            }
        }
    }
   
    [BurstCompile]
    public partial struct ApplyForces_Adipose : IJobChunk
    {
        [ReadOnly]
        public float min_distance;
        [ReadOnly]
        public float max_distance;
        [ReadOnly]
        public float bunching_strength;

        [ReadOnly]
        public ComponentTypeHandle<LocalTransform> ReadOnlyTransformHandle;
        public ComponentTypeHandle<PhysicsVelocity> ReadWriteVelocityHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalTransform> translations = chunk.GetNativeArray(ref ReadOnlyTransformHandle);
            NativeArray<PhysicsVelocity> velocities = chunk.GetNativeArray(ref ReadWriteVelocityHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while (enumerator.NextEntityIndex(out var i))
            {
                float2 pos = new float2(translations[i].Position.x, translations[i].Position.y);

                float3 cluster_force = float3.zero;
                var inner_enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                while (inner_enumerator.NextEntityIndex(out var j))
                {
                    if (i == j)
                        continue;
                    float3 dist = (translations[j].Position - translations[i].Position);
                    float mag = math.length(dist);
                    if (mag < min_distance && mag > 0)
                    {
                        float repulsion = 1 - (mag / min_distance);
                        cluster_force += math.normalizesafe(dist, float3.zero) * -1 * repulsion * bunching_strength;
                    }
                    else if (mag > max_distance && mag < max_distance * 1.2f)
                    {
                        float repulsion = (mag / max_distance) - 1;
                        cluster_force -= math.normalizesafe(dist, float3.zero) * repulsion * bunching_strength *.5f;
                    }
                }

                var vel = velocities[i];
                vel.Linear = cluster_force;
                vel.Linear.z = -translations[i].Position.z;
                vel.Angular = float3.zero;
                velocities[i] = vel;
            }
        }
    }

    [BurstCompile]
    public partial struct ApplyForces_Platelet : IJobChunk
    {
        [ReadOnly]
        public float bunching_distance;
        [ReadOnly]
        public float bunching_strength;
        [ReadOnly]
        public float attraction_strength;

        [ReadOnly]
        public ComponentTypeHandle<LocalTransform> ReadOnlyTransformHandle;
        [ReadOnly]
        public ComponentTypeHandle<PlateletComponent> ReadOnlyPlateletHandle;
        public ComponentTypeHandle<PhysicsVelocity> ReadWriteVelocityHandle;

        float2 ClosestPointOnLine(float2 lineStart, float2 lineEnd, float2 point)
        {
            float2 lineDirection = math.normalizesafe(lineEnd - lineStart, float2.zero);
            float dotProduct = math.dot(point - lineStart, lineDirection);
            return lineStart + lineDirection * dotProduct;
        }

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalTransform> translations = chunk.GetNativeArray(ref ReadOnlyTransformHandle);
            NativeArray<PlateletComponent> platelets = chunk.GetNativeArray(ref ReadOnlyPlateletHandle);
            NativeArray<PhysicsVelocity> velocities = chunk.GetNativeArray(ref ReadWriteVelocityHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while (enumerator.NextEntityIndex(out var i))
            {
                float3 cluster_force = float3.zero;
                var inner_enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                while (inner_enumerator.NextEntityIndex(out var j))
                {
                    if (i == j)
                        continue;
                    float3 dist = (translations[j].Position - translations[i].Position);
                    float mag = math.length(dist);
                    if(mag < bunching_distance && mag > 0)
                    {
                        float repulsion = 1 - (mag / bunching_distance);
                        cluster_force += math.normalizesafe(dist, float3.zero) * -1 * repulsion * bunching_strength;
                    }
                }

                float2 pos = new float2(translations[i].Position.x, translations[i].Position.y);
                float2 closestPoint = ClosestPointOnLine(platelets[i].hold_a, platelets[i].hold_b, pos);
                float2 directionToLine = math.normalizesafe(closestPoint - pos, float2.zero);

                if(math.length(directionToLine) > bunching_distance * 2)
                {
                    directionToLine *= 2;
                }

                var vel = velocities[i];

                vel.Linear = cluster_force + new float3((directionToLine * attraction_strength), 0.0f);
                vel.Linear.z = -translations[i].Position.z;
                vel.Angular = float3.zero;
                velocities[i] = vel;
            }
        }
        /*
            float3 directionToOther = positions[j] - currentPosition;
            float distance = math.length(directionToOther);

            // Check if within repulsion radius (idealDistance)
            if (distance < idealDistance && distance > 0)
            {
                // Calculate repulsion force based on how close the objects are
                float repulsionFactor = 1 - (distance / idealDistance);
                repulsionForce += math.normalize(directionToOther) * -1 * repulsionFactor * repulsionStrength;
            }


                void ApplySeparation()
                {
                    Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, repulsionRadius);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.gameObject != gameObject) // Avoid self
                        {
                            Vector2 directionAway = ((Vector2)transform.position - (Vector2)neighbor.transform.position).normalized;
                            rb.AddForce(directionAway * repulsionStrength);
                        }
                    }
                }
         * */
    }
}