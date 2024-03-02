using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColonyCell 
{
    public Colony parent;
    public Coord coord;

    public float health, max_health;
    public float strength = 1.0f;
    public float regen = 0.25f;

    float growth_time, growth_timer;
    public float animation_time;
    public Vector3 bonus_pos;
    public ColonyCell Setup(Colony parent, Coord coord, Coord from)
    {
        this.parent = parent;
        this.coord = coord;
        growth_time = 3.25f;
        growth_timer = 0.0f;
        animation_time = Time.timeSinceLevelLoad;
        bonus_pos = from.ConvertToGrid() - coord.ConvertToGrid();
        health = 0.5f;
        max_health = health * 2.0f;
        strength = 1.0f;
        return this;
    }

    public void Grow(float time)
    {
        if(growth_time > growth_timer)
        {
            growth_timer += time;
            if(growth_timer > growth_time)
            {
                parent.CellGrown(coord);
            }
        }
        if(bonus_pos != Vector3.zero)
        {
            bonus_pos = Vector3.MoveTowards(bonus_pos, Vector3.zero, 50.0f * time);
        }
    }

    public void Regen(float time)
    {
        health = Mathf.Min(max_health, health + (time * regen));
    }

    public void Damage(ColonyCell other)
    {
        other.health = Mathf.Max(other.health - (strength * Time.deltaTime), 0);
        if (other.health <= 0.0f)
            other.parent.Kill(other);
    }

    public float GetSizePercentage()
    {
        return Mathf.Min(growth_timer / growth_time, health / max_health);
    }

    public Vector3 GetBonusPos()
    {
        return bonus_pos;
    }
}
