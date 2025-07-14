using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;   

public class testRoundMaster : MonoBehaviour
{
    [Header("本場遊戲資訊")]
    public bool GameGoing = false;
    public bool isPlayerRound = false;
    public int leftRoundCount = 5;

    [Header("UI")]
    public GameObject testGameStartButton;
    public GameObject gameCanvas;
    public GameObject GameEnviroment;

    public void GameStart()
    {
        GameInitialization(config);

        testGameStartButton.SetActive(false);

        gameCanvas.SetActive(true);
        GameEnviroment.SetActive(true);
    }

    public void playerEndDect()
    {
        StartCoroutine(GameAnimationPlay());
        //WaitForIt
    }

    public IEnumerator GameAnimationPlay()
    {
        yield return null;
    }

    public void GameInitialization(GameConfig config)
    {

    }

    public GameConfig config;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class GameConfig
{
    public int allowRoundCount = 5;
    public int mapSizeX = 5;
    public int mapSizeY = 5;



    public string gameTarget = "default";

    public GameConfig(int allowRoundCount, string gameTarget)
    {
        this.allowRoundCount = allowRoundCount;
        this.gameTarget = gameTarget;
    }
}
