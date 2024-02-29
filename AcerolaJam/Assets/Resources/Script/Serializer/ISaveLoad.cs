
//Serialization for Non Monobehaviours.
public interface ISaveLoad<T, Data> : IDefault<T> where T : ISaveLoad<T, Data> where Data : VersionedDataFile
{
    public void Save(Data data);
    public void Load(Data data);

    public void postLoad(Data data);

}

public interface IFileSaveLoad
{
    public void Save();
    public void Load();
    public void postLoad();
}