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

[UpdateInGroup(typeof(PhysicsSimulationGroup))]
[UpdateAfter(typeof(PhysicsCreateBodyPairsGroup))]
[UpdateBefore(typeof(PhysicsCreateContactsGroup))]
[RequireMatchingQueriesForUpdate]
public partial struct ColonyDevourSystem : ISystem
{
    ComponentLookup<CellComponent> read_write_cell_lookup;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CellComponent>();
        state.RequireForUpdate<SimulationSingleton>();
        read_write_cell_lookup = SystemAPI.GetComponentLookup<CellComponent>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        if (simulation.Type == SimulationType.NoPhysics)
        {
            return;
        }

        read_write_cell_lookup.Update(ref state);
        var physicsWorld = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;

        state.Dependency = new CellCollisionEvents
        {
            cell_lookup = read_write_cell_lookup,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), ref physicsWorld, state.Dependency);
        state.CompleteDependency();
    }

    [BurstCompile]
    public partial struct CellCollisionEvents : IBodyPairsJob
    {
        public ComponentLookup<CellComponent> cell_lookup;

        public void Execute(ref ModifiableBodyPair pair)
        {
            if (pair.EntityA != null && pair.EntityB != null)
            {
                bool a_cell = cell_lookup.HasComponent(pair.EntityA);
                bool b_cell = cell_lookup.HasComponent(pair.EntityB);

                if (a_cell && b_cell)
                {
                    RefRW<CellComponent> cella = cell_lookup.GetRefRW(pair.EntityA);
                    RefRW<CellComponent> cellb = cell_lookup.GetRefRW(pair.EntityB);

                    if (cella.ValueRO.belongs_to != cellb.ValueRO.belongs_to)
                    {
                        cella.ValueRW.health -= cellb.ValueRO.power;
                        cellb.ValueRW.health -= cella.ValueRO.power;
                    }
                }
            }
        }
    }
}
