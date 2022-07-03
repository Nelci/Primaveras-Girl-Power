using UnityEngine;
using UnityEngine.SceneManagement;

public class PreFase2 : MonoBehaviour
{
    public void StartLevel3() {
        SceneManager.LoadScene("Fase3");
    } 

    public void GameQuit() {
         Application.Quit();
    }
}
