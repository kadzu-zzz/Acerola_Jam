using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    public GameObject root;

    public List<GameObject> cinematic_displays;

    List<Action> cinematic_intro;
    List<Action> cinematic_breach;
    List<Action> cinematic_breakout;


    public Sprite[] sprite_intro, sprite_breach, sprite_breakout;

    int id;
    Action callback_finish;

    int current = 0;

    private void Start()
    {
        root.SetActive(false);
        foreach(var obj in cinematic_displays)
        {
            obj.SetActive(false);
            obj.transform.parent.gameObject.SetActive(false);
        }

        cinematic_intro = new List<Action> {
            () =>
            {
                for(int i = 0; i < 4; i++)
                    cinematic_displays[i].GetComponent<Image>().sprite = sprite_intro[i];
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            }
        };
        cinematic_breach = new List<Action> {
            () =>
            {
                for(int i = 0; i < 6; i++)
                    cinematic_displays[i].GetComponent<Image>().sprite = sprite_breach[i];
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            }
        };
        cinematic_breakout = new List<Action> {
            () =>
            {
                for(int i = 0; i < 5; i++)
                    cinematic_displays[i].GetComponent<Image>().sprite = sprite_breakout[i];
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            },
            () =>
            {
            }
        };
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            ShowFrame(++current);
        }        
    }

    public void PlayCinematicUI(int id)
    {
        PlayCinematic(id, null);
    }
    
    public void PlayCinematic(int id, Action callOnFinish)
    {
        if(id >= 0 && id <= 2)
        {
            this.id = id;
            callback_finish = callOnFinish;
            root.SetActive(true);
            root.GetComponent<RectTransform>();
            current = 0;
            ShowFrame(0);            
        }
    }

    public void ShowFrame(int index)
    {
        ShowFrame(index, id == 0 ? cinematic_intro : id == 1 ? cinematic_breach : cinematic_breakout);    
    }

    void ShowFrame(int index, List<Action> cinematic)
    {
        if(index >= cinematic.Count)
        {
            CancelCinematic();
            if(callback_finish != null)
                callback_finish();
        }
        else
        {
            cinematic[index]();
            cinematic_displays[index].SetActive(true);
            cinematic_displays[index].transform.parent.gameObject.SetActive(true);
        }
    }

    public void CancelCinematic()
    {
        root.SetActive(false);
        foreach (var obj in cinematic_displays)
        {
            obj.SetActive(false);
            obj.transform.parent.gameObject.SetActive(false);
        }
    }
}
