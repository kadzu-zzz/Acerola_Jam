using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferToLevel : MonoBehaviour
{
    public static int int_level;

    public int target_level = 0;

    public void Transfer()
    {
        int_level = target_level;
        SceneManager.LoadScene("GameScene");
    }
}
