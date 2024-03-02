using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Colony
{
    struct RenderData
    {
        public Material target;
        public float scale;
        public float animation_speed;
    }

    GameMap map;
    public Coord core;
    public Dictionary<Coord, ColonyCell> cells = new();

    HashSet<Coord> open_neighbours = new();
    HashSet<Coord> growing_cells = new();

    public float growth_timer, growth_time;

    public MaterialPropertyBlock block_core = new();
    public MaterialPropertyBlock block_cell = new();
    RenderData render_core = new RenderData();
    RenderData render_cell = new RenderData();

    public static Mesh render_plane;

    public static Material player_cell_material, player_core_material;
    public static Material core_eye_material;
    public static Material enemy_triangle_material, enemy_square_material, enemy_core_material;

    Vector4[] frames;

    List<Matrix4x4> render_trs = new();
    List<Vector4> render_data = new();

    Matrix4x4[] array_render_trs;
    Vector4[] array_render_data;
    const int MaxRenderCount = 1024;
    float spawn_time;

    public PolygonCollider2D eye_bounds;

    public Vector2 grow_towards = Vector2.zero;

    bool valid = false;

    public bool player_colony = false;

    public Colony Setup(GameMap map, Coord core, bool player_colony = false)
    {
        if(render_plane == null)
        {
            render_plane = MeshMaker.Create(20.0f, 20.0f);

            player_cell_material = Resources.Load<Material>("Material/CellMaterial");
            player_core_material = new Material(player_cell_material);
            enemy_triangle_material = new Material(player_cell_material);
            enemy_square_material = new Material(player_cell_material);
            enemy_core_material = new Material(player_cell_material);
            core_eye_material = Resources.Load<Material>("Material/EyeMaterial");
            player_cell_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            player_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_overlay");
            core_eye_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_eye");
            enemy_triangle_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_triangle");
            enemy_square_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            enemy_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_core");
        }

        this.map = map;
        this.core = core;
        this.player_colony = player_colony;
        grow_towards = core.ConvertToGrid();
        spawn_time = Time.timeSinceLevelLoad;

        array_render_trs = new Matrix4x4[MaxRenderCount];
        array_render_data = new Vector4[MaxRenderCount];
        render_cell.target = player_colony ? player_cell_material : enemy_triangle_material;
        render_cell.scale = player_colony ? 1 : 2.1f;
        render_cell.animation_speed = 1.75f;

        render_core.target = player_colony ? player_core_material : enemy_core_material;
        render_core.scale = 1;
        render_core.animation_speed = 3.0f;

        if (player_colony)
        {
            frames = new Vector4[4];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Vector4(i * 0.25f, 0.0f, 0.25f, 1.0f);
            }
        } 
        else
        {
            frames = new Vector4[] { new Vector4(0, 0, 1, 1) };
        }

        foreach (var c in Coord.neighbours)
        {
            open_neighbours.Add((c + core).Simplify());
        }

        growth_time = 0.9f;
        growth_timer = growth_time;

        valid = true;
        return this;
    }
    
    public void Update()
    {
        if(valid)
        {
            foreach(var cell in cells.Values)
            {
                cell.Grow(Time.deltaTime);
                cell.Regen(Time.deltaTime);
            }

            growth_timer -= Time.deltaTime;
            if(growth_timer <= 0.0f)
            {
                growth_timer += growth_time;

                if (player_colony)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector3 dir = ray.direction.normalized;
                    float t = (20.0f - ray.origin.z) / dir.z;
                    grow_towards = ray.origin + (dir * t);
                }

                Grow();
            }
        }
    }

    void Grow()
    {
        if (open_neighbours.Count == 0)
            return;

        Coord lowest = null;
        float closest = float.MaxValue;
        foreach(Coord c in open_neighbours)
        {
            Vector3 pos = c.ConvertToGrid();
            float dist = Vector2.Distance(grow_towards, new Vector2(pos.x, pos.y));
            if(dist < closest)
            {
                closest = dist;
                lowest = c;
            }
        }
        if (lowest == null)
            return;

        open_neighbours.Remove(lowest);
        SpawnCell(lowest);

    }

    void SpawnCell(Coord c)
    {
        open_neighbours.Remove(c);
        growing_cells.Add(c);
        Coord from = core;
        foreach(var n in Coord.neighbours)
        {
            var temp = (n + c).Simplify();
            if (cells.ContainsKey(temp) && !growing_cells.Contains(temp))
            {
                from = temp;
                break;
            }
        }

        cells.Add(c, new ColonyCell().Setup(this, c, from));
        map.AddCell(cells[c]);
    }

    public void CellGrown(Coord c)
    {
        growing_cells.Remove(c);
        foreach(var n in Coord.neighbours)
        {
            Coord neighbour = (n + c).Simplify();
            if (map.valid_tiles.Contains(neighbour) && !cells.ContainsKey(neighbour) && neighbour != core)
            {
                open_neighbours.Add(neighbour);
            }
        }
    }

    public void Kill(ColonyCell c)
    {
        if(c.parent == this)
        {
            cells.Remove(c.coord);
            open_neighbours.Add(c.coord);
            map.RemoveCell(c);
        }
    }

    public int Size()
    {
        return cells.Count;
    }
    
    public void Render()
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
    }
}
