
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class QuadTreeNode<T> 
{
    public Vector2 pos;
    public T data;

    public QuadTreeNode(Vector2 pos, T data)
    {
        this.pos = pos;
        this.data = data;
    }
}

public class QuadTree<T>
{
    AABB boundry;
    List<QuadTreeNode<T>> nodes;
    QuadTree<T> NE, SE, SW, NW;
    const int QuadTreeCapacity = 2;

    public QuadTree(Vector2 pos, float half_size)
    {
        boundry = new(pos.x, pos.y, half_size);
        nodes = new(QuadTreeCapacity);
        NE = null;
        SE = null;
        SW = null;
        NW = null;
    }
        
    public QuadTreeNode<T> Insert(Vector2 pos, T data)
    {
        if (!boundry.IsInside(pos))
            return null;

        if(nodes.Count < QuadTreeCapacity)
        {
            nodes.Add(new(pos, data));
            return nodes[nodes.Count - 1];
        }

        if (NW == null)
            Subdivide();

        QuadTreeNode<T> node = NW.Insert(pos, data);
        if (node != null)
            return node;
        node = NE.Insert(pos, data);
        if (node != null)
            return node;
        node = SE.Insert(pos, data);
        if (node != null)
            return node;
        node = SW.Insert(pos, data);
        if (node != null)
            return node;
        return null;
    }

    public void Remove(Vector2 pos)
    {
        if(!boundry.IsInside(pos))
        {
            return;
        }

        for (int i = 0; i < Mathf.Min(nodes.Count, QuadTreeCapacity); i++)
        {
            if (nodes[i].pos == pos)
            {
                nodes.RemoveAt(i);
                return;
            }
        }

        if (NW == null)
            return;

        NW.Remove(pos);
        NE.Remove(pos);
        SE.Remove(pos);
        SW.Remove(pos);
    }

    public void Remove(AABB bounds)
    {
        if (!boundry.Intersects(bounds) && !bounds.IsCompletlyInside(boundry))
        {
            return;
        }

        for (int i = 0; i < Mathf.Min(nodes.Count, QuadTreeCapacity); i++)
        {
            if (bounds.IsInside(nodes[i].pos))
            {
                nodes.RemoveAt(i);
                i--;
            }
        }

        if (NW == null)
            return;

        NW.Remove(bounds);
        NE.Remove(bounds);
        SE.Remove(bounds);
        SW.Remove(bounds);
    }

    public List<QuadTreeNode<T>> Query(AABB bounds, ref List<QuadTreeNode<T>> result)
    {
        if (!boundry.Intersects(bounds) && !bounds.IsCompletlyInside(boundry))
            return result;

        for(int i = 0; i < Mathf.Min(nodes.Count, QuadTreeCapacity); i++)
        {
            if (bounds.IsInside(nodes[i].pos))
            {
                result.Add(nodes[i]);
            }
        }

        if (NW == null)
            return result;

        NW.Query(bounds, ref result);
        NE.Query(bounds, ref result);
        SE.Query(bounds, ref result);
        SW.Query(bounds, ref result);

        return result;
    }

    public List<QuadTreeNode<T>> QueryAll()
    {
        var d = new List<QuadTreeNode<T>>();
        return Query(boundry, ref d);
    }

    public void Clear()
    {
        nodes.Clear();

        if (NW == null)
            return;

        NW.Clear();
        NE.Clear();
        SE.Clear();
        SW.Clear();
    }

    protected void Subdivide()
    {
        float half_half = boundry.halfWidth / 2.0f;
        NW = new QuadTree<T>(new Vector2(boundry.x + half_half, boundry.y + half_half), half_half);
        NE = new QuadTree<T>(new Vector2(boundry.x - half_half, boundry.y + half_half), half_half);
        SW = new QuadTree<T>(new Vector2(boundry.x + half_half, boundry.y - half_half), half_half);
        SE = new QuadTree<T>(new Vector2(boundry.x - half_half, boundry.y - half_half), half_half);
    }

}