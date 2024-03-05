using UnityEngine;

public class AABB
{
    public readonly float x;
    public readonly float y;
    public readonly float halfWidth;
    public readonly float halfHeight;

    public AABB(float x, float y, float halfDimension) : this(x, y, halfDimension, halfDimension)
    {
    }

    public AABB(float x, float y, float halfWidth, float halfHeight)
    {
        this.x = x;
        this.y = y;
        this.halfWidth = halfWidth; 
        this.halfHeight = halfHeight;
    }

    public Vector2 Min()
    {
        return new(MinX(), MinY());
    }

    public Vector2 Max()
    {
        return new(MaxX(), MaxY());
    }

    public float MinX()
    {
        return x - halfWidth;
    }

    public float MaxX()
    {
        return x + halfWidth;
    }

    public float MinY()
    {
        return y - halfHeight;
    }

    public float MaxY()
    {
        return y + halfHeight;
    }

    public bool IsInside(Vector2 pos)
    {
        return !(pos.x > x + halfWidth ||
            pos.x < x - halfWidth ||
            pos.y < y - halfHeight ||
            pos.y > y + halfHeight);
    }

    public bool Intersects(AABB other)
    {
        return !(other.x - other.halfWidth > x + halfWidth ||
            other.x + other.halfWidth < x - halfWidth ||
            other.y - other.halfHeight > y + halfHeight ||
            other.y + other.halfHeight < y - halfHeight);
    }

    public bool IsCompletlyInside(AABB other)
    {
        return other.x - other.halfWidth < x - halfWidth &&
            other.x + other.halfWidth > x + halfWidth &&
            other.y - other.halfHeight < y - halfHeight &&
            other.y + other.halfHeight > y + halfHeight;
    }
}