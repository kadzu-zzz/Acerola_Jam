using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOnBounds : MonoBehaviour
{
    public BoxCollider2D bounds;
    new CinemachineVirtualCamera camera;
    void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();

        Vector3 min = bounds.bounds.min;
        Vector3 max = bounds.bounds.max;
        Vector3 center = (min + max) / 2;

        float boundsWidth = max.x - min.x;
        float halfFOV = (camera.m_Lens.FieldOfView / 2) * Mathf.Deg2Rad;
        float distance = boundsWidth / (2 * Mathf.Tan(halfFOV));

        Vector3 direction = (camera.transform.position - center).normalized;
        camera.transform.position = center - direction * -distance;
        camera.transform.LookAt(center);
    }

    
}
