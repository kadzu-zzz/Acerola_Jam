using System;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;

public struct CellComponent : IComponentData
{
    public int belongs_to;

    public float health;
    public float max_health;

    public float power;

    public float consume;
    public float uv;
    public float fire;
    public float2 impulse;

    public bool was_consume;
    public bool was_burning;
    public bool was_uv;
    public bool was_impulse;
}

public struct CoreComponent : ISharedComponentData
{
    public int id;
    public bool player_colony;
}

public struct HazardComponent : IComponentData
{
    public float food;
    public float uv;
    public float fire;
    public float2 impulse;
}

public struct CoreData
{
    public float2 target;

    public int cells;
    public float2 center;

    public float repel;
    public float repel_r;
    public float cohesion;
    public float speed;

    public float strength;

    public bool fire_immunity;
    public bool uv_immunity;
}