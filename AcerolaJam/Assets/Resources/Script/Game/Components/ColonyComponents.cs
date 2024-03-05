using System;
using Unity.Entities;
using Unity.Mathematics;

public struct CellComponent : IComponentData
{
    public float health;
    public float max_health;
}

public struct CoreComponent : ISharedComponentData
{
    public int id;
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