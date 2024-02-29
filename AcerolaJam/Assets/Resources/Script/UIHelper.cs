using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHelper : MonoBehaviour
{
    public string transition_scene = "";
    public LoadSceneMode transition_mode = LoadSceneMode.Single;

    public void UI_ChangeScene()
    {
        SceneManager.LoadScene(transition_scene, transition_mode);
    }

    public void UI_Exit()
    {
        ProgramManager.Instance().Quit();
    }
}
