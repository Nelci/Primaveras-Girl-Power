using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerFase4 : GameManagerFaseBase
{
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI gameOverText;
    public GameObject refObject;

    void Start()
    {
        LevelName = "Fase4";
        PlayerPrefs.SetString(PrefCurrentScene, LevelName);
        gameOverText.enabled = getIsGameOver();
        addScore(PlayerPrefs.GetInt(PrefScore, 0));
    }
    IEnumerator waiterGameOver()
    {
        //Wait for 5 seconds
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(SceneGameOver);
    }
    IEnumerator waiterFinish()
    {
        //Wait for 5 seconds
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene("Thanks");
    }
    public override void OnChangeIsGameOver(bool isGameOver)
    {
        if (!isGameOver)
        {
            return;
        }
        gameOverText.enabled = isGameOver;
        StartCoroutine(waiterGameOver());
    }

    public override void OnChangeScore(int value)
    {
        scoreText.text = getScore().ToString();
    }
    
    public override void OnChangeIsFinished(bool isFinished)
    {
        PlayerPrefs.SetInt(PrefScore, getScore());
        StartCoroutine(waiterFinish());
        
    }
}

