using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataV1 : VersionedDataFile, IFileSave, INamedDefault<SaveDataV1>, IFileSaveLoad//, IFileUpgrader
{
    [SerializeField] public int level_progress;
        
    public SaveDataV1 Default(string input)
    {
        file_name = input;

        level_progress = -1;

        return this;
    }

    public void Save()
    {
    }

    public void Load()
    {
    }

    public void postLoad()
    {
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

    public int GetLevelProgress()
    {
        return level_progress;
    }
}
