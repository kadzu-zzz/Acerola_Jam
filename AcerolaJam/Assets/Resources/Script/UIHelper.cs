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
    public void UI_Exit_Delay(float delay)
    {
        ProgramManager.Instance().Quit(delay);
    }

    public void UI_OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
