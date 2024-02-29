using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public PolygonCollider2D polygonCollider = null;
    public float speed = 100.0f;

    void Update()
    {
        Vector2 delta = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            delta.y += speed;
        }
        if(Input.GetKey(KeyCode.S))
        {
            delta.y -= speed;
        }
        if(Input.GetKey(KeyCode.A))
        {
            delta.x -= speed;
        } 
        if(Input.GetKey(KeyCode.D))
        {
            delta.x += speed;
        }

        transform.position = transform.position + (new Vector3(delta.x, delta.y, 0) * Time.deltaTime);

        if(!polygonCollider.OverlapPoint(transform.position)) {
            transform.position = polygonCollider.ClosestPoint(transform.position);
        }
    }
}
