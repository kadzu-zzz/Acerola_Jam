using System;
using Unity.Entities;

[Serializable]
public struct RenderComponent : ISharedComponentData
{
    public int texture_id;
}

[Serializable]
public struct AnimatedRenderComponent : ISharedComponentData
{
    public int animation_id;
}

[Serializable]
public struct TimeOffsetComponent : IComponentData
{
    public float time_offset;
}