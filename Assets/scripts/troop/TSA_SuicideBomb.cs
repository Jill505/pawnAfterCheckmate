using UnityEngine;

public class TSA_SuicideBomb : MonoBehaviour
{
    public int CountDown = 2;

    public RoundManager roundManager;
    public GameManager gameManager;

    public Troop myTroop;


    public void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void Start()
    {
        roundManager.Action_OnRoundEnd += RoundProcess;
    }


    //自爆炸彈兵
    public void DoSuicide()
    {
        Debug.Log("KA BOOM!");
        //環形檢查自己周圍九格的目標，若大於關卡陣列
        int myNX = myTroop.myNowX;
        int myNY = myTroop.myNowY;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    //Do not suicide yet.
                }
                else
                {
                    CheckAndKill(myNX + i, myNY + j);
                }
            }
        }

        myTroop.killTroop();
    }

    public void CheckAndKill(int X, int Y)
    {
        if (X >= gameManager.levelData.gridSizeX || X < 0)
        {
            //SKIP
            return;
        }
        if (Y >= gameManager.levelData.gridSizeY || Y < 0)
        {
            //SKIP
            return;
        }

        Troop t = gameManager.chessBoardObjectRefArr[Y, X].GetComponent<unit>().TroopsOnMe;

        if (t != null)
        {
            //there's troop on the unit.
            if (t.isPlayer)
            {
                roundManager.MakePlayerDie();
            }
            else
            {
                t.killTroop();
            }
        }
    }

    public void RoundProcess()
    {
        CountDown--;

        if (CountDown <= 0)
        {
            DoSuicide();
        }
    }
    private void OnDestroy()
    {
        roundManager.Action_OnRoundEnd -= RoundProcess;
    }
}
