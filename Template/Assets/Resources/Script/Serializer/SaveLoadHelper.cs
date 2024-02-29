using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public static class SaveLoadHelper<T> where T : VersionedDataFile, INamedDefault<T>, IFileSaveLoad, new()
{
    public static T New(string file_name = "default", bool overwrite = false)
    {
        T temp_file = new T().Default(file_name);
        if(!FileManager<T>.Has(temp_file) || overwrite)
        {
            FileManager<T>.New(ref temp_file);
        }
        return temp_file;
    }

    public static T Load(string file_name)
    {
        T temp_file = new T().Default(file_name);

        if(FileManager<T>.Has(temp_file))
        {
            FileManager<T>.Load(ref temp_file);
            return temp_file;
        }
        return New(file_name);
    }

    public static void Save(T data)
    {
        FileManager<T>.Save(data);
    }
}
