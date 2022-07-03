using UnityEngine;
using UnityEngine.SceneManagement;

public class PreFase1 : MonoBehaviour
{
    public void StartLevel1() {
        SceneManager.LoadScene("Fase1");
    } 

    public void GameQuit() {
         Application.Quit();
    }
}
