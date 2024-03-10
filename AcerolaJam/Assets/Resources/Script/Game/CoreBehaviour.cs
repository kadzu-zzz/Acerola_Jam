using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using Unity.Collections;
using Unity.Physics.Extensions;

public class CoreBehaviour : MonoBehaviour
{
    struct RenderData
    {
        public UnityEngine.Material target;
        public float scale;
        public float animation_speed;
    }

    public PolygonCollider2D eye_bounds;

    public float movement_strength = 10.0f;
    public float cohesion_strength = 15.0f;
    public float repel_distance = 10.0f;
    public float repel_strength = 10.0f;

    void Start()
    {
        for (int i = 0; i < 68; i++)
        {
           // GrowCell();
        }
       /* if (render_plane == null)
        {
            render_plane = MeshMaker.Create(20.0f, 20.0f);

            player_cell_material = Resources.Load<UnityEngine.Material>("Material/CellMaterial");
            player_core_material = new UnityEngine.Material(player_cell_material);
            enemy_triangle_material = new UnityEngine.Material(player_cell_material);
            enemy_square_material = new UnityEngine.Material(player_cell_material);
            enemy_core_material = new UnityEngine.Material(player_cell_material);
            core_eye_material = Resources.Load<UnityEngine.Material>("Material/EyeMaterial");
            player_cell_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            player_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_overlay");
            core_eye_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_eye");
            enemy_triangle_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_triangle");
            enemy_square_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            enemy_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_core");
        }*/
    }

   /* public void Render()
    {
        render_data.Clear();
        render_trs.Clear();

        foreach (var c in cells)
        {
            Vector3 scale = Vector3.one;
            render_trs.Add(Matrix4x4.TRS(new Vector3(c.position.x, c.position.y, 0) + (Vector3.forward * ((Time.time - c.spawn_time) % 1.25f)), Quaternion.identity, scale * render_cell.scale));
            render_data.Add(frames[(int)(((Time.time - c.spawn_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))]);
        }

        for (int c = 0; c < render_data.Count; c += MaxRenderCount)
        {
            int index = 0;
            int j;
            for (j = 0; j < Mathf.Min(c + MaxRenderCount, render_data.Count); j++)
            {
                array_render_data[index] = render_data[j];
                array_render_trs[index++] = render_trs[j];
            }
            block_cell.SetVectorArray("_FrameData", array_render_data);
            Graphics.DrawMeshInstanced(render_plane, 0, render_cell.target, array_render_trs, j, block_cell);
        }

        Vector3 center = new Vector3(this.center.x, this.center.y, -3.0f);
        array_render_data[0] = frames[(int)(((Time.time - spawn_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))];
        //array_render_trs[0] = Matrix4x4.TRS(center + (Vector3.forward * ((Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale);
        block_cell.SetVectorArray("_FrameData", array_render_data);
        Graphics.DrawMesh(render_plane, Matrix4x4.TRS(center + (Vector3.forward * ((Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale), player_cell_material, 0, Camera.main, 0, block_cell);

        array_render_trs[0] = Matrix4x4.TRS(center + new Vector3(0, 0, -2) + (Vector3.forward * ((Time.time - spawn_time) % 1)), Quaternion.identity, Vector3.one * render_core.scale);
        array_render_data[0] = frames[(int)(((Time.time - spawn_time) % render_core.animation_speed) / (render_core.animation_speed / frames.Length))];
        block_core.SetVectorArray("_FrameData", array_render_data);
        Graphics.DrawMeshInstanced(render_plane, 0, render_core.target, array_render_trs, 1, block_core);


        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = ray.direction.normalized;
        float t = (20.0f - ray.origin.z) / dir.z;
        Vector3 point = ray.origin + (dir * t);
        Vector2 eyeLoc = Vector3.MoveTowards(center, point, (point - center).magnitude / 100.0f).XY();
        if (!eye_bounds.OverlapPoint(eyeLoc))
        {
            eyeLoc = eye_bounds.ClosestPoint(eyeLoc);
        }
        var p = new RenderParams(core_eye_material);
        Graphics.RenderMesh(p, render_plane, 0, Matrix4x4.TRS(new Vector3(eyeLoc.x, eyeLoc.y, 0) + (center + new Vector3(0, 0, -2.5f) + (Vector3.forward * ((Time.time - spawn_time) % 1))), Quaternion.identity, Vector3.one));
        
    }*/
}
