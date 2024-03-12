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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


[RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class RenderSystem : SystemBase
{
    public static RenderSystem handle;
    EntityQuery query_static, query_animated;

    public Mesh mesh;
    public Material mat;
    public Dictionary<int, Material> staticmat;
    Vector4[] inputDatas;
    Matrix4x4[] matrixes;
    MaterialPropertyBlock block;
    MaterialPropertyBlock staticblock;
    string offsetInstanceName = "_FrameData";
    int maxRenderCount = 1023;

    Material eye_material;
    Material core_material;
    Material core_cell_material;
    public static PolygonCollider2D eye_bounds;


    NativeArray<float4x4> render_trs;
    NativeArray<float4> render_details;

    int texture_counter;
    int anim_counter;

    BiDictionary<Texture2D, int> texture_map;
    BiDictionary<Texture2D, int> anim_map;
    Dictionary<int, NativeArray<float4>> anim_frames;

    protected override void OnCreate()
    {
        handle = this;

        if (render_trs.IsCreated)
            render_trs.Dispose();
        if (render_details.IsCreated)
            render_details.Dispose();
        if(anim_frames != null)
        {
            foreach (var value in anim_frames.Values)
                value.Dispose();
        }

        query_static = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, RenderComponent>().Build(this);
        query_animated = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, TimeOffsetComponent, AnimatedRenderComponent>().Build(this);
        texture_map = new();
        anim_map = new();
        anim_frames = new();
        texture_counter = 1;
        anim_counter = 1;

        render_trs = new(1024, Allocator.Persistent);
        render_details = new(1024, Allocator.Persistent);

        mesh = MeshMaker.Create(20, 20);
        mat = Resources.Load<Material>("Material/CellMaterial");
        staticmat = new ();
        inputDatas = new Vector4[maxRenderCount];
        matrixes = new Matrix4x4[maxRenderCount];
        block = new MaterialPropertyBlock();
        staticblock = new MaterialPropertyBlock();

        core_cell_material = new Material(mat);
        core_material = new Material(mat);
        core_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_overlay");
        eye_material = Resources.Load<Material>("Material/EyeMaterial");

        core_cell_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");

        core_cell_material.renderQueue++;
        core_material.renderQueue += 2;
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

                        Graphics.DrawMeshInstanced(mesh, 0, staticmat[component.texture_id], matrixes, size, staticblock);
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
                    var anim = ColonySystem.handle.GetCore(1);

                    int animation_offset = 0;
                    if (anim.uv_immunity)
                        animation_offset++;
                    if (anim.fire_immunity)
                        animation_offset += 2;
                    mat.mainTexture = texture_map.GetReverse(component.animation_id + animation_offset);
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

            if(ColonySystem.handle.HasCore(1))
            {
                CoreData c = ColonySystem.handle.GetCore(1);

                Vector3 center = new(c.center.x, c.center.y, 0);

                float time = ((float) SystemAPI.Time.ElapsedTime) - c.spawn_time;
                var frames = anim_frames[1];
                float animation_speed = 1.0f;

                inputDatas[0] = frames[(int)(((time) % animation_speed) / (animation_speed / frames.Length))];
                //array_render_trs[0] = Matrix4x4.TRS(center + (Vector3.forward * ((Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale);
                block.SetVectorArray("_FrameData", inputDatas);
                Graphics.DrawMesh(mesh, Matrix4x4.TRS((center * new float3(1, 1, 0)) +
                (math.sin(new float3(0, 0, (time)))) + new float3(0, 0, -2), Quaternion.identity, Vector3.one), core_cell_material, 0, Camera.main, 0, block);

                matrixes[0] = Matrix4x4.TRS((center * new float3(1, 1, 0)) +
                (math.sin(new float3(0, 0, (time)))) + new float3(0, 0, -4), Quaternion.identity, Vector3.one );
                inputDatas[0] = frames[(int)(((time) % animation_speed) / (animation_speed / frames.Length))];
                block.SetVectorArray("_FrameData", inputDatas);
                Graphics.DrawMeshInstanced(mesh, 0, core_material, matrixes, 1, block);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 dir = ray.direction.normalized;
                float t = -(ray.origin.z) / dir.z;
                Vector3 point = ray.origin + (dir * t);
                Vector2 eyeLoc = Vector3.MoveTowards(center, point, (point - center).magnitude / 100.0f).XY();
                eye_bounds.offset = center;
                if (!eye_bounds.OverlapPoint(eyeLoc))
                {
                    eyeLoc = eye_bounds.ClosestPoint(eyeLoc);
                } 
                var p = new RenderParams(eye_material);
                Graphics.RenderMesh(p, mesh, 0, Matrix4x4.TRS(new Vector3(eyeLoc.x, eyeLoc.y, -6F), Quaternion.identity, Vector3.one * 1.1f));
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
        Material m = new Material(mat);
        m.mainTexture = tex;
        staticmat.Add(texture_counter, m);
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
