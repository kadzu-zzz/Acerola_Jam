using System.Collections;
using System.Collections.Generic;

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
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ColonyTriggerSystem : ISystem
{
    EntityQuery query;
    ComponentLookup<CellComponent> read_write_cell_lookup;
    ComponentLookup<HazardComponent> read_write_hazard_lookup;
    ComponentLookup<LocalTransform> read_write_transform;

    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform>().WithAny<HazardComponent, CellComponent>().Build(state.EntityManager);

        state.RequireForUpdate<CoreComponent>();

        read_write_cell_lookup = SystemAPI.GetComponentLookup<CellComponent>(false);
        read_write_hazard_lookup = SystemAPI.GetComponentLookup<HazardComponent>(false);
        read_write_transform = SystemAPI.GetComponentLookup<LocalTransform>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        read_write_cell_lookup.Update(ref state);
        read_write_hazard_lookup.Update(ref state);
        read_write_transform.Update(ref state);
        state.Dependency = new CellTriggerEvents
        {
            cell_lookup = read_write_cell_lookup,
            hazard_lookup = read_write_hazard_lookup,
            transform_lookup = read_write_transform
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        state.CompleteDependency();
    }

    [BurstCompile]
    public partial struct CellTriggerEvents : ITriggerEventsJob
    {
        public ComponentLookup<CellComponent> cell_lookup;
        public ComponentLookup<HazardComponent> hazard_lookup;
        public ComponentLookup<LocalTransform> transform_lookup;

        public void Execute(TriggerEvent collisionEvent)
        {
            bool a_cell = cell_lookup.HasComponent(collisionEvent.EntityA);
            bool b_cell = cell_lookup.HasComponent(collisionEvent.EntityB);

            if(a_cell)
            {
                if(hazard_lookup.HasComponent(collisionEvent.EntityB))
                {
                    RefRW<HazardComponent> comp = hazard_lookup.GetRefRW(collisionEvent.EntityB);
                    RefRW<CellComponent> cell = cell_lookup.GetRefRW(collisionEvent.EntityA);
                    cell.ValueRW.uv += comp.ValueRO.uv;
                    cell.ValueRW.fire += comp.ValueRO.fire;
                    cell.ValueRW.death += comp.ValueRO.death;
                    cell.ValueRW.impulse += comp.ValueRO.impulse;
                    if (cell.ValueRO.consume < 1.0f)
                    {
                        float f = comp.ValueRO.food;
                        cell.ValueRW.consume += math.min(cell.ValueRO.power, comp.ValueRO.food);
                        comp.ValueRW.food = math.max(0.0f, comp.ValueRO.food - cell.ValueRO.power);
                        if(f != comp.ValueRO.food)
                        {
                            var transform = transform_lookup.GetRefRW(collisionEvent.EntityB);
                            transform.ValueRW.Scale = 0.5f + ((comp.ValueRO.food / comp.ValueRO.max_food) * 0.5f);
                        }
                    }
                }
            } 
            else if(b_cell)
            {
                if (hazard_lookup.HasComponent(collisionEvent.EntityA))
                {
                    RefRW<HazardComponent> comp = hazard_lookup.GetRefRW(collisionEvent.EntityA);
                    RefRW<CellComponent> cell = cell_lookup.GetRefRW(collisionEvent.EntityB);
                    cell.ValueRW.uv += comp.ValueRO.uv;
                    cell.ValueRW.fire += comp.ValueRO.fire;
                    cell.ValueRW.death += comp.ValueRO.death;
                    cell.ValueRW.impulse += comp.ValueRO.impulse;
                    if (cell.ValueRO.consume < 1.0f)
                    {
                        float f = comp.ValueRO.food;
                        cell.ValueRW.consume += math.min(cell.ValueRO.power, comp.ValueRO.food);
                        comp.ValueRW.food = math.max(0.0f, comp.ValueRO.food - cell.ValueRO.power);
                        if (f != comp.ValueRO.food)
                        {
                            var transform = transform_lookup.GetRefRW(collisionEvent.EntityA);
                            transform.ValueRW.Scale = 0.5f + ((comp.ValueRO.food / comp.ValueRO.max_food) * 0.5f);
                        }
                    }
                }
            }
        }
    }
}
