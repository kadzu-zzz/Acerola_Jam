using System;

[Serializable]
public abstract class VersionedDataFile : IFileType
{
    protected string file_name = "";

    public string GetFileName()
    {
        return file_name;
    }
    public abstract string GetFileExtension();
    public virtual string GetFilePathAddition()
    {
        return "/";
    }
}
