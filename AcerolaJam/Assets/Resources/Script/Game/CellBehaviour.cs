using System;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour 
{
    public int id;
    public Vector2 position;
    public float spawn_time;

    public CellBehaviour(int id, Vector2 position)
    {
        this.id = id;
        this.position = position;
        this.spawn_time = Time.time;
    }
}
