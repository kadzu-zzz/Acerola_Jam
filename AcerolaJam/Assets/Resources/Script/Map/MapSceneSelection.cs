using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSceneSelection : MonoBehaviour
{
    public static bool first_complete = false;
    public static int level_complete = -1;
    int current_level;

    public Button[] level_buttons = new Button[1];
    public Button ui_transit_lab, ui_transit_body;
    public Button ui_replay_intro, ui_replay_breach, ui_replay_breakout;

    public ChangeBounds b_lab, b_body;
    public CinematicController cine;

    private void Awake()
    {
        LevelChangeCleanupSystem.ForceClean();
    }

    void Start()
    {
        current_level = GameManager.Instance().data.level_progress;

        if(current_level == -1)
        {
            current_level = (GameManager.Instance().data.level_progress = 0);

            cine.PlayCinematic(0, () =>
            {
            });
        }

        int index = 0;
        foreach(Button b in level_buttons)
        {
            b.interactable = index++ <= current_level + 1;
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
                cine.PlayCinematic(2, () =>
                {
                    //Credits?
                });
            }
        }
    }

    public void MoveToBody()
    {
        b_lab.Change();
    }

    public void MoveToLab()
    {
        b_body.Change();
    }
}
