using JetBrains.Annotations;
using System.Collections.Generic;

public class BiDictionary<T1, T2>
{
    Dictionary<T1, T2> forward;
    Dictionary<T2, T1> reverse;

    public BiDictionary(int size_hint = 64)
    {
        forward = new(size_hint);
        reverse = new(size_hint);
    }

    public bool ContainsForward(T1 item)
    {
        return forward.ContainsKey(item);
    }

    public bool ContainsReverse(T2 item)
    {
        return reverse.ContainsKey(item);
    }

    public bool Add(T1 a, T2 b)
    {
        if (!forward.ContainsKey(a) && !reverse.ContainsKey(b))
        {
            forward.Add(a, b);
            reverse.Add(b, a);
            return true;
        }
        return false;
    }

    public T2 GetForward(T1 item)
    {
        if (forward.ContainsKey(item))
        {
            return forward[item];
        }
        return default;
    }

    public T1 GetReverse(T2 item)
    {
        if (reverse.ContainsKey(item))
        {
            return reverse[item];
        }
        return default;
    }

    public bool RemoveForward(T1 item)
    {
        if (forward.ContainsKey(item))
        {
            reverse.Remove(forward[item]);
            forward.Remove(item);
            return true;
        }
        return false;
    }

    public bool RemoveReverse(T2 item)
    {
        if (reverse.ContainsKey(item))
        {
            forward.Remove(reverse[item]);
            reverse.Remove(item);
            return true;
        }
        return false;
    }
}