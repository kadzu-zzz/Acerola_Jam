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
    public float death;
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
    public float death;
    public float2 impulse;
    public int gameobj_id;
    public float max_food;
    public static HazardComponent CreateFood(float food,int gameobj_id)
    {
        return new HazardComponent
        {
            gameobj_id = gameobj_id,
            food = food,
            max_food = food,
            fire = 0,
            death = 0,
            uv = 0,
            impulse = 0
        };
    }
    public static HazardComponent CreateUV(float uv)
    {
        return new HazardComponent
        {
            food = 0,
            max_food = 0,
            fire = 0,
            death = 0,
            uv = uv,
            impulse = 0,
            gameobj_id = -1
        };
    }
    public static HazardComponent CreateFire(float fire)
    {
        return new HazardComponent
        {
            food = 0,
            max_food = 0,
            fire = fire,
            death = 0,
            uv = 0,
            impulse = 0,
            gameobj_id = -1
        };
    }
    public static HazardComponent CreateDeath(float death)
    {
        return new HazardComponent
        {
            food = 0,
            max_food = 0,
            fire = 0,
            death = death,
            uv = 0,
            impulse = 0,
            gameobj_id = -1
        };
    }
    public static HazardComponent CreateImpulse(float2 impulse)
    {
        return new HazardComponent
        {
            food = 0,
            max_food = 0,
            fire = 0,
            death = 0,
            uv = 0,
            impulse = impulse,
            gameobj_id = -1
        };
    }
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

    public BlobAssetReference<Unity.Physics.Collider> collider_ref;
    public Action OnDestroy;
    public bool player;

    public static CoreData Create(float repel_strength, float repel_radius, float cohesion_strength, float movement_strength,
        float cell_power, bool fire_immune, bool uv_immune,  Action OnDestroy)
    {
        return new CoreData
        {
            cells = 0,
            repel = repel_strength,
            repel_r = repel_radius,
            cohesion = cohesion_strength,
            speed = movement_strength,
            strength = cell_power,
            fire_immunity = fire_immune,
            uv_immunity = uv_immune,
            OnDestroy = OnDestroy,
            player = false
        };
    }
}
