using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferToLevel : MonoBehaviour
{
    public static string level_string = "";

    public string target_level = "";

    public void Transfer()
    {
        level_string = target_level;
        SceneManager.LoadScene("GameScene");
    }
}
