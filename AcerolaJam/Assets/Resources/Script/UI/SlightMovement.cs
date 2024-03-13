using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class SlightMovement : MonoBehaviour
{
    Vector3 position;

    public float speed;
    public float magnitude;
    float time_start;

    void Start()
    {
        position = transform.position;
        time_start = Time.timeSinceLevelLoad + UnityEngine.Random.Range(0, 100.0f);
    }

    void Update()
    {
        float diff = ((Time.timeSinceLevelLoad - time_start)) * speed;
        transform.position = position + new Vector3(Mathf.Sin(diff + diff), Mathf.Cos(diff * magnitude), 0) * magnitude;
    }

    public void Set(float speed, float magnitude)
    {
        this.speed = speed;
        this.magnitude = magnitude;
    }
}
