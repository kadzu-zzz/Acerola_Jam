using System;
using UnityEngine;

[Serializable]
public class ProgramDataV1 : VersionedDataFile, IFileProgram, INamedDefault<ProgramDataV1>, IFileSaveLoad, IFileUpgrader
{
    //More serialzied stuff goes here
    [SerializeField] public int example_program_setting;
    
    public ProgramDataV1 Default(string input)
    {
        example_program_setting = -1;

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

    public ProgramDataV1()
    {
        file_name = "application_settings";
    }

    public override string GetFileExtension()
    {
        return ".dat";
    }

    public int GetTestData()
    {
        return example_program_setting;
    }

    public int GetTestDataV2()
    {
        throw new NotImplementedException();
    }

    public object Upgrade()
    {
        ProgramDataV2 output = new();

        output.Default(GetFileName());

        output.example_program_setting = example_program_setting;
        //New data
        //output.v2_setting;

        return output;
    }
}
