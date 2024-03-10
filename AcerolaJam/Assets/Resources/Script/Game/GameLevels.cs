

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameLevels
{
    public static GameLevel GetLevel(int level)
    {
        Material mat_food = Resources.Load<UnityEngine.Material>("Material/FoodMaterial");

        switch (level)
        {
            case 0:
                return new GameLevel(
                    "Genesis",
                    (GameMap map) => {

                        GameLevel.CreateColony(new Vector2(-100, 0), 5, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            0.01f, false, false, () => { }));

                        GameLevel.CreatePetriDishBorder(250.0f, 32);

                        GameLevel.CreateFoodHazard(new Vector2(100.0f, 0.0f),
                            GameLevel.CreateCircle(50, 16),
                            HazardComponent.CreateFood(60, -1), mat_food);
                    },
                    (GameMap map) => {
                        
                        map.condition_text.SetText("Grow " + map.Player().cells + " / " + 50);

                        return map.Player().cells >= 50;
                    }
                    );

            case 1:
                return new GameLevel(
                    "Scorch",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, -150), 15, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, false, () => { }));

                        GameLevel.CreateColony(new Vector2(0, 100), 100, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => { }));

                        GameLevel.CreatePetriDishBorder(250.0f, 32);


                        GameLevel.CreateHazard(new Vector2(50, 0),
                            new List<Vector2> { new Vector2(-25, -25), new Vector2(-25, 25), new Vector2(80, 80), new Vector2(80, -80), new Vector2(-80, -80) },
                            HazardComponent.CreateFire(0.01f), Resources.Load<UnityEngine.Material>("Material/BurningMaterial"));

                        GameLevel.CreateHazard(new Vector2(0, -100.0f),
                            new List<Vector2> { new Vector2(-50, -50), new Vector2(-50, 50), new Vector2(50, 50), new Vector2(50, -50) },
                             HazardComponent.CreateUV(0.5f), Resources.Load<UnityEngine.Material>("Material/UVMaterial"));

                        GameLevel.CreateHazard(new Vector2(-100.0f, -100.0f),
                            new List<Vector2> { new Vector2(-50, -50), new Vector2(-50, 50), new Vector2(50, 50), new Vector2(50, -50) },
                            HazardComponent.CreateImpulse(new float2(10, -30)), Resources.Load<UnityEngine.Material>("Material/PushMaterial"));

                        GameLevel.CreateFoodHazard(new Vector2(-100.0f, 0.0f),
                            new List<Vector2> { new Vector2(-50, -50), new Vector2(-50, 50), new Vector2(50, 50), new Vector2(50, -50) },
                            HazardComponent.CreateFood(100, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
            case 2:
                return new GameLevel(
                    "Devour",
                    (GameMap map) => {
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
            case 3:
                return new GameLevel(
                    "Shade",
                    (GameMap map) => {
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
            case 4:
                return new GameLevel(
                    "Consume",
                    (GameMap map) => {
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
            case 5:
                return new GameLevel(
                    "Dominion",
                    (GameMap map) => {
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
            case 6:
                return new GameLevel(
                    "Breach",
                    (GameMap map) => {
                    },
                    (GameMap map) => {
                        return false;
                    }
                    );
        }
        return null;
    }
}