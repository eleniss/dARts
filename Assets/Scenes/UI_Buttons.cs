using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("0");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("exit game"); 
    }
}
