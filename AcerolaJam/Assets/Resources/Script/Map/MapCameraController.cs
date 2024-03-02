using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera virtual_camera;
    public PolygonCollider2D polygonCollider = null;
    public float speed = 100.0f;

    public float min_zoom = 150;
    public float max_zoom = 250;

    private void Start()
    {

    }

    void Update()
    {
        float scroll = 0.0f;
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
        scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            CinemachineComponentBase componentBase = virtual_camera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = Mathf.Max(Mathf.Min(max_zoom, (75000 * -scroll * Time.deltaTime) + (componentBase as CinemachineFramingTransposer).m_CameraDistance), min_zoom); 
            }
        }

        transform.position = transform.position + (new Vector3(delta.x, delta.y, 0) * Time.deltaTime);

        if(!polygonCollider.OverlapPoint(transform.position)) {
            transform.position = polygonCollider.ClosestPoint(transform.position);
        }
    }
}
