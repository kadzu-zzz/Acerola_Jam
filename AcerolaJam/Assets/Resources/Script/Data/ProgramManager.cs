using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance()
    {
        return instance;
    }
    static ProgramManager instance = null;

    public ProgramDataV2 data;
    bool is_quitting = false;
    float quit_delay;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnStart()
    {
        data = SaveLoadHelper<ProgramDataV2>.Load("");
        Debug.Log(data.example_program_setting);
        data.example_program_setting = 42;

        data.postLoad();
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefab/Management/GameManager"));
    }

    private void Update()
    {
        if(is_quitting)
        {
            quit_delay -= Time.deltaTime;
            if(quit_delay < 0.0f)
            {
                Quit();
                is_quitting = false;
            }
        }
    }

    public void Cleanup()
    {
        SaveLoadHelper<ProgramDataV2>.Save(data);
    }

    public void GameStarted()
    {

    }

    public void GameStopped()
    {

    }

    private void Start()
    {
        OnStart();
    }

    private void OnApplicationQuit()
    {
        Cleanup();
    }

    public void Quit(float delay)
    {
        is_quitting = true;
        quit_delay = delay;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }
}
