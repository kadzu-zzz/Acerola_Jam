using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;

public class BlobAssetKey
{
    public Unity.Physics.ColliderType type;
    public int key_id;
    public CollisionFilter filter;

    public override bool Equals(object obj)
    {
        return obj is BlobAssetKey key &&
               type == key.type &&
               key_id == key.key_id &&
               filter.Equals(key.filter);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, key_id, filter);
    }
}

public class BlobAssetStore 
{
    public static BlobAssetStore Instance()
    {
        if (instance == null)
            instance = new();
        return instance;
    }

    static BlobAssetStore instance;

    Dictionary<BlobAssetKey, BlobAssetReference<Unity.Physics.Collider>> blobs;

    public BlobAssetStore()
    {
        blobs = new();
    }
    
    ~BlobAssetStore()
    {
        foreach (var value in blobs.Values)
        {
            value.Dispose();
        }
    }

    public BlobAssetReference<Collider> Create(int key_id, CylinderGeometry cyl, CollisionFilter filter)
    {
        BlobAssetKey key = new BlobAssetKey { filter= filter, key_id = key_id, type = ColliderType.Cylinder };

        if (blobs.ContainsKey(key))
            return blobs[key];
        var v = CylinderCollider.Create(cyl, filter);
        blobs.Add(key, v);
        return blobs[key];
    }
    public BlobAssetReference<Collider> Create(int key_id, BoxGeometry box, CollisionFilter filter)
    {
        BlobAssetKey key = new BlobAssetKey { filter = filter, key_id = key_id, type = ColliderType.Box };

        if (blobs.ContainsKey(key))
            return blobs[key];
        var v = BoxCollider.Create(box, filter);
        blobs.Add(key, v);
        return blobs[key];
    }
}
