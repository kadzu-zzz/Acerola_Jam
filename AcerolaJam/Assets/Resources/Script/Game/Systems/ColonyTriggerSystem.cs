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

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ColonyTriggerSystem : ISystem
{
    ComponentLookup<CellComponent> read_write_cell_lookup;
    ComponentLookup<HazardComponent> read_write_hazard_lookup;

    public void OnCreate(ref SystemState state) 
    {
        read_write_cell_lookup = SystemAPI.GetComponentLookup<CellComponent>(false);
        read_write_hazard_lookup = SystemAPI.GetComponentLookup<HazardComponent>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        read_write_cell_lookup.Update(ref state);
        read_write_hazard_lookup.Update(ref state);
        state.Dependency = new CellTriggerEvents
        {
            cell_lookup = read_write_cell_lookup,
            hazard_lookup = read_write_hazard_lookup
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    public partial struct CellTriggerEvents : ITriggerEventsJob
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<CellComponent> cell_lookup;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<HazardComponent> hazard_lookup;

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
                    cell.ValueRW.impulse += comp.ValueRO.impulse;
                    if (cell.ValueRO.consume < 1.0f)
                    {
                        cell.ValueRW.consume += math.min(cell.ValueRO.power, comp.ValueRO.food);
                        comp.ValueRW.food = math.max(0.0f, comp.ValueRO.food - cell.ValueRO.power);
                    }
                }
            } 
            else if(b_cell)
            {
                if (hazard_lookup.HasComponent(collisionEvent.EntityA))
                {
                    RefRW<HazardComponent> comp = hazard_lookup.GetRefRW(collisionEvent.EntityB);
                    RefRW<CellComponent> cell = cell_lookup.GetRefRW(collisionEvent.EntityA);
                    cell.ValueRW.uv += comp.ValueRO.uv;
                    cell.ValueRW.fire += comp.ValueRO.fire;
                    cell.ValueRW.impulse += comp.ValueRO.impulse;
                    if (cell.ValueRO.consume < 1.0f)
                    {
                        cell.ValueRW.consume += math.min(cell.ValueRO.power, comp.ValueRO.food);
                        comp.ValueRW.food = math.max(0.0f, comp.ValueRO.food - cell.ValueRO.power);
                    }
                }
            }
        }
    }
}
