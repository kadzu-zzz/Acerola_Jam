using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord
{
    [SerializeField]
    private int h;
    [SerializeField]
    private int r;
    [SerializeField]
    private int l;

    public static Coord[] neighbours =
                                          { new Coord(1, 0, 0),
                                            new Coord(-1, 0, 0),
                                            new Coord(0, 1, 0),
                                            new Coord(0, -1, 0),
                                            new Coord(0, 0, 1),
                                            new Coord(0, 0, -1)
                                          };


    public static Coord Zero()
    {
        return new Coord(0, 0, 0);
    }

    public int H
    {
        get { return h; }
    }

    public int R
    {
        get { return r; }
    }

    public int L
    {
        get { return l; }
    }

    public Coord(int h, int r, int l)
    {
        this.h = h;
        this.r = r;
        this.l = l;
    }

    public void setH(int H)
    {
        this.h = H;
    }

    public void setR(int R)
    {
        this.r = R;
    }
    public void setL(int L)
    {
        this.l = L;
    }

    public Vector3 ConvertToGrid()
    {
        Vector3 grid = Vector3.zero;

        grid.z = 20;
        grid.y = ((l + r)) * (Hex.outerRadius * 1.5f);
        grid.x = (h + ((l - r) / 2f)) * (Hex.innerRadius * 2f);
        

        return grid;
    }

    public static Coord FromGrid(float x, float y)
    {
        Coord output = Coord.Zero();

        output.l = Mathf.RoundToInt(y / (Hex.outerRadius * 1.5f));
        output.h = Mathf.RoundToInt(((x / (Hex.innerRadius * 2f)) - (output.l / 2f)));
        output.r = 0;

        return output;
    }

    public bool isNeighbour(Coord coord)
    {
        foreach (var v in neighbours)
        {
            if ((coord + v).Simplify() == this)
                return true;
        }
        return false;
    }

    public Coord Simplify()
    {
        if (r != 0)
        {
            l -= r;
            h += r;
            r = 0;
        }
        return this;
    }

    public override string ToString()
    {
        return "(" + h.ToString() + ", " + r.ToString() + ", " + l.ToString() + ")";
    }

    public override bool Equals(object obj)
    {
        if (obj is (Coord))
            return (Coord)obj == this;
        return false;
    }

    public override int GetHashCode()
    {
        return h + r * 547 + l * 569;
    }

    public static bool operator ==(Coord a, Coord b)
    {
        return a.h == b.h && a.l == b.l && a.r == b.r;
    }
    public static bool operator !=(Coord a, Coord b)
    {
        return a.h != b.h || a.l != b.l || a.r != b.r;
    }
    public static Coord operator +(Coord a, Coord b)
    {
        return new Coord(a.h + b.h, a.r + b.r, a.l + b.l);
    }

    public static Coord operator -(Coord a, Coord b)
    {
        return new Coord(a.h - b.h, a.r - b.r, a.l - b.l);
    }
}
