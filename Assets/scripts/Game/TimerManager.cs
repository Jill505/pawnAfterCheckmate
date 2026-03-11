using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TimerManager : MonoBehaviour
{
    [Header("Component Refs")]
    public GameManager gameManager;
    public RoundManager roundManager;
    public TrickManager trickManager;

    [Header("Timer Variables")]
    public bool isPause = false;
    public bool isCountingTime = false;

    public float myIndependentTimeScale = 1;

    public float leftTime = 0;
    public float leftTimeCT =0 ;
    public float leftTime_forShow = 0;

    public TextMeshProUGUI allLeftTimeTMP;
    public float systemSmoothTime = 1f;


    public float roundTime = 10f; 
    public float roundTimeCT = 0; 
    public float roundTime_forShow = 0;
    //public float roundTime_forShow = 0; 
    public Image roundLeftTimeImage;
    public TextMeshProUGUI roundLeftTimeTMP;


    public float roundStartBufferTime = 1.2f;
    public float roundStartBufferCT = 0f;

    public float roundTimeEndBufferTime = 1f;
    public float roundTimeEndBufferCT= 0f;

    public float leftTimeDeadBufferTime = 2f;
    public float leftTimeDeadBufferCT = 0f;

    public bool playerDieClog = false;

    public void InitTimer()
    {
        if (SaveSystem.SF.difficulty == 0)
        {
            Debug.Log("timer更 虏虫家Α");
            myIndependentTimeScale = 1;
            leftTime = 301f;
            roundTime = 10f;

            roundStartBufferTime = 1.2f;
            roundTimeEndBufferTime = 1f;
            leftTimeDeadBufferTime = 2f;
        }
        else if (SaveSystem.SF.difficulty == 1)
        {
            Debug.Log("timer更 螟家Α");
            myIndependentTimeScale = 1;
            leftTime = 61f;
            roundTime = 5f;

            roundStartBufferTime = 0.9f;
            roundTimeEndBufferTime = 0.5f;
            leftTimeDeadBufferTime = 2f;
        }
        else
        {
            Debug.Log("timer更 冠芁家Α");
            myIndependentTimeScale = 1;
            leftTime = 10f;
            roundTime = 3f;

            roundStartBufferTime = 0.9f;
            roundTimeEndBufferTime = 0.5f;
            leftTimeDeadBufferTime = 2f;
        }

        leftTimeCT = leftTime;
        roundStartBufferCT = roundStartBufferTime;
        roundTimeEndBufferCT = roundTimeEndBufferTime;
        leftTimeDeadBufferCT = leftTimeDeadBufferTime;
    }

    public void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();   
        trickManager = FindFirstObjectByType<TrickManager>();
        
        leftTime_forShow = 0;
    }

    private void Update()
    {
        SyncTimer(systemSmoothTime);
        TimeCounting();
    }

    float _swap_V =0;
    public void SyncTimer(float smoothTime = 1f)
    {
        leftTime_forShow = Mathf.LerpUnclamped(leftTime_forShow, leftTimeCT, (4* leftTimeCT / 60)* smoothTime * Time.deltaTime);
        int min = (int)leftTime_forShow / 60;
        int sec = (int)leftTime_forShow % 60;
        allLeftTimeTMP.text = $"{min:00}:{sec:00}";

        roundTime_forShow = Mathf.LerpUnclamped(roundTime_forShow, roundTimeCT, 5 * Time.deltaTime);
        roundLeftTimeImage.fillAmount = roundTime_forShow / roundTime;

        if (roundTimeCT >= 10f)
        {
            roundLeftTimeTMP.text = (int)roundTimeCT+"s";
        }
        else
        {
            string v = roundTimeCT.ToString("0.0");
            roundLeftTimeTMP.text = v + "s";
        }
    }
    public void TimeCounting()
    {
        //if (!isPause || SaveSystem.SF.difficulty >=2) //Game is processing
        if (!isPause) //Game is processing
        {
            if (isCountingTime) //it's player's round
            {
                //go round start buffer
                //if round start buffer is 0
                if (roundStartBufferCT > 0)
                {
                    roundStartBufferCT -= Time.deltaTime * myIndependentTimeScale;
                    return;
                }

                //go count roundTime;
                //if roundTime is 0;
                if (roundTimeCT > 0)
                {
                    roundTimeCT -= Time.deltaTime * myIndependentTimeScale;
                    return;
                }

                //go roundTimeEnd buffer
                //if roundTimeEnd buffer is 0
                if (roundTimeEndBufferCT > 0)
                {
                    roundTimeEndBufferCT -= Time.deltaTime * myIndependentTimeScale;
                    return; 
                }                

                //go leftTime;
                //if leftTime is 0;
                if (leftTimeCT > 0)
                {
                    leftTimeCT -= Time.deltaTime * myIndependentTimeScale;
                    return; 
                }

                //go die buffer;
                //if die buffer is 0
                if (leftTimeDeadBufferCT > 0)
                {
                    leftTimeDeadBufferCT -= Time.deltaTime * myIndependentTimeScale;
                    return;
                }

                //go die
                PlayerTimeOut();
            }
        }
    }
    
    public void KillReward()
    {
        roundStartBufferCT = roundStartBufferTime /2 ;
        roundTimeEndBufferCT = roundTimeEndBufferTime;
        leftTimeDeadBufferCT = leftTimeDeadBufferTime;

        roundTimeCT = roundTime;
    }


    public void PlayerRoundCall()
    {
        //reset buffer
        roundStartBufferCT = roundStartBufferTime;
        roundTimeEndBufferCT = roundTimeEndBufferTime;
        leftTimeDeadBufferCT = leftTimeDeadBufferTime;

        roundTimeCT = roundTime;

        isCountingTime = true;  
    }
    public void PlayerRoundFinishCall()
    {
        isCountingTime = false;

        if (roundTimeCT > 0)
        {
            leftTimeCT += roundTimeCT;
            trickManager.GainEnergy(roundTimeCT * 10f);
        }
    }



    public void PlayerTimeOut()
    {
        //clog
        if (!playerDieClog)
        {
            playerDieClog = true;

            TSA_Player player = FindFirstObjectByType<TSA_Player>();
            //player left life = 0;
            player.myTroop.leftLife = 0;
            //let player die;
            roundManager.MakePlayerDie();
        }
    }
}
