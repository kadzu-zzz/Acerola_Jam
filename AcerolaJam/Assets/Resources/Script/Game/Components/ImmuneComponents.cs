using System;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ImmuneComponent : ISharedComponentData
{
    public int behaviour_type;

    public float attack_range_squared;
}

public struct AdiposeComponent : IComponentData
{
}

public struct PlateletComponent : IComponentData
{
    public float2 hold_a;
    public float2 hold_b;
}
