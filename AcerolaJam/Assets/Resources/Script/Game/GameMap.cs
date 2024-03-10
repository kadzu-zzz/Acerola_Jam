using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMap : MonoBehaviour
{
    public static GameMap Instance;

    public PolygonCollider2D eye_zone;

    public TMPro.TextMeshProUGUI title_text, condition_text;

    public GameObject GameOverUI;

    public static int level = 0;
    GameLevel current;

    float win_check = 1.0f;
    public bool check = true;

    public float delay = 4.5f;

    void Start()
    {
        Instance = this;
        level = TransferToLevel.int_level;
        current = GameLevels.GetLevel(level);
        current.Setup(this);

        title_text.SetText(current.name);

        current.CheckVictory(this);
    }


    public CoreData Player()
    {
        return ColonySystem.handle.GetCore(1);
    }

    public CoreData GetCore(int core)
    {
        return ColonySystem.handle.GetCore(core);
    }

    public bool HasCore(int core)
    {
        return ColonySystem.handle.HasCore(core);
    }

    public void ObjectsDestroyed(List<int> obj_ids)
    {

    }

    void Update()
    {
        if (!check)
            return;
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        if (!HasCore(1))
        {
            check = false;
            GameOverUI.SetActive(true);
            return;
        }
        win_check -= Time.deltaTime;
        if (win_check < 0.0f)
        {
            win_check += 1.0f;
            if(current.CheckVictory(this))
            {
                MapSceneSelection.first_complete = (GameManager.Instance().data.level_progress < level);
                MapSceneSelection.level_complete = level;
                GameManager.Instance().data.level_progress = Mathf.Max(level, GameManager.Instance().data.level_progress);
                SceneManager.LoadScene("MapScene");
            }
        }
    }

    public void UI_Retry()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void UI_Menu()
    {
        SceneManager.LoadScene("MapScene");
    }

}
