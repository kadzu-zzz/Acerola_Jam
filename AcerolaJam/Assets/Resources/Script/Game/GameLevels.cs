

public class GameLevels
{
    public static GameLevel GetLevel(int level)
    {
        switch(level)
        {
            case 0:
                return new GameLevel(
                    "Genesis",
                    (GameMap map) => {

                        for (int r = -1; r < 2; r++)
                        {
                            for (int q = -1; q < 2; q++)
                            {
                                for (int l = -1; l < 2; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Grow " + map.Player().Size() + " / " + (map.tiles.Count - 1 ));

                        return map.Player().Size() >= (map.tiles.Count - 1);
                    }
                    );

            case 1:
                return new GameLevel(
                    "Scorch",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
            case 2:
                return new GameLevel(
                    "Devour",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
            case 3:
                return new GameLevel(
                    "Shade",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
            case 4:
                return new GameLevel(
                    "Consume",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
            case 5:
                return new GameLevel(
                    "Dominion",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
            case 6:
                return new GameLevel(
                    "Breach",
                    (GameMap map) => {

                        for (int r = -2; r < 3; r++)
                        {
                            for (int q = -2; q < 3; q++)
                            {
                                for (int l = -2; l < 3; l++)
                                {
                                    map.AddTile(new Coord(r, q, l));
                                }
                            }
                        }

                        map.AddPlayerColony(Coord.Zero());
                    },
                    (GameMap map) => {
                        return map.Player().Size() > 25;
                    }
                    );
        }
        return null;
    }
}