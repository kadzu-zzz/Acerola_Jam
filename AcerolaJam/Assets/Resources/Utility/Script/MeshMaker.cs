using UnityEngine;

public class MeshMaker
{
    public static Mesh Create(float width, float height)
    {
        Mesh mesh = new Mesh();

        float w2 = width / 2.0f;
        float h2 = height / 2.0f;

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-w2, -h2, 0),
            new Vector3(w2, -h2, 0),
            new Vector3(-w2, h2, 0),
            new Vector3(w2, h2, 0)
        };

        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -new Vector3(0, 1, 0),
            -new Vector3(0, 1, 0),
            -new Vector3(0, 1, 0),
            -new Vector3(0, 1, 0)
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }
}