using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile 
{
    public Coord coord;
    Colony colony = null;
    HashSet<ColonyCell> cells = new HashSet<ColonyCell>();

    public static GameTile Empty(Coord c)
    {
        return new GameTile().Setup(c);
    }
    public GameTile Setup(Coord coord)
    {
        this.coord = coord;
        return this;
    }

    public void DamageOverlapCells()
    {
        if(cells.Count > 0)
        {
            List<Tuple<ColonyCell, ColonyCell >> interactions = new();
            foreach(ColonyCell cell in cells)
            {
                foreach(ColonyCell other in cells)
                {
                    if (cell == other)
                        continue;
                    interactions.Add(new(cell, other));
                }
            }
            foreach(var pair in interactions)
            {
                pair.Item1.Damage(pair.Item2);
            }
        }
    }

    public void AddCore(Colony c)
    {
        colony = c;
    }

    public void AddCell(ColonyCell c)
    {
        cells.Add(c);
    }

    public void RemoveCell(ColonyCell c)
    {
        cells.Remove(c);
    }
}
