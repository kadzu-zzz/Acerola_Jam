using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public Dictionary<Coord, GameTile> tiles = new();
    public HashSet<Coord> valid_tiles = new();

    List<Colony> colonies = new();
    Colony player;

    public PolygonCollider2D eye_zone;

    public TMPro.TextMeshProUGUI title_text, condition_text;

    public static int level = 0;
    GameLevel current;

    float win_check = 1.0f;

    void Start()
    {
        current = GameLevels.GetLevel(level);
        current.Setup(this);

        title_text.SetText(current.name);

        foreach (Colony c in colonies)
        {
            tiles[c.core].AddCore(c);
            if (c.player_colony)
            {
                eye_zone.transform.position = c.core.ConvertToGrid();
                c.eye_bounds = eye_zone;
            }
        }

        current.CheckVictory(this);
    }

    public void AddTile(Coord tile)
    {
        tile = tile.Simplify();
        valid_tiles.Add(tile);
        if(!tiles.ContainsKey(tile))
            tiles.Add(tile, GameTile.Empty(tile));
    }

    public void AddPlayerColony(Coord tile)
    {
        tile = tile.Simplify();
        colonies.Add(new Colony().Setup(this, tile, true));
        player = colonies.Last();
    }

    public void AddEnemyColony(Coord tile, bool type)
    {
        tile = tile.Simplify();
        colonies.Add(new Colony().Setup(this, tile, false));
    }


    public Colony Player()
    {
        return player;
    }


    void Update()
    {
        foreach(var c in colonies)
        {
            c.Update();
            c.Render();
        }    

        foreach(var t in tiles.Values)
        {
            t.DamageOverlapCells();
        }

        win_check -= Time.deltaTime;
        if(win_check < 0.0f)
        {
            win_check += 1.0f;
            if(current.CheckVictory(this))
            {
                Debug.Log("Winnar");
            }
        }
    }

    public void AddCell(ColonyCell c)
    {
        tiles[c.coord].AddCell(c);
    }

    public void RemoveCell(ColonyCell c)
    {
        tiles[c.coord].RemoveCell(c);
    }
}
