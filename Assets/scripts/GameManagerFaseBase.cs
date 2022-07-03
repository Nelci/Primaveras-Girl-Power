using UnityEngine;

public class GameManagerFaseBase : GameManagerBase
{
    private int score = 0;
    private bool isGameOver = false;
    private bool isVictimDead = false;
    private bool isFinished = false;

    public string LevelName = SceneMenu;

    public int getScore()
    {
        return score;
    }

    public void addScore(int value)
    {
        if (value == 0)
        {
            return;
        }
        score += value;
        OnChangeScore(value);
    }
    public virtual void OnChangeScore(int value)
    {

    }
    public void setVictimDead(bool v)
    {
        if (isVictimDead == v)
        {
            return;
        }
        isVictimDead = v;
        OnChangeIsVictimDead(isVictimDead);
    }

    public virtual void OnChangeIsVictimDead(bool isVictimDead)
    {
        if (!isVictimDead)
        {
            return;
        }
        setIsGameOver(isVictimDead);
    }

    public bool getIsGameOver()
    {
        return isGameOver;
    }
    public void setIsGameOver(bool v)
    {
        if (isGameOver == v)
        {
            return;
        }
        isGameOver = v;
        OnChangeIsGameOver(isGameOver);
    }

    public virtual void OnChangeIsGameOver(bool isGameOver)
    {

    }
    public bool getIsFinished()
    {
        return isFinished;
    }
    public void setIsFinished(bool v)
    {
        if (isFinished == v)
        {
            return;
        }
        isFinished = v;
        OnChangeIsFinished(isFinished);
    }

    public virtual void OnChangeIsFinished(bool isFinished)
    {

    }

}

