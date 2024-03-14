using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
public class PolygonMeshFactory
{
    static List<int> Triangulate(List<Vector2> points)
    {
        List<int> indexes = new();
        for (int i = 0; i < points.Count - 2; i++)
        {
            indexes.Add(0);
            indexes.Add(i + 1);
            indexes.Add(i + 2);
        }
        return indexes;
    }

    public static Mesh Create(Vector2 offset, List<Vector2> points)
    {
        var indices = Triangulate(new List<Vector2>(points));

        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y , 0);
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    public static GameObject CreatePolygonObject(Vector2 offset, List<Vector2> points, Material mat, GameObject obj_in = null)
    {
        var mesh = Create(offset, points);
        GameObject go = obj_in == null ? new GameObject("RuntimePolygon") : obj_in;
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();

        

        meshFilter.mesh = mesh;
        meshRenderer.material = mat;
        return go;
    }
    public static GameObject CreateLineRenderObject(Vector2 offset, List<Vector2> points, Color c, GameObject obj_in = null)
    {
        GameObject go = obj_in == null ? new GameObject("RuntimeLineRenderer") : obj_in;
        LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
       
        if(obj_in != null)
        {
            if(obj_in.GetComponent<MeshRenderer>() != null)
            {
                c = obj_in.GetComponent<MeshRenderer>().material.GetColor("_Colour"); 
            }
        }

        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.sortingOrder = -5;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = false;

        if (points[0] != points[points.Count - 1])
        {
            points.Add(points[0]);
        }

        Vector3[] positions = points.Select(p => new Vector3(p.x , p.y, 0)).ToArray();
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
        

        return go;
    }
}