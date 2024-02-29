using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMaterializable {
    public void OnMaterialize();
    public void OnPreDematerialize();
    public void OnPostDematerialize();
}

[AttributeUsage(AttributeTargets.Field)]
public class DataBridgeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class PrefabPathAttribute : Attribute
{
    public PrefabPathAttribute(string path)
    {
        this.path = path;
    }

    public string path;
}


[Serializable]
public abstract class AbstractPrefabDataBridge<T, D> : IDefault<D> where T : MonoBehaviour where D : AbstractPrefabDataBridge<T,D>
{
    enum FillMode
    {
        Materialize,
        Dematerialize,
        Default
    }
    public bool is_null = false;
    public bool is_enabled = true;
    [NonSerialized] protected WeakReference<T> instantiated = null;
    [NonSerialized] static Dictionary<Type, Dictionary<string, Action<AbstractPrefabDataBridge<T, D>, T, FillMode>>> reflection_bindings = new();
    [NonSerialized] static Dictionary<string, GameObject> prefab_bindings = new();

    static T Materialize(GameObject prefab, Transform parent)
    {
        return GameObject.Instantiate(prefab, parent).GetComponent<T>();
    }

    protected void Map()
    {
        if(reflection_bindings.ContainsKey(GetType()))
        {
            return;
        }
        var bindings = new Dictionary<string, Action<AbstractPrefabDataBridge<T, D>, T, FillMode>>();
        var data_props = GetType().GetFields();
        var concrete_props = typeof(T).GetFields();

        foreach (var prop in concrete_props)
        {
            if (prop.HasAttribute<DataBridgeAttribute>())
            {
                bool found_matching_attribute = false;
                foreach (var d in data_props)
                {
                    if (prop.Name == d.Name)
                    {
                        bindings.Add(prop.Name, (AbstractPrefabDataBridge<T, D> data, T concrete, FillMode mode) =>
                        {
                            switch (mode)
                            {
                                case FillMode.Materialize:
                                    prop.SetValue(concrete, d.GetValue(data));
                                    break;
                                case FillMode.Dematerialize:
                                    d.SetValue(data, prop.GetValue(concrete));
                                    break;
                                case FillMode.Default:
                                    d.SetValue(data, prop.GetValue(GetPrefab().GetComponent<T>()));
                                    break;
                            }
                        });
                        found_matching_attribute = true;
                        break;
                    }
                }

                if(!found_matching_attribute)
                {
                    Debug.LogWarning("Failed to find matching [DataBridge] " + typeof(T).FullName + "." + prop.Name + "; Expected in " + GetType().FullName + ".");
                }
            }
        }
        reflection_bindings.Add(GetType(), bindings);
    }


    public T GetInstantiatedReference()
    {
        if (instantiated != null)
        {
            if (instantiated.TryGetTarget(out T obj))
            {
                return obj;
            }
        }
        return null;
    }

    public void SetInstantiatedReference(T new_instance)
    {
        if (new_instance == null)
        {
            // (Ignore setting this to null, destroying the instance of T invalidates the WeakReference)
            Debug.LogWarning("Unsupported Null @ IPrefabDataBridge.SetInstantiatedReference");
            return;
        }
        SetTarget(new_instance);
    }

    public T Materialize(Transform parent)
    {
        if(is_null)
        {
            return null;
        }
        T data = Materialize(GetPrefab(), parent);
        Materialize(ref data);
        if (data is IMaterializable) ((IMaterializable) data)?.OnMaterialize();
        
        SetTarget(data);
        data.enabled = is_enabled;
        return data;
    }

    public void Dematerialize()
    {
        if (instantiated != null && instantiated.TryGetTarget(out var obj) && obj != null)
        {
            is_null = false;
            is_enabled = obj.enabled;
            if(obj is IMaterializable) ((IMaterializable)obj)?.OnPreDematerialize();
            Dematerialize(ref obj);
            if (obj is IMaterializable) ((IMaterializable)obj)?.OnPostDematerialize();
            SetTarget(obj);
        }
        else
        {
            is_null = true;
        }
    }

    void SetTarget(T obj)
    {
        if(instantiated == null)
        {
            instantiated = new WeakReference<T>(obj);
        }
        instantiated?.SetTarget(obj);
    }

    protected virtual GameObject GetPrefab()
    {
        string path = GetPrefabPath();
        if(!prefab_bindings.ContainsKey(path))
        { 
            prefab_bindings.Add(path, Resources.Load<GameObject>(path));
        }
        return prefab_bindings[path];
    }

    protected virtual string GetPrefabPath()
    {
        var attrib = GetType().GetAttribute<PrefabPathAttribute>();

        if (attrib != null)
            return attrib.path;

        Debug.LogWarning("Type " + GetType().FullName + " does not implement [PrefabPath(\"example\\path\")] but requests one, implement the Attribute or override GetPrefab() || GetPrefabPath().");
        return "";
    }

    protected virtual void Materialize(ref T obj)
    {
        Map();

        foreach(var t in reflection_bindings[GetType()].Values)
        {
            t(this, obj, FillMode.Materialize);
        }
    }
    protected virtual void Dematerialize(ref T obj)
    {
        Map();

        foreach (var t in reflection_bindings[GetType()].Values)
        {
            t(this, obj, FillMode.Dematerialize);
        }
    }

    public virtual D Default() 
    {
        Map();

        foreach (var t in reflection_bindings[GetType()].Values)
        {
            t(this, null, FillMode.Default);
        }

        return (D)this;
    }
}