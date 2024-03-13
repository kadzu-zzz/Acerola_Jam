using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using Unity.Collections;
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
    public GameObject GameVictoryUI;
    public GameObject GameEscapeUI;
    public TMPro.TextMeshProUGUI victory_text;

    public GameObject lookatfocus;
    public PolygonCollider2D level_bounds;

    public CinemachineVirtualCamera colony_camera, level_camera;

    public AudioClip[] pop_sounds;
    public AudioClip[] grow_sounds;

    public static int level = 0;
    GameLevel current;

    float win_check = 1.0f;
    public bool check = true;

    public float delay = 4.5f;
    bool second_delay = false;

    void Start()
    {
        Instance = this;
        level = TransferToLevel.int_level;
        ColonySystem.player_follow_mouse = false;
        current = GameLevels.GetLevel(level);
        current.Setup(this);
        var v = Player().center;
        lookatfocus.transform.SetPositionAndRotation(new Vector3(v.x, v.y, 0.0f), Quaternion.identity);
        title_text.SetText(current.name);
        victory_text.SetText(current.victory_text);
        RenderSystem.eye_bounds = eye_zone;

        level_camera.Priority = 20;
        colony_camera.Priority = 10;

        current.CheckVictory(this);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
    public void SetLevelCameraView(Vector3 min, Vector3 max)
    {    
        Vector3 center = (min + max) / 2;
        level_camera.transform.LookAt(center);

        float boundsWidth = max.x - min.x;
        float halfFOV = (level_camera.m_Lens.FieldOfView / 2) * Mathf.Deg2Rad;
        float distance = boundsWidth / (2 * Mathf.Tan(halfFOV));

        Vector3 direction = (level_camera.transform.position - center).normalized;
        level_camera.transform.position = center - direction * -distance;
    }

    public CoreData Player()
    {
        return ColonySystem.handle.GetCore(1);
    }

    public void UpdatePlayer(CoreData c)
    {
        ColonySystem.handle.UpdateCore(1, c);
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
            if (delay < 0 && ! second_delay)
            {
                delay += 3.0f;
                second_delay = true;
                level_camera.Priority = 10;
                colony_camera.Priority = 20;
            }
            if(delay < 0 && second_delay)
            {
                ColonySystem.player_follow_mouse = true;
            }
            return;
        }

        if (!HasCore(1))
        {
            check = false;
            GameOverUI.SetActive(true);
            return;
        }
        lookatfocus.transform.position = new Vector3(GetCore(1).center.x, GetCore(1).center.y, 0);
        win_check -= Time.deltaTime;
        if (win_check < 0.0f)
        {
            win_check += 0.05f;
            if(current.CheckVictory(this))
            {
                check = false;
                GameVictoryUI.SetActive(true);
                return;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            bool state = !GameEscapeUI.gameObject.activeInHierarchy;

            GameEscapeUI.SetActive(state);

            Time.timeScale = state ? 0.0f : 1.0f;
        }

        if((Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.S)) ||
            (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.S)))
        {
            if (this.HasCore(1) && this.Player().cells <= 1000)
            {
                EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
                GameLevel.GenerateCells(buffer, 1, 20);
                buffer.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
                buffer.Dispose();
            }
        }
        if ((Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.T)) ||
            (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.T)))
        {
            check = false;
            GameVictoryUI.SetActive(true);
        }
    }

    public void FixTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public void UI_VictoryMap()
    {
        check = false;
        MapSceneSelection.first_complete = (GameManager.Instance().data.level_progress < level);
        MapSceneSelection.level_complete = level;
        GameManager.Instance().data.level_progress = Mathf.Max(level, GameManager.Instance().data.level_progress);
        SceneManager.LoadScene("MapScene");
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
