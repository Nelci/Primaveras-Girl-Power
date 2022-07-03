using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerFase1 : GameManagerFaseBase
{
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI gameOverText;
    public GameObject refObject;
    public GameObject aggretion1;
    public GameObject aggretion2;

    public float timeInSeconds = 30f;
    public float intervalInSeconds = 1f;

    private Timer timer1;
    private float counter = -60f;
    private int period = 250;
    private GameObject[] aggretionList = new GameObject[] { };
    void Start()
    {
        LevelName = "Fase1";
        PlayerPrefs.SetString(PrefCurrentScene, LevelName);
        StartCoroutine(CreateAggretion());
        counter = -1 * timeInSeconds;
        timer1 = new Timer(timer1_Tick, null, 1000, period);
        gameOverText.enabled = getIsGameOver();
    }

    public override void OnChangeIsVictimDead(bool isVictimDead)
    {
        if (!isVictimDead)
        {
            return;
        }
        pointsConsolidation();
    }

    private void pointsConsolidation()
    {
        timer1.Dispose();
        if (counter < 0)
        {
            setIsGameOver(true);
            return;
        }
        addScore(100 + (((int)counter) * 10));
        setIsFinished(true);
    }

    private IEnumerator CreateAggretion()
    {
        aggretionList = new GameObject[] { aggretion1, aggretion2 };
        randomInstatiateAggretion();

        yield return new WaitForSeconds(intervalInSeconds);

        yield return CreateAggretion();
    }

    private void randomInstatiateAggretion()
    {
        var limit = Random.Range(0, 1);
        for (int i = 0; i <= limit; i++)
        {
            instatiateAggretion(aggretionList[Random.Range(0, aggretionList.Length)]);
        }

    }
    private void instatiateAggretion(GameObject aggretion)
    {
        var z = Random.Range(45, 55);
        var drag = Random.Range(0f, 2f);
        var obj = Instantiate(aggretion);
        obj.transform.position = new Vector3(6.051f, 12f, z);
        obj.GetComponent<Rigidbody>().drag = drag;

    }
    private void timer1_Tick(object sender)
    {
        Debug.Log("timer1_Tick: " + counter);
        counter += period / 1000f;
    }
    IEnumerator waiterGameOver()
    {
        //Wait for 5 seconds
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(SceneGameOver);
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
    public override void OnChangeIsFinished(bool isFinished)
    {
        PlayerPrefs.SetInt(PrefScore, getScore());
        SceneManager.LoadScene("PreFase2");
    }

    // public override void OnChangeScore(int value)
    // {
    //     scoreText.text = counter.ToString("F");
    // }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = counter.ToString("F");
        if (counter >= timeInSeconds)
        {
            setVictimDead(true);
        }
    }
}

