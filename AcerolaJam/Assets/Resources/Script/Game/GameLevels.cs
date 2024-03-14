

using System.Collections.Generic;
using System.Threading;
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
                        GameLevel.CreateFoodHazard(new Vector2(200.0f, -200.0f),
                            GameLevel.CreateCircle(30, 16),
                            HazardComponent.CreateFood(100, -1), Resources.Load<UnityEngine.Material>("Material/FoodMaterial"));
                        GameLevel.CreateFoodHazard(new Vector2(0.0f, -300.0f),
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
                            1.0f, false, true, () => {
                                var p = map.Player();
                                p.uv_immunity = true;
                                map.UpdatePlayer(p);
                            }));

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
                                GameLevel.AngleOffset(135, 400),
                                GameLevel.AngleOffset(140, 400),
                                GameLevel.AngleOffset(145, 400),
                                GameLevel.AngleOffset(0, 30),
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
                                GameLevel.AngleOffset(0, 30),
                                GameLevel.AngleOffset(325, 400),
                                GameLevel.AngleOffset(330, 400),
                                GameLevel.AngleOffset(335, 400),
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
                                p.cohesion = 5;
                                p.speed = 40;
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
            case 7:
                return new GameLevel(
                    "Infection",
                    (GameMap map) =>
                    {
                        GameLevel.CreateColony(new Vector2(360, -190), 250, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 50, 15, 100,
                            1.0f, false, false, () => { }));
                        
                        map.SetLevelCameraView(new Vector3(10, -1590, 0), new Vector3(1670, -10, 0));

                        for(int i = 0; i < 50; i++)
                        {
                            GameLevel.CreateWhiteBloodCell(new Vector2(935, -1133) + new Vector2(20,20) * new Vector2(UnityEngine.Random.Range(-1.0f,1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            GameLevel.CreateAdipose(new Vector2(1153, -520) + new Vector2(2, 2) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));
                        }
                        for (int i = 0; i < 800; i++)
                        {
                            GameLevel.CreatePlatelet(new Vector2(700, -505) + new Vector2(200, 150) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(474, -283), new Vector2(970, -310));
                        }

                        GameLevel.CreateHazard(new Vector2(1020, -1435),
                           GameLevel.CreateCircle(100, 32),
                            new HazardComponent { death = 0, fire = 0, food = 0, impulse = float2.zero, max_food = 0, uv = 0 }, Resources.Load<UnityEngine.Material>("Material/GoalMaterial"));

                        GameLevel.CreateHazard(new Vector2(0, 0),
                            new List<Vector2> {
                            new Vector2(626, -1247),
                            new Vector2(600, -1138),
                            new Vector2(582, -1070),
                            new Vector2(525, -739),
                            new Vector2(492, -562),
                            new Vector2(465, -401),
                            new Vector2(468, -305),
                            new Vector2(820, -315),
                            new Vector2(903, -374),
                            new Vector2(962, -512),
                            new Vector2(948, -678),
                            new Vector2(930, -805),
                            new Vector2(870, -1020),
                            new Vector2(831, -1135),
                            new Vector2(731, -1340),
                            new Vector2(685, -1222),
                            },
                            HazardComponent.CreateImpulse(new float2(0, 82f)), Resources.Load<UnityEngine.Material>("Material/PushMaterial"));
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -400),
    new Vector2(-1600,-296),
    new Vector2(468, -263),
    new Vector2(465, -401),

                        }, Resources.Load<UnityEngine.Material>("Material/SkinMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero,
new List<Vector2>
{
    new Vector2(972, -411),
    new Vector2(975, -290),
    new Vector2(1558, -308),
    new Vector2(3200, -315),
    new Vector2(3200, -436),
                        }, Resources.Load<UnityEngine.Material>("Material/SkinMaterial"), true, false); 
                        
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -296),
    new Vector2(-1600, -127),
    new Vector2(370, -108),
    new Vector2(202, -287),

                        }, Resources.Load<UnityEngine.Material>("Material/HazardMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1559, -308),
    new Vector2(1373, -118),
    new Vector2(3200, -120),
    new Vector2(3200, -315),

                        }, Resources.Load<UnityEngine.Material>("Material/HazardMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -127),
    new Vector2(-1600, 0),
    new Vector2(3200, 0),
    new Vector2(3200, -120),
                        }, null, false, false); 
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(948, -678),
    new Vector2(962, -512),
    new Vector2(1011, -568),
    new Vector2(1010, -630),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(500, -1050),
    new Vector2(460, -1000),
    new Vector2(450, -800),
    new Vector2(525, -739),
    new Vector2(582, -1070),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(718, -2000),
    new Vector2(626,-1247),
    new Vector2(685, -1222),
    new Vector2(731, -1340),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -600),
    new Vector2(-1600, -400),
    new Vector2(465, -401),
    new Vector2(492, -562),
    new Vector2(389, -600),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -860),
    new Vector2(-1600, -600),
    new Vector2(389, -600),
    new Vector2(289, -863),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -1138),
    new Vector2(-1600, -860),
    new Vector2(289, -863),
    new Vector2(347, -1112),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(-1600, -2000),
    new Vector2(-1600, -1138),
    new Vector2(347, -1112),
    new Vector2(411, -1140),
    new Vector2(600, -1138),
    new Vector2(626, -1247),
    new Vector2(718, -2000),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);


                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1199, -532),
    new Vector2(1028, -475),
    new Vector2(972, -411),
    new Vector2(3200, -436),
    new Vector2(3200, -570),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1205, -582),
    new Vector2(1199, -532),
    new Vector2(3200, -570),
    new Vector2(3200, -620),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1180, -740),
    new Vector2(1135, -636),
    new Vector2(1135,-602),
    new Vector2(1205, -582),
    new Vector2(3200, -620),
    new Vector2(3200, -800),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(870, -1020),
    new Vector2(930, -805),
    new Vector2(1017, -792),
    new Vector2(1070, -765),
    new Vector2(1180, -740),
    new Vector2(3200, -800),
    new Vector2(3200, -1192),
    new Vector2(910, -1030),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1110, -1409),
    new Vector2(1111, -1320),
    new Vector2(1006, -1103),
    new Vector2(910, -1030),
    new Vector2(3200, -1192),
    new Vector2(3200, -1500),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(936, -2000),
    new Vector2(956, -1486),
    new Vector2(1065, -1500),
    new Vector2(1124, -2000),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(814, -2000),
    new Vector2(880, -1356),
    new Vector2(940, -1380),
    new Vector2(965, -1486),
    new Vector2(936, -2000),

                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(718, -2000),
    new Vector2(731, -1340),
    new Vector2(831, -1135),
    new Vector2(905, -1211),
    new Vector2(880, -1356),
    new Vector2(814, -2000),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                        GameLevel.CreateBlocking(Vector2.zero, new List<Vector2>
                        {
    new Vector2(1124, -2000),
    new Vector2(1065,-1500),
    new Vector2(1110, -1409),
    new Vector2(3200, -1500),
    new Vector2(3200, -2000),
                        }, Resources.Load<UnityEngine.Material>("Material/FleshMaterial"), true, false);
                    },
                    (GameMap map) =>
                    {
                        map.condition_text.SetText("Enter the body as deeply as possible");

                        if (math.distance(map.Player().center, new float2(1020, -1435)) <= 100)
                        {
                            return true;
                        }
                        return false;
                    },
                    "Inflitration Successful");
            case 8:
                return new GameLevel("Claustrophobia",
                    (GameMap map) =>
                    {
                        GameLevel.CreateColony(new Vector2(160, -119), 50, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            250, 30, 15, 100,
                            1.0f, false, false, () => { }));

                        Material flesh = Resources.Load<UnityEngine.Material>("Material/FleshMaterial");
                        float max_x = 3000;
                        float min_x = -1000;
                        float max_y = 1500;
                        float min_y = -1500;

                        map.SetLevelCameraView(new Vector3(10, -300, 0), new Vector3(2010, -10, 0));

                        for (int i = 0; i < 25; i++)
                        {
                            GameLevel.CreatePlatelet(new Vector2(890, -100) + new Vector2(10, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(895, -70), new Vector2(880, -113));
                            GameLevel.CreatePlatelet(new Vector2(1347, -124) + new Vector2(10, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(1352, -109), new Vector2(1350, -136));
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            GameLevel.CreateWhiteBloodCell(new Vector2(613, -113) + 
                                new Vector2(5, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));
                            GameLevel.CreateWhiteBloodCell(new Vector2(1587, -132) +
                                new Vector2(5, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            GameLevel.CreateWhiteBloodCell(new Vector2(974, -122) +
                                new Vector2(25, 25) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));
                        }

                        GameLevel.CreateAdipose(new Vector2(544, -184));
                        GameLevel.CreateAdipose(new Vector2(544, -184));
                        GameLevel.CreateAdipose(new Vector2(495, -45));
                        GameLevel.CreateAdipose(new Vector2(495, -45));
                        GameLevel.CreateAdipose(new Vector2(218, -118));
                        GameLevel.CreateAdipose(new Vector2(817, -94));
                        GameLevel.CreateAdipose(new Vector2(823, -100));
                        GameLevel.CreateAdipose(new Vector2(817, -100));
                        GameLevel.CreateAdipose(new Vector2(1118, -164));
                        GameLevel.CreateAdipose(new Vector2(1118, -164));
                        GameLevel.CreateAdipose(new Vector2(1118, -164));
                        GameLevel.CreateAdipose(new Vector2(1118, -164));


                        GameLevel.CreateHazard(new Vector2(1712, -150),
                           GameLevel.CreateCircle(100, 32),
                            new HazardComponent { death = 0, fire = 0, food = 0, impulse = float2.zero, max_food = 0, uv = 0 }, Resources.Load<UnityEngine.Material>("Material/GoalMaterial"));

                        {
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, min_y),
new Vector2(min_x, max_y),
new Vector2(102, max_y),
new Vector2(102, -103),
new Vector2(119, -156),
new Vector2(119, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(102, -103),
new Vector2(102, max_y),
new Vector2(146, max_y),
new Vector2(146, -73)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(146, -73),
new Vector2(146, max_y),
new Vector2(227, max_y),
new Vector2(227, -98)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(227, -98),
new Vector2(227, max_y),
new Vector2(265, max_y),
new Vector2(265, -69)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(265, -69),
new Vector2(265, max_y),
new Vector2(457, max_y),
new Vector2(457, -13),
new Vector2(363, -36)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(457, -13),
new Vector2(457, max_y),
new Vector2(589, max_y),
new Vector2(589, -32),
new Vector2(539, -29),
new Vector2(496, -17)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(589, -35),
new Vector2(589, max_y),
new Vector2(725, max_y),
new Vector2(725, -60),
new Vector2(645, -45),
new Vector2(613, -45)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(725, -60),
new Vector2(725, max_y),
new Vector2(919, max_y),
new Vector2(919, -43),
new Vector2(895, -70),
new Vector2(816, -74)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(919, -43),
new Vector2(919, max_y),
new Vector2(1025, max_y),
new Vector2(1025, -65)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1025, 6-5),
new Vector2(1025, max_y),
new Vector2(1134, max_y),
new Vector2(1134, -41)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1134, -41),
new Vector2(1134, max_y),
new Vector2(1226, max_y),
new Vector2(1226, -72)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1226, -72),
new Vector2(1226, max_y),
new Vector2(1275, max_y),
new Vector2(1275, -90),
new Vector2(1237, -93)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1275, -90),
new Vector2(1275, max_y),
new Vector2(1352, max_y),
new Vector2(1352, -109),
new Vector2(1302, -107)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1352, -109),
new Vector2(1352, max_y),
new Vector2(1479, max_y),
new Vector2(1479, -112),
new Vector2(1430, -123),
new Vector2(1372, -117)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1479, -112),
new Vector2(1479, max_y),
new Vector2(1600, max_y),
new Vector2(1600, -91)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1600, -91),
new Vector2(1600, max_y),
new Vector2(max_x, max_y),
new Vector2(1739, -106)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1739, -106),
new Vector2(max_x, max_y),
new Vector2(max_x, min_y),
new Vector2(1723, -210)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(max_x, min_y),
new Vector2(1694, min_y),
new Vector2(1694, -193),
new Vector2(1723, -210)
}, flesh, true, false);

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(max_x, min_y),
new Vector2(1694, min_y),
new Vector2(1694, -193),
new Vector2(1723, -210)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1561, min_y),
new Vector2(1694, min_y),
new Vector2(1694, -193),
new Vector2(1561, -194)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1561, min_y),
new Vector2(1444, min_y),
new Vector2(1444, -173),
new Vector2(1561, -194)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1405, min_y),
new Vector2(1444, min_y),
new Vector2(1444, -173),
new Vector2(1405, -144)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1278, min_y),
new Vector2(1405, min_y),
new Vector2(1405, -144),
new Vector2(1278, -127)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1230, min_y),
new Vector2(1278, min_y),
new Vector2(1278, -127),
new Vector2(1230, -191)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1197, min_y),
new Vector2(1230, min_y),
new Vector2(1230, -191),
new Vector2(1197, -206)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1030, min_y),
new Vector2(1230, min_y),
new Vector2(1230, -206),
new Vector2(1030, -180)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(975, min_y),
new Vector2(1030, min_y),
new Vector2(1030, -180),
new Vector2(975, -185)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(913, min_y),
new Vector2(975, min_y),
new Vector2(975, -185),
new Vector2(913, -160)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(885, min_y),
new Vector2(913, min_y),
new Vector2(913, -160),
new Vector2(885, -137)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(823, min_y),
new Vector2(885, min_y),
new Vector2(885, -137),
new Vector2(880, -113),
new Vector2(823, -137)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(692, min_y),
new Vector2(823, min_y),
new Vector2(823, -137),
new Vector2(692, -182)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(613, min_y),
new Vector2(692, min_y),
new Vector2(692, -182),
new Vector2(613, -177)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(546, min_y),
new Vector2(613, min_y),
new Vector2(613, -177),
new Vector2(546, -203)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(470, min_y),
new Vector2(546, min_y),
new Vector2(546, -203),
new Vector2(470, -178)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(360, min_y),
new Vector2(470, min_y),
new Vector2(470, -178),
new Vector2(360, -170)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(360, min_y),
new Vector2(267, min_y),
new Vector2(267, -164),
new Vector2(360, -170)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(267, min_y),
new Vector2(191, min_y),
new Vector2(191, -151),
new Vector2(267, -164)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(119, min_y),
new Vector2(191, min_y),
new Vector2(191, -151),
new Vector2(119, -156)
}, flesh, true, false);

                            //Islands

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(239, -117),
new Vector2(308, -78),
new Vector2(370, -117),
new Vector2(266, -144)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(266, -144),
new Vector2(370, -117),
new Vector2(442, -118),
new Vector2(360, -151)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(330, -70),
new Vector2(439, -55),
new Vector2(598, -76),
new Vector2(608, -93),
new Vector2(467, -96),
new Vector2(357, -84)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(404, -157),
new Vector2(508, -125),
new Vector2(480, -157)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(480, -157),
new Vector2(508, -125),
new Vector2(606, -118),
new Vector2(609, -155),
new Vector2(540, -170)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(609, -155),
new Vector2(606, -118),
new Vector2(697, -156)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(630, -107),
new Vector2(657, -88),
new Vector2(708, -100),
new Vector2(717, -143)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(717, -143),
new Vector2(708, -100),
new Vector2(762, -100),
new Vector2(789, -125),
new Vector2(760, -141)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1085, -162),
new Vector2(1101, -125),
new Vector2(1140, -140),
new Vector2(1116, -160)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1085, -86),
new Vector2(1116, -82),
new Vector2(1202, -100),
new Vector2(1241, -126),
new Vector2(1140, -140),
new Vector2(1101, -125)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1436, -140),
new Vector2(1528, -127),
new Vector2(1563, -167),
new Vector2(1486, -160)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(1528, -127),
new Vector2(1573, -130),
new Vector2(1582, -176),
new Vector2(1563, -167)
}, flesh, true, false);
                        }
                    },
                    (GameMap map) =>
                    {
                        map.condition_text.SetText("Worm your way deeper while mainting Biomass (" + math.min(map.Player().cells, 50) + " / 50)");

                        if (math.distance(map.Player().center, new float2(1712, -150)) <= 100)
                        {
                            if(map.Player().cells >= 50)
                                return true;
                        }
                        return false;
                    },
                    "Corruption Spreads");
            case 9:
                return new GameLevel(
                    "Conduit",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 0), 10, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            450, 50, 5, 40,
                            1.0f, false, false, () => { }));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Was supposed to involve high speed movement modifers acting like ateries, and a constant attack of white blood cells");
                        return true;
                    },
                    "Heartbound"
                    );
            case 10:
                return new GameLevel(
                    "Resistance",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 0), 10, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            450, 50, 5, 40,
                            1.0f, false, false, () => { }));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Was to introduce an antibiotic mechanic, a kind of super white blood cell");
                        return true;
                    },
                    "Medicine Ineffective"
                    );
            case 11:
                return new GameLevel(
                    "Chambered",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 0), 10, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            450, 50, 5, 40,
                            1.0f, false, false, () => { }));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Powerful currents move you around in a peroidic motion, you need to devour several colonies at certain spots");
                        return true;
                    },
                    "Eat your heart out"
                    );
            case 12:
                return new GameLevel(
                    "Barrier",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(0, 0), 10, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            450, 50, 5, 40,
                            1.0f, false, false, () => { }));
                    },
                    (GameMap map) => {
                        map.condition_text.SetText("Representing the blood brain barrier, you had to maneuver through layers of platelet walls");
                        return true;
                    },
                    "Penetration Complete"
                    );
            case 13:
                return new GameLevel(
                    "Synapse",
                    (GameMap map) => {
                        GameLevel.CreateColony(new Vector2(120, -612), 70, new Vector2(10.0f, 10.0f), ColonyType.PLAYER, CoreData.Create(
                            60, 10, 5, 50,
                            1.0f, false, false, () => { }));

                        map.SetLevelCameraView(new Vector3(10, -800, 0), new Vector3(800, -10, 0));
                        Material flesh = Resources.Load<UnityEngine.Material>("Material/FleshMaterial");
                        float max_x = 3000;
                        float min_x = -1000;
                        float max_y = 2500;
                        float min_y = -2500;

                        GameLevel.CreateColony(new Vector2(380, -387), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));
                        GameLevel.CreateColony(new Vector2(503, -494), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));
                        GameLevel.CreateColony(new Vector2(640, -255), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));
                        GameLevel.CreateColony(new Vector2(715, -434), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));
                        GameLevel.CreateColony(new Vector2(932, -446), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));
                        GameLevel.CreateColony(new Vector2(879, -228), 10, new Vector2(10.0f, 10.0f), ColonyType.SYNAPSE, CoreData.Create(
                        250, 15, 5, 40,
                        1.0f, false, true, () => {
                        }));

                        GameLevel.CreateAdipose(new Vector2(413, -454));
                        GameLevel.CreateAdipose(new Vector2(468, -338));
                        GameLevel.CreateAdipose(new Vector2(744, -314));
                        GameLevel.CreateAdipose(new Vector2(455, -244));
                        GameLevel.CreateAdipose(new Vector2(413, -454));
                        GameLevel.CreateAdipose(new Vector2(468, -338));
                        GameLevel.CreateAdipose(new Vector2(744, -314));
                        GameLevel.CreateAdipose(new Vector2(455, -244));


                        for (int i = 0; i < 5; i++)
                        {
                            GameLevel.CreatePlatelet(new Vector2(240, -510) + new Vector2(10, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(259, -517), new Vector2(229, -505));
                            GameLevel.CreatePlatelet(new Vector2(290, -430) + new Vector2(10, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(282, -428), new Vector2(301, -448));
                            GameLevel.CreatePlatelet(new Vector2(586, -520) + new Vector2(3, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(584, -530), new Vector2(590, -490));
                            GameLevel.CreatePlatelet(new Vector2(750, -283) + new Vector2(50, 5) * new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
                                new Vector2(707, -284), new Vector2(794, -282));
                        }
                        {

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(85, -535),
new Vector2(282, -422),
new Vector2(229, -505),
new Vector2(173, -639)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(211, -680),
new Vector2(205, -626),
new Vector2(232, -560),
new Vector2(301, -448),
new Vector2(336, -453)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(211, -680),
new Vector2(336, -453),
new Vector2(360, -440),
new Vector2(432, -484)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(211, -680),
new Vector2(432, -484),
new Vector2(477, -546)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(432, -484),
new Vector2(458, -480),
new Vector2(477, -546)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(510, -551),
new Vector2(554, -518),
new Vector2(608, -538),
new Vector2(640, -568)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(744, -270),
new Vector2(727, -246),
new Vector2(820, -206),
new Vector2(796, -247)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(796, -247),
new Vector2(820, -206),
new Vector2(827, -226),
new Vector2(819, -244)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(700, -238),
new Vector2(708, -211),
new Vector2(820, -206),
new Vector2(796, -247),
new Vector2(727, -246)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(708, -211),
new Vector2(694, -173),
new Vector2(855, -173),
new Vector2(820, -206)
}, flesh, true, false);

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(908, -380),
new Vector2(917, -326),
new Vector2(992, -406)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(917, -329),
new Vector2(907, -284),
new Vector2(932, -244),
new Vector2(992, -406)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(932, -244),
new Vector2(938, -185),
new Vector2(992, -406)
}, flesh, true, false);
                        }

                        {

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, min_y),
new Vector2(146, -699),
new Vector2(211, -680),
new Vector2(211, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(211, min_y),
new Vector2(211, -680),
new Vector2(477, -546),
new Vector2(510, -551),
new Vector2(640, -568),
new Vector2(640, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(640, min_y),
new Vector2(640, -568),
new Vector2(711, -560),
new Vector2(744, -570),
new Vector2(744, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(744, min_y),
new Vector2(744, -570),
new Vector2(780, -555),
new Vector2(810, -558),
new Vector2(810, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(810, min_y),
new Vector2(810, -558),
new Vector2(866, -530),
new Vector2(903, -532),
new Vector2(903, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(903, min_y),
new Vector2(903, -532),
new Vector2(991, -451),
new Vector2(max_x, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(991, -451),
new Vector2(992, -406),
new Vector2(max_x, -406),
new Vector2(max_x, min_y)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(992, -406),
new Vector2(938, -185),
new Vector2(max_x, max_y),
new Vector2(max_x, -406)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, min_y),
new Vector2(76, -640),
new Vector2(146, -699)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, min_y),
new Vector2(min_x, -569),
new Vector2(51, -569),
new Vector2(76, -640)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, -569),
new Vector2(min_x, -535),
new Vector2(85, -535),
new Vector2(51, -569)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, -535),
new Vector2(min_x, -395),
new Vector2(269, -395),
new Vector2(282, -422),
new Vector2(85, -535)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(min_x, -395),
new Vector2(min_x, max_y),
new Vector2(269, max_y),
new Vector2(269, -395)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(269, -398),
new Vector2(269, max_y),
new Vector2(319, max_y),
new Vector2(319, -326)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(319, -326),
new Vector2(319, max_y),
new Vector2(398, max_y),
new Vector2(398, -285),
new Vector2(386, -322),
new Vector2(348, -341)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(398, -285),
new Vector2(398, max_y),
new Vector2(491, max_y),
new Vector2(491, -206)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(491, -206),
new Vector2(491, max_y),
new Vector2(608, max_y),
new Vector2(608, -192),
new Vector2(557, -180)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(608, -192),
new Vector2(608, max_y),
new Vector2(649, max_y),
new Vector2(649, -160)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(649, -160),
new Vector2(649, max_y),
new Vector2(694, max_y),
new Vector2(694, -173)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(694, -173),
new Vector2(694, max_y),
new Vector2(835, max_y),
new Vector2(835, -173)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(835, -173),
new Vector2(835, max_y),
new Vector2(885, max_y),
new Vector2(885, -168),
new Vector2(859, -176)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(885, -168),
new Vector2(885, max_y),
new Vector2(max_x, max_y),
new Vector2(938, -185),
new Vector2(897, -181)
}, flesh, true, false);
                        }
                        //Inner Islands
                        {

                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(430, -294),
new Vector2(450, -271),
new Vector2(515, -234),
new Vector2(575, -260),
new Vector2(578, -288),
new Vector2(558, -326),
new Vector2(531, -343),
new Vector2(500, -336),
new Vector2(463, -320)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(515, -234),
new Vector2(584, -223),
new Vector2(575, -260)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(368, -378),
new Vector2(412, -338),
new Vector2(433, -336),
new Vector2(507, -373),
new Vector2(510, -491),
new Vector2(489, -446),
new Vector2(450, -453),
new Vector2(366, -400)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(549, -443),
new Vector2(542, -379),
new Vector2(600, -423)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(549, -443),
new Vector2(600, -423),
new Vector2(627, -423),
new Vector2(641, -427),
new Vector2(642, -474),
new Vector2(619, -505),
new Vector2(540, -483)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(619, -505),
new Vector2(642, -474),
new Vector2(671, -517),
new Vector2(643, -530)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(715, -518),
new Vector2(744, -505),
new Vector2(803, -518),
new Vector2(750, -534)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(744, -505),
new Vector2(767, -470),
new Vector2(789, -485),
new Vector2(803, -518)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(803, -518),
new Vector2(789, -485),
new Vector2(825, -495),
new Vector2(847, -514)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(772, -436),
new Vector2(870, -399),
new Vector2(886, -420),
new Vector2(852, -270),
new Vector2(811, -463)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(772, -436),
new Vector2(850, -277),
new Vector2(873, -288),
new Vector2(886, -320),
new Vector2(870, -399)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(772, -436),
new Vector2(758, -388),
new Vector2(767, -300),
new Vector2(837, -263),
new Vector2(850, -277)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(571, -354),
new Vector2(690, -335),
new Vector2(646, -313),
new Vector2(626, -379),
new Vector2(593, -376)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(626, -378),
new Vector2(646, -313),
new Vector2(689, -269),
new Vector2(664, -387),
new Vector2(654, -400)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(689, -269),
new Vector2(728, -300),
new Vector2(696, -368),
new Vector2(664, -387)
}, flesh, true, false);
                            GameLevel.CreateBlocking(Vector2.zero, new List<Vector2> {
new Vector2(696, -368),
new Vector2(728, -300),
new Vector2(732, -350),
new Vector2(727, -384)
}, flesh, true, false);
                        }
                    },
                    (GameMap map) => {
                        int devour_count = 0;

                        for (int i = 2; i < 8; i++)
                        {
                            if(!map.HasCore(i))
                            {
                                devour_count++;
                            }
                        }
                        
                        map.condition_text.SetText("Consume the synapses (" + devour_count + " / 6)");
                        return devour_count >= 6;
                    },
                    "Assimilation Complete"
                    );
        }
        return null;
    }
}