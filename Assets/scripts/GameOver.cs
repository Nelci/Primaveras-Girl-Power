using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : GameManagerBase
{
    private string currentLevel = SceneMenu;
    public void Start() {
        currentLevel = PlayerPrefs.GetString(PrefCurrentScene, SceneMenu);
    }
    public void GameReStartLevel() {
        SceneManager.LoadScene(currentLevel);
    } 

    public void GameGiveUp() {
        SceneManager.LoadScene(SceneMenu);
    }
    public void GameRequestHelp() {
         Application.OpenURL("https://www.primaverasgirlspower.com/busque-ajuda");
    }
    public void GameQuit() {
         Application.Quit();
    }
}
