using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataV1 : VersionedDataFile, IFileSave, INamedDefault<SaveDataV1>, IFileSaveLoad//, IFileUpgrader
{
    //More serialzied stuff goes here
    [SerializeField] public int test_data;

    [SerializeField] public TestType test_type;
    [SerializeField] public DataObject test_instantiate;
        
    public SaveDataV1 Default(string input)
    {
        file_name = input;

        test_data = -1;
        test_type = new TestType().Default();

        test_instantiate = new DataObject().Default();

        return this;
    }

    public void Save()
    {
        test_type.Save(this);
        test_instantiate.Dematerialize();
    }

    public void Load()
    {
        test_type.Load(this);
    }

    public void postLoad()
    {
        test_type.postLoad(this);
        test_instantiate.Materialize(null);
    }

    public SaveDataV1() { }
    public SaveDataV1(string input)
    {
        file_name = input;
    }

    public override string GetFileExtension()
    {
        return ".sav";
    }
    public override string GetFilePathAddition()
    {
        return "/" + file_name + "/";
    }

    public int GetTestData()
    {
        return test_data;
    }

    public TestType GetTestType()
    { 
        return test_type;
    }
}
