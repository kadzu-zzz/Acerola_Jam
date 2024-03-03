using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MainMenuEyeFollow : MonoBehaviour
{
    PolygonCollider2D eye_bounds;
    Vector2 center = Vector3.zero;

    Material eye_material;
    Mesh render_plane;

    void Start()
    {
        eye_bounds = GetComponent<PolygonCollider2D>();
        foreach(var c in eye_bounds.points)
        {
            center += transform.TransformPoint(new Vector3(c.x, c.y, 0.0f)).XY();
        }
        center /= eye_bounds.points.Length;
        center -= GetComponent<RectTransform>().position.XY() * new Vector2(1, 2);
        render_plane = MeshMaker.Create(1.0f, 1.0f);
        eye_material = Resources.Load<Material>("Material/EyeMaterial");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = ray.direction.normalized;
        float t = (ray.origin.z) / dir.z;
        Vector3 point = ray.origin + (dir * t);
        Vector2 eyeLoc = Vector3.MoveTowards(center, point, (point - new Vector3(center.x, center.y, 0.0f)).magnitude / 100.0f).XY();
        if (!eye_bounds.OverlapPoint(eyeLoc))
        { 
           eyeLoc = eye_bounds.ClosestPoint(eyeLoc);
        }
        var p = new RenderParams(eye_material);
        Graphics.RenderMesh(p, render_plane, 0, Matrix4x4.TRS(new Vector3(eyeLoc.x, eyeLoc.y, -0.20F), Quaternion.identity, Vector3.one * 1.1f));

    }
}
