using System;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;
using Unity.VisualScripting;
using System.Security.AccessControl;
using JetBrains.Annotations;

public class GameLevel
{
    public readonly string name;
    Action<GameMap> setup;
    Func<GameMap, bool> win_check;

    static int game_obj_count = 0;
    public static Dictionary<int, GameObject> game_obj_links = new();

    public GameLevel(string name, Action<GameMap> setup, Func<GameMap, bool> win_check)
    {
        this.name = name;
        this.setup = setup;
        this.win_check = win_check;
    }

    ~GameLevel()
    {
        game_obj_links.Clear();
    }

    public void Setup(GameMap map)
    {
        game_obj_count = 0;
        game_obj_links.Clear();
        setup(map);
    }

    public bool CheckVictory(GameMap map)
    {
        return win_check(map);
    }

    public static int CreateColony(Vector2 center, int count, Vector2 range, ColonyType type, CoreData core)
    {        
        core.target = new float2(center.x,center.y);
        core.center = core.target;

        int col_id = ColonySystem.handle.NewCore(core);

        core = ColonySystem.handle.GetCore(col_id);
        uint layer = (uint)(1 << col_id);
        var cyl = CreateCylinder(layer, ~layer);
        core.collider_ref = cyl;
        core.player = type == ColonyType.PLAYER;
        ColonySystem.handle.UpdateCore(col_id, core);

        for (int i = 0; i < count; i++)
        {
            SpawnCell(col_id, center, -range, range, core.player);
        }

        return col_id;
    }

    public static void GenerateCells(int colony, int count)
    {
        if (!ColonySystem.handle.HasCore(colony))
            return;

        CoreData core = ColonySystem.handle.GetCore(colony);
        for (int i = 0; i < count; i++)
        {
            SpawnCell(colony, core.center, -new Vector2(2.5f, 2.5f), new Vector2(2.5f, 2.5f), core.player);
        }
    }

    public static void CreateFoodHazard(Vector2 center, List<Vector2> polygon, HazardComponent hazard, UnityEngine.Material material)
    {
        polygon.Add(polygon[0]);
        var obj = PolygonMeshFactory.CreatePolygonObject(center, polygon, material);
        PolygonMeshFactory.CreateLineRenderObject(center, polygon, UnityEngine.Color.white, obj);

        game_obj_links.Add(++game_obj_count, obj);
        var hCopy = hazard;
        hCopy.gameobj_id = game_obj_count;

        CreateTriggerEntity(center, polygon, hCopy);
        obj.transform.SetPositionAndRotation(center, Quaternion.identity);
    }
    public static void CreateHazard(Vector2 center, List<Vector2> polygon, HazardComponent hazard, UnityEngine.Material material)
    {
        CreateTriggerEntity(center, polygon, hazard);

        polygon.Add(polygon[0]);
        var obj = PolygonMeshFactory.CreatePolygonObject(center, polygon, material);
        PolygonMeshFactory.CreateLineRenderObject(center, polygon, UnityEngine.Color.white, obj);
        obj.transform.SetPositionAndRotation(center, Quaternion.identity);
    }

    public static void CreateBlocking(Vector2 center, List<Vector2> polygon, UnityEngine.Material material, bool fill, bool outline, UnityEngine.Color outline_colour = default)
    {
        CreateBlockingEntity(center, polygon);

        polygon.Add(polygon[0]);
        if (fill && outline)
        {
            var obj = PolygonMeshFactory.CreatePolygonObject(center, polygon, material);
            PolygonMeshFactory.CreateLineRenderObject(center, polygon, UnityEngine.Color.white, obj);
            obj.transform.SetPositionAndRotation(center, Quaternion.identity);
        } else if(fill)
        {
            PolygonMeshFactory.CreatePolygonObject(center, polygon, material).transform.SetPositionAndRotation(center, Quaternion.identity); ;
        } else if(outline)
        {
            PolygonMeshFactory.CreateLineRenderObject(center, polygon, outline_colour).transform.SetPositionAndRotation(center, Quaternion.identity);;
        }
    }
    static Entity SpawnCell(int core_id, Vector2 pos, Vector2 min_range, Vector2 max_range, bool player)
    {
        return SpawnColony(core_id, pos + new Vector2(UnityEngine.Random.Range(min_range.x, max_range.x), UnityEngine.Random.Range(min_range.y, max_range.y)), player);
    }
    static Entity SpawnColony(int core_id, Vector2 position, bool player)
    {
        int id = RenderSystem.handle.GetTextureIndex(Resources.Load<Texture2D>("Sprite/colony_cell"));
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(LocalToWorld),
            typeof(TimeOffsetComponent),
            typeof(AnimatedRenderComponent),
            typeof(CellComponent),
            typeof(CoreComponent),
            typeof(PhysicsCollider),
            typeof(PhysicsWorldIndex),
            typeof(PhysicsVelocity),
            typeof(PhysicsMass), 
            typeof(SceneDestroyFlagComponent));
        Entity entity = manager.CreateEntity(archetype);

        // 3
        manager.AddComponentData(entity, new LocalTransform { Position = new Vector3(position.x, position.y, 0.0f), Rotation = Quaternion.identity, Scale = 1f });

        manager.AddComponentData(entity, new TimeOffsetComponent { time_offset = UnityEngine.Time.time + UnityEngine.Random.Range(-100.0f, 0.0f) });

        manager.AddSharedComponentManaged(entity, new AnimatedRenderComponent
        {
            animation_id = id,
        });

        if (!RenderSystem.handle.HasAnim(id))
        {
            NativeArray<float4> array = new NativeArray<float4>(4, Allocator.Persistent);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Vector4(i * 0.25f, 0.0f, 0.25f, 1.0f);
            }

            RenderSystem.handle.SetAnimFrame(id, array);
        }

        manager.AddComponentData(entity, new CellComponent
        {
            belongs_to = core_id,
            health = 1.0f,
            max_health = 1.0f,

            power = 0.1f,
            consume = 0.0f,
            uv = 0.0f,
            fire = 0.0f,
            impulse = float2.zero,

            was_burning = false,
            was_consume = false,
            was_impulse = false,
            was_uv = false
        });

        manager.AddSharedComponentManaged(entity, new CoreComponent
        {
            id = core_id,
            player_colony = player
        });

        manager.AddSharedComponentManaged(entity, new PhysicsWorldIndex
        {
            Value = 0
        });

        var d = ColonySystem.handle.GetCore(core_id);
        manager.SetComponentData(entity, new PhysicsCollider
        {
            Value = d.collider_ref
        });

        manager.AddComponentData(entity, new PhysicsVelocity
        {
            Linear = float3.zero,
            Angular = float3.zero
        });

        unsafe
        {
            Unity.Physics.Collider* colliderPtr = (Unity.Physics.Collider*)d.collider_ref.GetUnsafePtr();
            var mass = PhysicsMass.CreateDynamic(colliderPtr->MassProperties, 1.0f);
            mass.InverseInertia = float3.zero;

            manager.AddComponentData(entity, mass);
        }

        return entity;
    }
    static Entity CreateBlockingEntity(Vector2 center, List<Vector2> points)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
             typeof(LocalTransform),
             typeof(LocalToWorld),
             typeof(PhysicsCollider),
             typeof(PhysicsWorldIndex),
             typeof(PhysicsVelocity),
             typeof(PhysicsMass),
            typeof(SceneDestroyFlagComponent));
        Entity entity = entityManager.CreateEntity(archetype);

        BlobAssetReference<Unity.Physics.Collider> collider = CreatePolygonCollider(points);

        entityManager.AddComponentData(entity, new PhysicsCollider { Value = collider });

        entityManager.AddComponentData(entity, new LocalTransform { Position = new float3(center.x, center.y, 0.0f), Rotation = Quaternion.identity, Scale = 1.0f });
        entityManager.AddComponentData(entity, new LocalToWorld { });

        entityManager.AddComponentData(entity, new PhysicsVelocity
        {
            Linear = float3.zero,
            Angular = float3.zero
        });

        var physicsMass = PhysicsMass.CreateKinematic(MassProperties.UnitSphere);
        entityManager.AddComponentData(entity, physicsMass);

        entityManager.AddComponentData(entity, new PhysicsGravityFactor { Value = 0 });

        entityManager.AddSharedComponentManaged(entity, new PhysicsWorldIndex
        {
            Value = 0
        });

        return entity;
    }
    static BlobAssetReference<Unity.Physics.Collider> CreateCylinder(uint BelongsTo, uint CollidesWith)
    {
        CylinderGeometry geo = new CylinderGeometry
        {
            Center = float3.zero,
            Radius = 7.5f,
            Height = 10.0f,
            Orientation = Quaternion.identity,
            SideCount = 16
        };

        var cyl = CylinderCollider.Create(geo, new CollisionFilter { BelongsTo = BelongsTo, CollidesWith = CollidesWith });
        return cyl;
    }

    static Entity CreateTriggerEntity(Vector2 center, List<Vector2> points, HazardComponent hazard)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
             typeof(LocalTransform),
             typeof(LocalToWorld),
             typeof(PhysicsCollider),
             typeof(PhysicsWorldIndex),
             typeof(PhysicsVelocity),
             typeof(PhysicsMass), 
             typeof(HazardComponent),
            typeof(SceneDestroyFlagComponent));
        Entity entity = entityManager.CreateEntity(archetype);

        BlobAssetReference<Unity.Physics.Collider> collider = CreatePolygonCollider(points);

        unsafe
        {
            collider.AsPtr()->SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
        }
        entityManager.AddComponentData(entity, new PhysicsCollider { Value = collider });

        entityManager.AddComponentData(entity, new LocalTransform { Position = new float3(center.x, center.y, 0.0f), Rotation = Quaternion.identity, Scale = 1.0f });
        entityManager.AddComponentData(entity, new LocalToWorld { });

        entityManager.AddComponentData(entity, new PhysicsVelocity
        {
            Linear = float3.zero,
            Angular = float3.zero
        });

        var physicsMass = PhysicsMass.CreateKinematic(MassProperties.UnitSphere);
        entityManager.AddComponentData(entity, physicsMass);

        entityManager.AddComponentData(entity, new PhysicsGravityFactor { Value = 0 });

        entityManager.AddSharedComponentManaged(entity, new PhysicsWorldIndex
        {
            Value = 0
        });
        entityManager.AddComponentData(entity, hazard);

        return entity;
    }

    const float height_offset = 100;

    static BlobAssetReference<Unity.Physics.Collider> CreatePolygonCollider(List<Vector2> points)
    {
        NativeArray<float3> hull_points = new NativeArray<float3>((points.Count * 6) + ((points.Count - 2) * 6), Allocator.Temp);

        points.Add(points[0]);
        int index = 0;
        for(int i = 0; i < points.Count - 1; i++)
        {
            hull_points[index++] = new float3(points[i], -height_offset);
            hull_points[index++] = new float3(points[i], height_offset);
            hull_points[index++] = new float3(points[i + 1], height_offset);
            hull_points[index++] = new float3(points[i], height_offset);
            hull_points[index++] = new float3(points[i + 1], height_offset);
            hull_points[index++] = new float3(points[i + 1], -height_offset);
        }
        points.RemoveAt(points.Count - 1);
        List<Vector2> copy = new List<Vector2>(points);
        for(int i = copy.Count - 1; i > copy.Count - 3; i--)
        {
            hull_points[index++] = new float3(points[i], height_offset);
            hull_points[index++] = new float3(points[i - 1], height_offset);
            hull_points[index++] = new float3(points[i - 2], height_offset);
            hull_points[index++] = new float3(points[i], -height_offset);
            hull_points[index++] = new float3(points[i - 1], -height_offset);
            hull_points[index++] = new float3(points[i - 2], -height_offset);

            copy.RemoveAt(copy.Count - 1);
            if (copy.Count < 3)
                break;
        }

        

        var polygonCollider = ConvexCollider.Create(hull_points, default, CollisionFilter.Default) ;
     
        hull_points.Dispose();

        return polygonCollider;
    }

    public static void CreatePetriDishBorder(float radius, int divisions)
    {
        List<Vector2> points = CreateCircle(radius, divisions);

        PolygonMeshFactory.CreateLineRenderObject(Vector2.zero, points, Color.white);
        points.Add(points[0]);

        for (int i = 0; i < points.Count - 1; i++)
        {
            CreateBlockingEntity(Vector2.zero, new List<Vector2> { points[i], points[i + 1], points[i + 1] + points[i + 1], points[i] + points[i] });
        }
    }

    public static List<Vector2> CreateCircle(float radius, int divisions)
    {
        List<Vector2> points = new();

        for (float f = 0; f < Mathf.PI * 2; f += (Mathf.PI * 2) / divisions)
        {
            points.Add(new Vector2((Mathf.Cos(f) * radius), Mathf.Sin(f) * radius));
        }

        return points;
    }
}