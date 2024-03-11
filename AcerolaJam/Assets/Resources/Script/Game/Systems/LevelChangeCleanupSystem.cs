using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.SceneManagement;

public struct SceneDestroyFlagComponent : IComponentData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(UpdateWorldTimeSystem))]
public partial class LevelChangeCleanupSystem : SystemBase
{
    private static EntityQuery destroyQuery = default;

    protected override void OnCreate()
    {
        destroyQuery = new EntityQueryBuilder(Allocator.Temp).WithAny<SceneDestroyFlagComponent>().Build(this);
        SceneManager.sceneUnloaded += RealUpdateOnSceneChange;
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneUnloaded -= RealUpdateOnSceneChange;
    }

    protected override void OnUpdate()
    {
    }

    public static void ForceClean()
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(destroyQuery);
    }

    private void RealUpdateOnSceneChange(Scene unloaded)
    {
        if (unloaded.isSubScene)
            return;

        EntityManager.DestroyEntity(destroyQuery);
    }
}