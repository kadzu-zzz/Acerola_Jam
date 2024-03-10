using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Colony
{    
    /*public void Render()
    {
        render_data.Clear();
        render_trs.Clear();

        render_trs.Add(Matrix4x4.TRS(core.ConvertToGrid() + (Vector3.forward * (-1.0f * (Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale));
        render_data.Add(frames[(int)(((Time.time - spawn_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))]);
        foreach (var c in cells)
        {
            Vector3 scale = Vector3.one * c.Value.GetSizePercentage(); 
            render_trs.Add(Matrix4x4.TRS(c.Value.GetBonusPos() + c.Key.ConvertToGrid() + (Vector3.forward * (-1.0f * (Time.time - c.Value.animation_time) % 1.25f)), Quaternion.identity, scale * render_cell.scale));
            render_data.Add(frames[(int)(((Time.time - c.Value.animation_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))]);
        }

        for(int c = 0; c < render_data.Count; c += MaxRenderCount)
        {
            int index = 0;
            int j;
            for(j = 0; j < Math.Min(c + MaxRenderCount, render_data.Count); j++)
            {
                array_render_data[index] = render_data[j];
                array_render_trs[index++] = render_trs[j];
            }
            block_cell.SetVectorArray("_FrameData", array_render_data);
            Graphics.DrawMeshInstanced(render_plane, 0, render_cell.target, array_render_trs, j, block_cell);
        }

        array_render_trs[0] = Matrix4x4.TRS(core.ConvertToGrid() + new Vector3(0, 0, -2) + (Vector3.forward * ((Time.time - spawn_time) % 1)), Quaternion.identity, Vector3.one * render_core.scale);
        array_render_data[0] = frames[(int)(((Time.time - spawn_time) % render_core.animation_speed) / (render_core.animation_speed / frames.Length))];
        block_core.SetVectorArray("_FrameData", array_render_data);
        Graphics.DrawMeshInstanced(render_plane, 0, render_core.target, array_render_trs, 1, block_core);

        if (player_colony)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 dir = ray.direction.normalized;
            float t = (20.0f - ray.origin.z) / dir.z;
            Vector3 point = ray.origin + (dir * t);
            Vector3 center = core.ConvertToGrid();
            Vector2 eyeLoc = Vector3.MoveTowards(center, point, (point - center).magnitude / 100.0f).XY();
            if (!eye_bounds.OverlapPoint(eyeLoc))
            {
                eyeLoc = eye_bounds.ClosestPoint(eyeLoc);
            }
            var p = new RenderParams(core_eye_material);
            Graphics.RenderMesh(p, render_plane, 0, Matrix4x4.TRS(new Vector3(eyeLoc.x, eyeLoc.y, 0) + (core.ConvertToGrid() + new Vector3(0, 0, -2.5f) + (Vector3.forward * ((Time.time - spawn_time) % 1))), Quaternion.identity, Vector3.one));
        }
    }*/
}
