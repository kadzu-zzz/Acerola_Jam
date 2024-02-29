using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTest : MonoBehaviour
{
    HashSet<Coord> spawned = new();

    public GameObject prefab;

    private void Awake()
    {
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 point = ray.origin + (ray.direction * 120.0f);

            Vector2 pos = new Vector2(point.x, point.y);
            var coord = Coord.FromGrid(pos.x, pos.y);

            Debug.Log(coord);


            if(!spawned.Contains(coord.Simplify()))
            {
                spawned.Add(coord);
                Instantiate(prefab, coord.ConvertToGrid(), prefab.transform.rotation);
            }
        }
    }

    
}
