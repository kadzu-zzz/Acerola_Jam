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


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class FoodHazardScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Dependency.Complete();
        EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
        List<int> destroyed_objs =new List<int>();

        if (GameMap.Instance == null || !GameMap.Instance.check)
            return;

        foreach (var (transform, haz, entity) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<HazardComponent>>().WithEntityAccess())
        {
            if (haz.ValueRO.max_food > 0)
            {
                if (haz.ValueRO.food > 0)
                {
                    if(GameLevel.game_obj_links.ContainsKey(haz.ValueRO.gameobj_id))
                        GameLevel.game_obj_links[haz.ValueRO.gameobj_id].transform.localScale = Vector3.one * transform.ValueRO.Value.Scale();
                }
                else
                {
                    destroyed_objs.Add(haz.ValueRO.gameobj_id);
                    GameObject.Destroy(GameLevel.game_obj_links[haz.ValueRO.gameobj_id]);
                    GameLevel.game_obj_links.Remove(haz.ValueRO.gameobj_id);
                    buffer.DestroyEntity(entity);
                }
            }
        }
        GameMap.Instance.ObjectsDestroyed(destroyed_objs);
        buffer.Playback(EntityManager);
        buffer.Dispose();
    }

}