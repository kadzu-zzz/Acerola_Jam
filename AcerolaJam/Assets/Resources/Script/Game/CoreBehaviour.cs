using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using Unity.Collections;
using Unity.Physics.Extensions;

public class CoreBehaviour : MonoBehaviour
{
    struct RenderData
    {
        public UnityEngine.Material target;
        public float scale;
        public float animation_speed;
    }

    public MaterialPropertyBlock block_core;
    public MaterialPropertyBlock block_cell;
    RenderData render_core = new RenderData();
    RenderData render_cell = new RenderData();

    public static Mesh render_plane;

    public static UnityEngine.Material player_cell_material, player_core_material;
    public static UnityEngine.Material core_eye_material;
    public static UnityEngine.Material enemy_triangle_material, enemy_square_material, enemy_core_material;

    Vector4[] frames;

    List<Matrix4x4> render_trs = new();
    List<Vector4> render_data = new();

    Matrix4x4[] array_render_trs;
    Vector4[] array_render_data;
    const int MaxRenderCount = 1024;
    float spawn_time;
    public PolygonCollider2D eye_bounds;


    List<CellBehaviour> cells;
    public QuadTree<CellBehaviour> cell_map;
    List<QuadTreeNode<CellBehaviour>> close_by;

    public Vector2 center;
    public Vector2 min, max;
    public float movement_strength = 10.0f;
    public float cohesion_strength = 15.0f;
    public float repel_distance = 10.0f;
    public float repel_strength = 10.0f;

    int cell_id = 0;
    EntityArchetype archetype;

    void Start()
    {
        cells = new();
        cell_map = new QuadTree<CellBehaviour>(Vector2.zero, 1024.0f);
        close_by = new();
        block_core = new();
        block_cell = new();

        center = Vector2.zero;
        for (int i = 0; i < 68; i++)
        {
           // GrowCell();
        }
        if (render_plane == null)
        {
            render_plane = MeshMaker.Create(20.0f, 20.0f);

            player_cell_material = Resources.Load<UnityEngine.Material>("Material/CellMaterial");
            player_core_material = new UnityEngine.Material(player_cell_material);
            enemy_triangle_material = new UnityEngine.Material(player_cell_material);
            enemy_square_material = new UnityEngine.Material(player_cell_material);
            enemy_core_material = new UnityEngine.Material(player_cell_material);
            core_eye_material = Resources.Load<UnityEngine.Material>("Material/EyeMaterial");
            player_cell_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            player_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_overlay");
            core_eye_material.mainTexture = Resources.Load<Texture2D>("Sprite/core_eye");
            enemy_triangle_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_triangle");
            enemy_square_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_cell");
            enemy_core_material.mainTexture = Resources.Load<Texture2D>("Sprite/colony_enemy_core");
        }

        array_render_trs = new Matrix4x4[MaxRenderCount];
        array_render_data = new Vector4[MaxRenderCount];
        render_cell.target = player_cell_material ;
        render_cell.scale = 1;
        render_cell.animation_speed = 1.75f;

        render_core.target = player_core_material;
        render_core.scale = 1;
        render_core.animation_speed = 3.0f;
        frames = new Vector4[4];
        for (int i = 0; i < frames.Length; i++)
        {
            frames[i] = new Vector4(i * 0.25f, 0.0f, 0.25f, 1.0f);
        }
        //


        archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(LocalToWorld),
            typeof(TimeOffsetComponent),
            typeof(AnimatedRenderComponent),
            typeof(CellComponent),
            typeof(CoreComponent),
            typeof(PhysicsCollider),
            typeof(PhysicsWorldIndex),
            typeof(PhysicsVelocity),
            typeof(PhysicsMass)
          /* typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor)*/);

        int core_id = ColonySystem.handle.NewCore(new CoreData { cohesion = cohesion_strength, 
            strength = 1.0f, speed = movement_strength, repel = repel_strength, repel_r = repel_distance, 
            target = float2.zero, center = float2.zero, cells = 0, fire_immunity = false, uv_immunity = false });
        int core_id2 = ColonySystem.handle.NewCore(new CoreData
        {
            cohesion = cohesion_strength,
            strength = 1.0f,
            speed = movement_strength,
            repel = repel_strength,
            repel_r = repel_distance,
            target = new float2(0, 150),
            center = float2.zero,
            cells = 0,
            fire_immunity = false,
            uv_immunity = false
        });

        for (int i = 0; i < 250; i++)
        {
            Spawn(core_id, new Vector2(0, -150) + new Vector2(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f)), true);
        }
        for (int i = 0; i < 100; i++)
        {
            Spawn(core_id2, new Vector2(0,100) + new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)), false);
        }

        CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(250, 0, 0), new float3(10, 1000, 1000));
        CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(-250, 0, 0), new float3(10, 1000, 1000));
        CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(0, 250, 0), new float3(1000, 10, 1000));
        CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(0, -250, 0), new float3(1000, 10, 1000));
        Entity e = CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(0, 0, 0), new float3(25, 25, 1000), true);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new HazardComponent
        {
            fire = 0.01f,
            uv = 0.0f,
            food = 0.0f,
            impulse = float2.zero
        }); 
        e = CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(0, -100, 0), new float3(100, 100, 1000), true);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new HazardComponent
        {
            fire = 0.0f,
            uv = 0.5f,
            food = 0.0f,
            impulse = float2.zero
        });
        e = CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(-100, -100, 0), new float3(100, 100, 1000), true);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new HazardComponent
        {
            fire = 0.0f,
            uv = 0.0f,
            food = 0.0f,
            impulse = new float2(100, 100)
        });
        e = CreateKinematicCube(World.DefaultGameObjectInjectionWorld.EntityManager, new float3(-100, 0, 0), new float3(100, 100, 1000), true);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new HazardComponent
        {
            fire = 0.0f,
            uv = 0.0f,
            food = 1000000.0f,
            impulse = float2.zero
        });
    }

    void Spawn(int core_id, Vector2 position, bool player)
    {
        int id = RenderSystem.handle.GetTextureIndex(Resources.Load<Texture2D>("Sprite/colony_cell"));
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = manager.CreateEntity(archetype);

        // 3
        manager.AddComponentData(entity, new LocalTransform { Position = new Vector3(position.x, position.y, 0.0f), Rotation = Quaternion.identity, Scale = 1f });

        manager.AddComponentData(entity, new TimeOffsetComponent { time_offset = UnityEngine.Time.time + UnityEngine.Random.Range(-100.0f, 0.0f) });

        manager.AddSharedComponentManaged(entity, new AnimatedRenderComponent
        {
            animation_id = id,
        });

        if (!RenderSystem.handle.HasAnim(id)) {

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

        }) ;

        manager.AddSharedComponentManaged(entity, new CoreComponent
        {
            id = core_id,
            player_colony = player
        });

        manager.AddSharedComponentManaged(entity, new PhysicsWorldIndex
        {
            Value = 0
        });

        //var cyl = CreateCylinder((uint)1 << core_id, ~(uint)1 << core_id);
        var cyl = CreateCylinder((uint)1 << core_id, ~(uint)0 );
        manager.SetComponentData(entity, new PhysicsCollider
        {
            Value = cyl
        });

        manager.AddComponentData(entity, new PhysicsVelocity
         {
             Linear = float3.zero,
             Angular = float3.zero
         });

        unsafe
        {
            Unity.Physics.Collider* colliderPtr = (Unity.Physics.Collider*)cyl.GetUnsafePtr();
            var mass =  PhysicsMass.CreateDynamic(colliderPtr->MassProperties, 1.0f);
            mass.InverseInertia = float3.zero;
            
            manager.AddComponentData(entity, mass);
        }
       /*  manager.AddComponentData(entity, new PhysicsDamping
         {
             Angular = 0.1f,
             Linear = 0.1f
         });


         manager.AddComponentData(entity, new PhysicsGravityFactor
         {
             Value = 0.0f
         });*/

    }
    public static Entity CreateKinematicCube(EntityManager entityManager, float3 center, float3 size, bool trigger = false)
    {
        // Create an entity
       var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(LocalToWorld),
            typeof(PhysicsCollider),
            typeof(PhysicsWorldIndex),
            typeof(PhysicsVelocity),
            typeof(PhysicsMass));
        Entity entity = entityManager.CreateEntity(archetype);

        BlobAssetReference<Unity.Physics.Collider> collider = CreateCubeCollider(size, trigger);

        unsafe
        {
            if (trigger)
                collider.AsPtr()->SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
        }
        entityManager.AddComponentData(entity, new PhysicsCollider { Value = collider });

        entityManager.AddComponentData(entity, new LocalTransform{ Position = center, Rotation = Quaternion.identity, Scale = 1.0f });
        entityManager.AddComponentData(entity, new LocalToWorld {});

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

    private static BlobAssetReference<Unity.Physics.Collider> CreateCubeCollider(float3 size, bool trigger)
    {
        BoxGeometry boxGeometry = new BoxGeometry
        {
            Center = float3.zero, 
            Size = size, 
            Orientation = quaternion.identity 
            
        };

        return Unity.Physics.BoxCollider.Create(boxGeometry);
    }

    private BlobAssetReference<Unity.Physics.Collider> CreateCylinder(uint BelongsTo, uint CollidesWith)
    {
        CylinderGeometry geo = new CylinderGeometry
        {
            Center = float3.zero,
            Radius = 7.5f,
            Height = 10.0f,
            Orientation = Quaternion.identity,
            SideCount = 16
        };

        return BlobAssetStore.Instance().Create(0, geo, new CollisionFilter { BelongsTo = BelongsTo, CollidesWith = CollidesWith });
    }

    void Update()
    {
        if (cells.Count == 0)
            return;

        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = ray.direction.normalized;
        float t = (20.0f - ray.origin.z) / dir.z;
        Vector3 point = ray.origin + (dir * t);

        cell_map.Clear();
        cell_map = new(center, Mathf.Max(new float[] { center.x - min.x, max.x - center.x, center.y - min.y, max.y - center.y } ));
        foreach (CellBehaviour cell in cells)
        {
            cell_map.Insert(cell.position, cell);
        }

        Vector2 total = Vector2.zero;
        Vector2 center_force = Vector2.zero, repulsion_force = Vector2.zero;
        Vector2 target = point.XY();

        min = center;
        max = center;

        for (int i = 0; i < cells.Count; i++)
        {
            total += cells[i].position;
            center_force = (center - cells[i].position).normalized * cohesion_strength;
            repulsion_force = Vector2.zero;

            close_by.Clear();
            cell_map.Query(new(cells[i].position.x, cells[i].position.y, repel_distance), ref close_by);
            for (int j = 0; j < close_by.Count; j++)
            {
                if (cells[i].id == close_by[j].data.id)
                    continue;
                Vector2 dist = cells[i].position - close_by[j].data.position;
                float mag = dist.magnitude;
                if (mag < repel_distance)
                {
                    if (mag != 0)
                    {
                        repulsion_force += (dist.normalized / mag);
                    }
                    else if(repulsion_force == Vector2.zero)
                    {
                        repulsion_force += new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                    }
                }
            }
            cells[i].position += (((target - cells[i].position).normalized * movement_strength) + (center_force + (repulsion_force * repel_strength))) * Time.deltaTime;

            if (cells[i].position.x < min.x)
                min.x = cells[i].position.x;
            if (cells[i].position.x > max.x)
                max.x = cells[i].position.x;
            if (cells[i].position.y < min.y)
                min.y = cells[i].position.y;
            if (cells[i].position.y > max.y)
                max.y = cells[i].position.y;
        }
        if (cells.Count > 0)
        {
            center = total / cells.Count;
        }
        else
        {
            center = Vector2.zero;
        }


        Render();
    }

    public void GrowCell()
    {
        cells.Add(new CellBehaviour(cell_id++, transform.position.XY() + new Vector2(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f))));
    }

    public void Render()
    {
        render_data.Clear();
        render_trs.Clear();

        foreach (var c in cells)
        {
            Vector3 scale = Vector3.one;
            render_trs.Add(Matrix4x4.TRS(new Vector3(c.position.x, c.position.y, 0) + (Vector3.forward * ((Time.time - c.spawn_time) % 1.25f)), Quaternion.identity, scale * render_cell.scale));
            render_data.Add(frames[(int)(((Time.time - c.spawn_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))]);
        }

        for (int c = 0; c < render_data.Count; c += MaxRenderCount)
        {
            int index = 0;
            int j;
            for (j = 0; j < Mathf.Min(c + MaxRenderCount, render_data.Count); j++)
            {
                array_render_data[index] = render_data[j];
                array_render_trs[index++] = render_trs[j];
            }
            block_cell.SetVectorArray("_FrameData", array_render_data);
            Graphics.DrawMeshInstanced(render_plane, 0, render_cell.target, array_render_trs, j, block_cell);
        }

        Vector3 center = new Vector3(this.center.x, this.center.y, -3.0f);
        array_render_data[0] = frames[(int)(((Time.time - spawn_time) % render_cell.animation_speed) / (render_cell.animation_speed / frames.Length))];
        //array_render_trs[0] = Matrix4x4.TRS(center + (Vector3.forward * ((Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale);
        block_cell.SetVectorArray("_FrameData", array_render_data);
        Graphics.DrawMesh(render_plane, Matrix4x4.TRS(center + (Vector3.forward * ((Time.time - spawn_time) % 1.25f)), Quaternion.identity, Vector3.one * render_cell.scale), player_cell_material, 0, Camera.main, 0, block_cell);

        array_render_trs[0] = Matrix4x4.TRS(center + new Vector3(0, 0, -2) + (Vector3.forward * ((Time.time - spawn_time) % 1)), Quaternion.identity, Vector3.one * render_core.scale);
        array_render_data[0] = frames[(int)(((Time.time - spawn_time) % render_core.animation_speed) / (render_core.animation_speed / frames.Length))];
        block_core.SetVectorArray("_FrameData", array_render_data);
        Graphics.DrawMeshInstanced(render_plane, 0, render_core.target, array_render_trs, 1, block_core);


        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = ray.direction.normalized;
        float t = (20.0f - ray.origin.z) / dir.z;
        Vector3 point = ray.origin + (dir * t);
        Vector2 eyeLoc = Vector3.MoveTowards(center, point, (point - center).magnitude / 100.0f).XY();
        if (!eye_bounds.OverlapPoint(eyeLoc))
        {
            eyeLoc = eye_bounds.ClosestPoint(eyeLoc);
        }
        var p = new RenderParams(core_eye_material);
        Graphics.RenderMesh(p, render_plane, 0, Matrix4x4.TRS(new Vector3(eyeLoc.x, eyeLoc.y, 0) + (center + new Vector3(0, 0, -2.5f) + (Vector3.forward * ((Time.time - spawn_time) % 1))), Quaternion.identity, Vector3.one));
        
    }
}
