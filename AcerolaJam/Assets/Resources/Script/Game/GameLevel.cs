using System;

public class GameLevel
{
    public readonly string name;
    Action<GameMap> setup;
    Func<GameMap, bool> win_check;

    public GameLevel(string name, Action<GameMap> setup, Func<GameMap, bool> win_check)
    {
        this.name = name;
        this.setup = setup;
        this.win_check = win_check;
    }

    public void Setup(GameMap map)
    {
        setup(map);
    }

    public bool CheckVictory(GameMap map)
    {
        return win_check(map);
    }
}