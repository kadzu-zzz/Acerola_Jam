using System;
using UnityEngine;

[Serializable]
public class ProgramDataV2 : ProgramDataV1, INamedDefault<ProgramDataV2>, IFileSaveLoad
{
    [SerializeField]
    public int v2_setting;
    
    public new ProgramDataV2 Default(string input)
    {
        base.Default(input);
        v2_setting = -1;

        return this;
    }

    public new void Save()
    {
        base.Save();

    }

    public new void Load()
    {
        base.Load();

    }

    public new void postLoad()
    {
        base.postLoad();

    }

    public new int GetTestDataV2()
    {
        return v2_setting;
    }
}
