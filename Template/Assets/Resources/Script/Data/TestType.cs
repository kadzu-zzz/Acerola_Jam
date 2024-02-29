using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class TestType : ISaveLoad<TestType, SaveDataV1>
{

    [SerializeField] public int test_data;

    public TestType Default()
    {
        test_data = 42;

        return this;
    }

    public void Load(SaveDataV1 data)
    {
        data.test_type = this;
    }

    public void postLoad(SaveDataV1 data)
    {
        Debug.Log(data.test_type.test_data);
    }

    public void Save(SaveDataV1 data)
    {
        test_data = data.test_type.test_data;
    }
}
