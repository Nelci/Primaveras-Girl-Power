using UnityEngine;
using UnityEngine.SceneManagement;

public class PreFase4 : MonoBehaviour
{
    public void StartLevel4() {
        SceneManager.LoadScene("Fase4");
    } 

    public void GameQuit() {
         Application.Quit();
    }
}
