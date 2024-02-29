using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance()
    {
        return instance;
    }
    static GameManager instance = null;

    public SaveDataV1 data;

    public static ConcreteObject concrete;

    private void Awake()
    {
        instance = this;
    }

    public void StopGame()
    {
        Destroy(gameObject);
        ProgramManager.Instance().GameStopped();
    }

    public void OnStart()
    {
        ProgramManager.Instance().GameStarted();

        data = SaveLoadHelper<SaveDataV1>.Load("test_save");
        Debug.Log(data.test_data);
        data.test_data = 42;

        data.postLoad();

        SceneManager.sceneUnloaded += SceneUnloading;

    }

    public void Cleanup()
    {
        SaveLoadHelper<SaveDataV1>.Save(data);
    }

    private void Start()
    {
        OnStart();
    }

    private void SceneUnloading(Scene scene)
    {
        Cleanup();
    }
}
