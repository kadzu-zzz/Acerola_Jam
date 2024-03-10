using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEditor.Search;
using UnityEngine.SceneManagement;



[UpdateInGroup(typeof(PresentationSystemGroup))]
public struct SceneDestroyFlagComponent : IComponentData { }

public partial class LevelChangeCleanupSystem : SystemBase
{
    private EntityQuery destroyQuery = default;

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

    private void RealUpdateOnSceneChange(Scene unloaded)
    {
        if (unloaded.isSubScene)
            return;

        EntityManager.DestroyEntity(destroyQuery);
    }
}