using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance()
    {
        return instance;
    }
    static ProgramManager instance = null;

    public ProgramDataV2 data;

    private void Awake()
    {
        instance = this;        
    }

    public void OnStart()
    {
        data = SaveLoadHelper<ProgramDataV2>.Load("");
        Debug.Log(data.example_program_setting);
        data.example_program_setting = 42;

        data.postLoad();
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefab/Management/GameManager"));
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
}
