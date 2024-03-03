using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeBounds : MonoBehaviour
{
    public CinemachineVirtualCamera virtual_camera;
    public PolygonCollider2D collision;

    public void Change()
    {
        virtual_camera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = collision;
    }
}
