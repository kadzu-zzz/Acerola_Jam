

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

                        map.SetLevelCameraView(new Vector3(-260, -260, 0), new Vector3(260, 260, 0));

                        GameLevel.CreateFoodHazard(new Vector2(100.0f, 0.0f),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(20, -1), mat_food);
                        GameLevel.CreateFoodHazard(new Vector2(0, 100.0f),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(20, -1), mat_food);
                        GameLevel.CreateFoodHazard(new Vector2(0.0f, -100.0f),
                            GameLevel.CreateCircle(30, 20),
                            HazardComponent.CreateFood(60, -1), mat_food);
                    },
                    (GameMap map) => {
                        
                        map.condition_text.SetText("Grow " + math.max(0, map.Player().cells - 5) + " / " + 60);

                        return map.Player().cells >= 65;
                    },
                    "Biomass Expanded"
                    );

            case 1:
                return new GameLevel(
                    "Scorch",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 300), 40, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, false, () => { }));

                        GameLevel.CreatePetriDishBorder(400.0f, 32);

                        map.SetLevelCameraView(new Vector3(-410, -410, 0), new Vector3(410, 410, 0));

                        GameLevel.CreateHazard(new Vector2(0, 0),
                            GameLevel.CreateCircle(100, 32),
                            HazardComponent.CreateFire(0.01f), Resources.Load<UnityEngine.Material>("Material/BurningMaterial"));

                        GameLevel.CreateFoodHazard(new Vector2(-200.0f, -200.0f),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(100, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(-200.0f, 200.0f),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(100, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(-300.0f, 0.0f),
                            GameLevel.CreateCircle(60, 32),
                            HazardComponent.CreateFood(200, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Expand " + math.max(0, map.Player().cells - 40) + " / " + 250);

                        return map.Player().cells >= 290;
                    },
                    "Expansion Unhindered"
                    );
            case 2:
                return new GameLevel(
                    "Devour",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, -200), 15, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, false, () => { }));

                        GameLevel.CreateColony(new Vector2(0, 200), 50, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => { }));

                        GameLevel.CreatePetriDishBorder(300.0f, 32);

                        map.SetLevelCameraView(new Vector3(-310, -310, 0), new Vector3(310, 310, 0));

                        GameLevel.CreateFoodHazard(new Vector2(0,0),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(80, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Consume Triangle");
                        return !map.HasCore(2);
                    },
                    "Feeding Complete"
                    );
            case 3:
                return new GameLevel(
                    "Shade",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(-100, -200), 50, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, false, () => { }));

                        GameLevel.CreateColony(new Vector2(100, -200), 30, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.uv_immunity = true;
                                map.UpdatePlayer(p);
                            }));

                        GameLevel.CreatePetriDishBorder(300.0f, 32);

                        map.SetLevelCameraView(new Vector3(-310, -310, 0), new Vector3(310, 310, 0));

                        GameLevel.CreateHazard(new Vector2(0, 0),
                            new List<Vector2>{ 
                                new Vector2(math.sin(85 * math.TORADIANS) * 300, math.cos(85 * math.TORADIANS) * 300),
                                new Vector2(math.sin(90 * math.TORADIANS) * 300, math.cos(90 * math.TORADIANS) * 300),
                                new Vector2(math.sin(95 * math.TORADIANS) * 300, math.cos(95 * math.TORADIANS) * 300),
                                new Vector2(math.sin(265 * math.TORADIANS) * 300, math.cos(265 * math.TORADIANS) * 300),
                                new Vector2(math.sin(270 * math.TORADIANS) * 300, math.cos(270 * math.TORADIANS) * 300),
                                new Vector2(math.sin(275 * math.TORADIANS) * 300, math.cos(275 * math.TORADIANS) * 300),
                            },
                            HazardComponent.CreateUV(5.5f), Resources.Load<UnityEngine.Material>("Material/UVMaterial"));

                        GameLevel.CreateFoodHazard(new Vector2(-100, 100),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(60, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(100, 100),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(60, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(0, 100),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(200, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Expand " + math.max(0, map.Player().cells - 50) + " / " + 250);

                        return map.Player().cells >= 300;
                    },
                    "Resistance Assimilated"
                    );
            case 4:
                return new GameLevel(
                    "Consume",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, -200), 15, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, false, () => { }));

                        GameLevel.CreateColony(new Vector2(math.sin(0) * 250, math.cos(0) * 250), 30, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.uv_immunity = true;
                                map.UpdatePlayer(p);
                            }));
                        GameLevel.CreateColony(new Vector2(math.sin(90 * math.TORADIANS) * 250, math.cos(90 * math.TORADIANS) * 250), 30, new Vector2(10.0f, 10.0f), ColonyType.FIRE_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.fire_immunity = true;
                                map.UpdatePlayer(p);
                            }));
                        GameLevel.CreatePetriDishBorder(400.0f, 32);

                        map.SetLevelCameraView(new Vector3(-410, -410, 0), new Vector3(410, 410, 0));

                        GameLevel.CreateHazard(new Vector2(0, 0),
                            new List<Vector2>{
                                GameLevel.AngleOffset(0, 30),
                                GameLevel.AngleOffset(325, 400),
                                GameLevel.AngleOffset(330, 400),
                                GameLevel.AngleOffset(335, 400),
                                GameLevel.AngleOffset(0, 90)
                            },
                            HazardComponent.CreateUV(0.5f), Resources.Load<UnityEngine.Material>("Material/UVMaterial"));
                        GameLevel.CreateHazard(new Vector2(0, 0),
                            new List<Vector2>{
                                GameLevel.AngleOffset(35, 400),
                                GameLevel.AngleOffset(40, 400),
                                GameLevel.AngleOffset(45, 400),
                                GameLevel.AngleOffset(0, 30),
                                GameLevel.AngleOffset(0, 90)
                            },
                            HazardComponent.CreateUV(0.5f), Resources.Load<UnityEngine.Material>("Material/UVMaterial"));

                        GameLevel.CreateHazard(new Vector2(0, 0),
                            new List<Vector2>{
                                GameLevel.AngleOffset(135, 400),
                                GameLevel.AngleOffset(140, 400),
                                GameLevel.AngleOffset(145, 400),
                                GameLevel.AngleOffset(0, 30),
                                GameLevel.AngleOffset(0, 30) + GameLevel.AngleOffset(45, 60)
                            },
                            HazardComponent.CreateFire(0.025f), Resources.Load<UnityEngine.Material>("Material/BurningMaterial"));

                        GameLevel.CreateFoodHazard(new Vector2(math.sin(240 * math.TORADIANS) * 250, math.cos(240 * math.TORADIANS) * 250),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(40, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(math.sin(15 * math.TORADIANS) * 350, math.cos(15 * math.TORADIANS) * 250),
                            GameLevel.CreateCircle(24, 16),
                            HazardComponent.CreateFood(30, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Consume Triangle & Square");
                        return (!map.HasCore(2) && !map.HasCore(3));
                    },
                    "Dominance Established"
                    );
            case 5:
                return new GameLevel(
                    "Hunger",
                    (GameMap map) => {
                        GameLevel.CreateColony(GameLevel.AngleOffset(180, 300), 25, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 15, 100,
                            1.0f, false, false, () => { }));
                        GameLevel.CreateColony(GameLevel.AngleOffset(90, 300), 30, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.uv_immunity = true;
                                p.cohesion += 5;
                                p.speed += 25;
                                map.UpdatePlayer(p);
                            }));
                        GameLevel.CreateColony(GameLevel.AngleOffset(270, 500), 60, new Vector2(10.0f, 10.0f), ColonyType.UV_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.uv_immunity = true;
                                p.cohesion += 5;
                                p.speed += 25;
                                map.UpdatePlayer(p);
                            }));
                        GameLevel.CreateColony(GameLevel.AngleOffset(270, 250), 70, new Vector2(10.0f, 10.0f), ColonyType.FIRE_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.fire_immunity = true;
                                map.UpdatePlayer(p);
                            }));
                        GameLevel.CreateColony(Vector2.zero, 150, new Vector2(10.0f, 10.0f), ColonyType.FIRE_RES, CoreData.Create(
                            250, 50, 5, 40,
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.fire_immunity = true;
                                map.UpdatePlayer(p);
                            }));

                        GameLevel.CreatePetriDishBorder(600.0f, 64);
                        map.SetLevelCameraView(new Vector3(-610, -610, 0), new Vector3(610, 610, 0));


                        GameLevel.CreateHazard(new Vector2(0, 0),
                            GameLevel.CreateCircle(160, 32),
                            HazardComponent.CreateFire(0.01f), Resources.Load<UnityEngine.Material>("Material/BurningMaterial"));

                        GameLevel.CreateFoodHazard(GameLevel.AngleOffset(0, 300),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(50, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(GameLevel.AngleOffset(300, 500),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(75, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(GameLevel.AngleOffset(220, 500),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(75, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(GameLevel.AngleOffset(150, 500),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(75, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(GameLevel.AngleOffset(30, 500),
                            GameLevel.CreateCircle(40, 16),
                            HazardComponent.CreateFood(75, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));

                        for (float f = 0; f < 360.0f; f += 10.0f)
                        {
                            GameLevel.CreateHazard(new Vector2(0, 0),
                                new List<Vector2>{
                                GameLevel.AngleOffset(f, 420),
                                GameLevel.AngleOffset(f + 10, 420),
                                GameLevel.AngleOffset(f + 10, 600),
                                GameLevel.AngleOffset(f, 600),
                                },
                                HazardComponent.CreateUV(0.5f), Resources.Load<UnityEngine.Material>("Material/UVMaterial"), false);
                        }

                        for (float f = 200; f < 360.0f; f += 10.0f)
                        {
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
                                GameLevel.AngleOffset(f, 400),
                                GameLevel.AngleOffset(f + 10, 400),
                                GameLevel.AngleOffset(f + 10, 430),
                                GameLevel.AngleOffset(f, 430),
                            }, Resources.Load<UnityEngine.Material>("Material/BlockingMat"), true, false);
                        }
                        for (float f = 30; f < 170.0f; f += 10.0f)
                        {
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
                                GameLevel.AngleOffset(f, 400),
                                GameLevel.AngleOffset(f + 10, 400),
                                GameLevel.AngleOffset(f + 10, 430),
                                GameLevel.AngleOffset(f, 430),
                            }, Resources.Load<UnityEngine.Material>("Material/BlockingMat"), true, false);
                        }
                        for (float f = 20; f < 350.0f; f += 10.0f)
                        {
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
                                GameLevel.AngleOffset(f, 155),
                                GameLevel.AngleOffset(f + 10, 155),
                                GameLevel.AngleOffset(f + 10, 185),
                                GameLevel.AngleOffset(f, 185),
                            }, Resources.Load<UnityEngine.Material>("Material/BlockingMat"), true, false);
                        }
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Devour All Of Them");
                        return (!map.HasCore(2) && !map.HasCore(3) &&
                                !map.HasCore(4) && !map.HasCore(5));
                    },
                    "Feast Concluded"
                    );
            case 6:
                return new GameLevel(
                    "Breach",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 0), 10, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            450, 50, 5, 40,
                            1.0f, false, false, () => { }));
                        GameLevel.CreatePetriDishBorder(100.0f, 32);

                        map.SetLevelCameraView(new Vector3(-110, -110, 0), new Vector3(110, 110, 0));

                        GameLevel.CreateFoodHazard(new Vector2(0, -80),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(250, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(0, 80),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(250, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(80, 0),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(250, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(-80, 0),
                            GameLevel.CreateCircle(20, 16),
                            HazardComponent.CreateFood(250, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("GROW QUICKLY " + math.max(0, map.Player().cells - 10) + " / " + 800);

                        if(map.Player().cells >= 810)
                        {
                            //Destroy walls?
                            return true;
                        }
                        return false;
                    },
                    "Containment Breached"
                    );
        }
        return null;
    }
}