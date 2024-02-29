using System;
using UnityEngine;

public class ConcreteObject : MonoBehaviour
{
    [DataBridge]
    public int data = 0;
    [DataBridge]
    public float real = 0;
    [DataBridge]
    public float unimplemented_real = 0;

    void Update()
    {
        data += 1;   
    }
}

[Serializable, PrefabPath("Prefab/TestObject")]
public class DataObject : AbstractPrefabDataBridge<ConcreteObject, DataObject>
{
    public int data;
    public float real;
}