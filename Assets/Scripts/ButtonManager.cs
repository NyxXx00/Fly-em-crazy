using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public void _ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }



    public void _Exit()
    {
        Application.Quit();
    }
}
