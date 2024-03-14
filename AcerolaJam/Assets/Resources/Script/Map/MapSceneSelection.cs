using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSceneSelection : MonoBehaviour
{
    public static bool first_complete = false;
    public static int level_complete = -1;
    int current_level;

    public Button[] level_buttons = new Button[1];
    public Button ui_transit_lab, ui_transit_body;
    public Button ui_replay_intro, ui_replay_breach, ui_replay_breakout;

    public CinemachineVirtualCamera lab_camera, body_camera;
    public CinematicController cine;

    public GameObject EscapeUI;

    public List<SpriteRenderer> lab_images =new List<SpriteRenderer>();

    private void Awake()
    {
        LevelChangeCleanupSystem.ForceClean();
    }

    void Start()
    {
        current_level = GameManager.Instance().data.level_progress;

        //Skip Unfinished Levels
        if(level_complete == 8)
        {
            current_level = GameManager.Instance().data.level_progress = 12;
            level_complete = 12;
        }

        int index = 0;
        foreach (Button b in level_buttons)
        {
            if (index++ <= current_level + 1) 
                b.gameObject.SetActive(true);
            if (index == current_level + 2)
                b.GetComponent<SlightMovement>().Set(4, 2);
        }

        for (int i = 0; i < lab_images.Count; i++)
        {
            lab_images[i].gameObject.SetActive(i <= current_level + 1);
        }

        if (current_level == -1)
        {
            current_level = (GameManager.Instance().data.level_progress = 0);

            cine.PlayCinematic(0, () =>
            {
            });
        }


        if (current_level < 6) 
            MoveToLab();
        else
            MoveToBody();

        if (current_level < 6)
        {
            ui_replay_breach.gameObject.SetActive(false);
            ui_transit_lab.gameObject.SetActive(false);
        }
        if(current_level < 13)
            ui_replay_breakout.gameObject.SetActive(false);


        if (first_complete)
        {
            if (level_complete == 6)
            {
                cine.PlayCinematic(1, () =>
                {
                    MoveToBody();
                });
            }
            else if (level_complete == 13)
            {
                first_complete = false;
                cine.PlayCinematic(2, () =>
                {
                    SceneManager.LoadScene("CreditScene");
                });
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool state = !EscapeUI.gameObject.activeInHierarchy;

            EscapeUI.SetActive(state);
        }
    }

    public void MoveToBody()
    {
        lab_camera.Priority = 10;
        body_camera.Priority = 100;
    }

    public void MoveToLab()
    {
        lab_camera.Priority = 100;
        body_camera.Priority = 10;
    }
}
