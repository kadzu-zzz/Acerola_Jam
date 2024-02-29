using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public static class FileManager<T> where T : VersionedDataFile, INamedDefault<T>, IFileSaveLoad, IFileType
{
    public static void New(ref T fresh_file)
    {
        Write(ref fresh_file);
    }

    public static void Load(ref T file_pointer)
    {
        Read(ref file_pointer);
        file_pointer.Load();
    }

    public static void Save(T data_file)
    {
        data_file.Save();
        Write(ref data_file);
    }

    public static bool Has(T file_pointer)
    {
        string path = Application.persistentDataPath + file_pointer.GetFilePathAddition() + file_pointer.GetFileName() + file_pointer.GetFileExtension();
        return File.Exists(path);
    }

    static void Write(ref T data)
    {
        string path = Application.persistentDataPath + data.GetFilePathAddition();
        string file_name = data.GetFileName() + data.GetFileExtension();
        Directory.CreateDirectory(path);
        file_name = path + file_name;
        if (File.Exists(file_name))
        {
            File.Delete(file_name);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(file_name, FileMode.CreateNew);
        bf.Serialize(file, data);
        file.Close();
    }

    static List<Type> GetFileTree(Type most_derived)
    {
        var tree = new List<Type>();

        for (var type = most_derived; type != null && type != typeof(object); type = type.BaseType)
        {
            if (type.IsAbstract || type.IsInterface)
                break;
            tree.Add(type);
        }

        return tree;
    }
    public static object TryDeserialize(Stream stream, Type targetType)
    {
        long originalPosition = stream.Position;
        IFormatter formatter = new BinaryFormatter();
        try
        {
            object deserialized = formatter.Deserialize(stream);
            if (targetType.IsInstanceOfType(deserialized))
            {
                stream.Position = originalPosition;
                return deserialized;
            }
        }
        catch (SerializationException e) 
        {
            Debug.Log(e);
        }
        stream.Position = originalPosition;
        return null;
    }

    static void Read(ref T data)
    {
        string path = Application.persistentDataPath + data.GetFilePathAddition();
        string file_name = data.GetFileName() + data.GetFileExtension();
        Directory.CreateDirectory(path);
        file_name = path + file_name;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(file_name, FileMode.Open);

        List<Type> class_list = GetFileTree(typeof(T));
        Type serialized_type = null;

        foreach(Type derived in class_list)
        {
            serialized_type = derived;
            var deserialized = TryDeserialize(file, derived);
            if(deserialized != null)
            {
                if(typeof(T) == derived)
                {
                    data = (T)deserialized;
                } 
                else
                {
                    Debug.Log("Upgrading File");
                    data = (T)UpgradeData(deserialized);
                }
                
                file.Close();
                return;
            }
        }
        file.Close();
        Debug.Log("Failed to find deserialization.");
    }

    public static object UpgradeData(object data)
    {
        object currentData = data;
        while (currentData is IFileUpgrader)
        {
            MethodInfo upgradeMethod = currentData.GetType().GetMethod("Upgrade", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (upgradeMethod == null)
            {
                break;
            }

            currentData = upgradeMethod.Invoke(currentData, null);
        }

        return currentData;
    }
}


