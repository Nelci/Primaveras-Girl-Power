using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : GameManagerBase
{
    public void Start() {
        PlayerPrefs.SetInt(PrefScore, 0);
    }
    public void GameStart() {
        SceneManager.LoadScene("PreFase1");
    } 

    public void GameQuit() {
         Application.Quit();
    }
}
