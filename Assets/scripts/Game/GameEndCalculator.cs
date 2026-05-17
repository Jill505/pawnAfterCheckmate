using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndCalculator : MonoBehaviour
{
    public TimerManager timerManager;
    public RoundManager roundManager;
    public GameManager gameManager;

    public Animator GameEndCalCanvasAnimator;
    public Animator GameEndCanvasAnimator;
    public Image GameEndCanvasBg_L;
    public Image GameEndCanvasBg_R;

    public Animator GameEndBlackScreenAnimator;

    public float totalTime = 0;
    public string levelName = "";

    public TextMeshProUGUI GameTitle;
    public TextMeshProUGUI GameID; 
    public TextMeshProUGUI RoundCount;
    public TextMeshProUGUI TotalTime;
    public TextMeshProUGUI KillCount;

    private void Awake()
    {
        timerManager = FindFirstObjectByType<TimerManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void EndAnimatorActive(bool isPlayerDie)
    {
        if (isPlayerDie)
        {
            GameEndCanvasBg_L.color = Color.black;
            GameEndCanvasBg_R.color = Color.black;
        }
        else
        {
            GameEndCanvasBg_L.color = Color.white;
            GameEndCanvasBg_R.color = Color.white;
        }
        GameEndCanvasAnimator.SetTrigger("Active");
        GameEndCalCanvasAnimator.SetTrigger("GameEnd");

        //Set Text
        GameTitle.text = gameManager.LevelName;

        GameID.text = gameManager.LevelID;

        TimeSpan t = TimeSpan.FromSeconds(timerManager.timeSpan);
        string time = t.ToString(@"mm\:ss");
        TotalTime.text = time;

        RoundCount.text = "回合數: " + roundManager.roundCount;

        KillCount.text = "擊殺數: "+ gameManager.killCount ;
    }

    public void LoadGameEndFunc()
    {

    }

    public void BackToLobbyFunc()
    {
        StartCoroutine(BackToLobbyCoroutine());
    }
    public IEnumerator BackToLobbyCoroutine()
    {
        GameEndBlackScreenAnimator.SetTrigger("Active");
        yield return null;
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(1);
    }
}
