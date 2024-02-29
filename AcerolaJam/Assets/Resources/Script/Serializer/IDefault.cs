public interface IDefault<T>
{
    public T Default();
}
public interface INamedDefault<T>
{
    public T Default(string input);
}
