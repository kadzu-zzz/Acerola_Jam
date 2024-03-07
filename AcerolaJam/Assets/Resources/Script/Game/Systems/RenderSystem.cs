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
using Unity.Transforms;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;


[RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class RenderSystem : SystemBase
{
    public static RenderSystem handle;
    EntityQuery query_static, query_animated;

    public Mesh mesh;
    public Material mat;
    Vector4[] inputDatas;
    Matrix4x4[] matrixes;
    MaterialPropertyBlock block;
    string offsetInstanceName = "_FrameData";
    int maxRenderCount = 1023;

    NativeArray<float4x4> render_trs = new(1024, Allocator.Persistent);
    NativeArray<float4> render_details = new(1024, Allocator.Persistent);

    int texture_counter;
    int anim_counter;

    BiDictionary<Texture2D, int> texture_map;
    BiDictionary<Texture2D, int> anim_map;
    Dictionary<int, NativeArray<float4>> anim_frames;

    protected override void OnCreate()
    {
        handle = this;
        query_static = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, RenderComponent>().Build(this);
        query_animated = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, TimeOffsetComponent, AnimatedRenderComponent>().Build(this);
        texture_map = new();
        anim_map = new();
        anim_frames = new();
        texture_counter = 1;
        anim_counter = 1;

        mesh = MeshMaker.Create(20, 20);
        mat = Resources.Load<Material>("Material/CellMaterial");
        inputDatas = new Vector4[maxRenderCount];
        matrixes = new Matrix4x4[maxRenderCount];
        block = new MaterialPropertyBlock();
        render_trs = new(1024, Allocator.Persistent);
        render_details = new(1024, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        render_trs.Dispose();
        render_details.Dispose();
        foreach (var value in anim_frames.Values)
            value.Dispose();
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();
        { 
            NativeList<RenderComponent> unique_renders;
            EntityManager.GetAllUniqueSharedComponents<RenderComponent>(out unique_renders, Allocator.Temp);
            int entity_count;

            foreach (var component in unique_renders)
            {
                query_static.ResetFilter();
                query_static.SetSharedComponentFilter(component);

                if ((entity_count = query_static.CalculateEntityCount()) > 0)
                {
                    mat.mainTexture = texture_map.GetReverse(component.texture_id);
                    if (entity_count > render_trs.Length)
                    {
                        render_trs.Dispose();
                        render_details.Dispose();
                        render_trs = new(entity_count, Allocator.Persistent);
                        render_details = new(entity_count, Allocator.Persistent);
                    }
                    int offset = 0;

                    new PrepareStaticIndiciesJob { render_trs = render_trs, offset = offset }.Run(query_static);
                    entity_count = entity_count - offset;

                    int size = maxRenderCount;
                    for (int i = 0; i < entity_count; i += maxRenderCount)
                    {
                        if (i + size > entity_count)
                            size = entity_count - i;

                        unsafe
                        {
                            NativeArray<Matrix4x4>.Copy(render_trs.Reinterpret<Matrix4x4>(), i, matrixes, 0, size);
                        }

                        Graphics.DrawMeshInstanced(mesh, 0, mat, matrixes, size, block);
                    }
                }
            }

            unique_renders.Dispose();
        }
        {
            NativeList<AnimatedRenderComponent> unique_renders;
            EntityManager.GetAllUniqueSharedComponents<AnimatedRenderComponent>(out unique_renders, Allocator.Temp);
            int entity_count;

            foreach (var component in unique_renders)
            {
                query_animated.ResetFilter();
                query_animated.SetSharedComponentFilter(component);

                if ((entity_count = query_animated.CalculateEntityCount()) > 0)
                {
                    mat.mainTexture = texture_map.GetReverse(component.animation_id);
                    if (entity_count > render_trs.Length)
                    {
                        render_trs.Dispose();
                        render_details.Dispose();
                        render_trs = new(entity_count, Allocator.Persistent);
                        render_details = new(entity_count, Allocator.Persistent);
                    }
                    int offset = 0;

                    new PrepareAnimatedIndiciesJob { render_trs = render_trs, render_details = render_details, offset = offset, animation_speed = 1.0f, current_time = UnityEngine.Time.time, frames = anim_frames[component.animation_id] }.Run(query_animated);
                    entity_count = entity_count - offset;

                    int size = maxRenderCount;
                    for (int i = 0; i < entity_count; i += maxRenderCount)
                    {
                        if (i + size > entity_count)
                            size = entity_count - i;

                        unsafe
                        {
                            NativeArray<Vector4>.Copy(render_details.Reinterpret<Vector4>(), i, inputDatas, 0, size);
                            NativeArray<Matrix4x4>.Copy(render_trs.Reinterpret<Matrix4x4>(), i, matrixes, 0, size);
                        }

                        block.SetVectorArray(offsetInstanceName, inputDatas);
                        Graphics.DrawMeshInstanced(mesh, 0, mat, matrixes, size, block);
                    }
                }
            }

            unique_renders.Dispose();
        }
    }

    [BurstCompile]
    public partial struct PrepareStaticIndiciesJob : IJobEntity
    {
        [WriteOnly]
        public NativeArray<float4x4> render_trs;

        public int offset;

        public void Execute([EntityIndexInQuery] int entityInQueryIndex, RefRO<LocalTransform> transform)
        {
            render_trs[entityInQueryIndex - offset] = float4x4.TRS(transform.ValueRO.Position, Quaternion.identity, transform.ValueRO.Scale);
        }
    }

    [BurstCompile]
    public partial struct PrepareAnimatedIndiciesJob : IJobEntity
    {
        [WriteOnly]
        public NativeArray<float4x4> render_trs;
        [WriteOnly]
        public NativeArray<float4> render_details;

        [ReadOnly]
        public NativeArray<float4> frames;

        public float animation_speed;
        public float current_time;
        public int offset;

        public void Execute([EntityIndexInQuery] int entityInQueryIndex, RefRO<LocalTransform> transform, RefRO<TimeOffsetComponent> time)
        {
            render_trs[entityInQueryIndex - offset] = float4x4.TRS((transform.ValueRO.Position * new float3(1, 1, 0)) + 
                (math.sin(new float3(0, 0, (current_time - time.ValueRO.time_offset))) ), 
                transform.ValueRO.Rotation, transform.ValueRO.Scale); 
            render_details[entityInQueryIndex - offset] = frames[(int)(((current_time - time.ValueRO.time_offset) % animation_speed) / (animation_speed / frames.Length))];
        }
    }
    public int GetTextureIndex(Texture2D tex)
    {
        if (texture_map.ContainsForward(tex))
            return texture_map.GetForward(tex);
        texture_map.Add(tex, texture_counter++);
        return texture_map.GetForward(tex);
    }
    public int GetAnimIndex(Texture2D tex)
    {
        if (anim_map.ContainsForward(tex))
            return anim_map.GetForward(tex);
        anim_map.Add(tex, anim_counter++);
        return anim_map.GetForward(tex);
    }

    public bool HasAnim(int it)
    {
        return anim_frames.ContainsKey(it);
    }
    public void SetAnimFrame(int id, NativeArray<float4> frames)
    {        
        anim_frames.Add(id, frames);
    }
}
