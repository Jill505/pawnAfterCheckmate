using System.Collections;
using UnityEngine;

public class TSA_SuicideBomb : MonoBehaviour
{
    public int CountDown = 2;

    public RoundManager roundManager;
    public GameManager gameManager;

    public CameraManager cameraManager;
    public SoundManager soundManager;

    public Troop myTroop;

    public GameObject ExplodeHintGameObject;
    public Vector3 ExplodeHintGameObjectOffset;
    public bool hintExist = false;

    [Header("爆炸動畫")]
    public float ExplodeInterval = 0.05f;

    public void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        soundManager = FindAnyObjectByType<SoundManager>();

        ExplodeHintGameObject = Resources.Load<GameObject>("VFX/Misc/BombHintVFX");
    }

    public void Start()
    {
        myTroop.Action_OnRoundEnd += RoundProcess;
    }


    //自爆炸彈兵
    public IEnumerator DoSuicide()
    {
        Debug.Log("KA BOOM!");
        //環形檢查自己周圍九格的目標，若大於關卡陣列
        int myNX = myTroop.myNowX;
        int myNY = myTroop.myNowY;


        yield return new WaitForSeconds(0.1f);

        //提前把 myNX myNY當格的動畫給播了 => 直接播放 炸彈兵 動畫 //TODO
        myTroop.troopOutfit.Do_BombExplodeAnimation();
        soundManager.PlaySFX("bomb_everything");

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
                    StartCoroutine(CheckAndKill(myNX + i, myNY + j));
                    yield return new WaitForSeconds(ExplodeInterval);
                }
            }
        }

        roundManager.EnemyAnimationCoroutineEnd = true;
        cameraManager.Shake(0.8f);
        myTroop.killTroop(gameObject);
    }

    public IEnumerator CheckAndKill(int X, int Y)
    {
        if (X >= gameManager.levelData.gridSizeX || X < 0)
        {
            //SKIP
            yield break;
        }
        if (Y >= gameManager.levelData.gridSizeY || Y < 0)
        {
            //SKIP
            yield break;
        }

        //此格存在

        unit u = gameManager.chessBoardObjectRefArr[Y, X].GetComponent<unit>();
        Troop t = gameManager.chessBoardObjectRefArr[Y, X].GetComponent<unit>().TroopsOnMe;

        //TODO => 該格新增 爆炸動畫 => 要去UnitOutfit插入爆炸動畫相關生成代碼 
        u.myUnitOutfit.SpawnExplode();

        cameraManager.Shake(0.32f);
        soundManager.PlaySFX("bomb_only");

        yield return new WaitForSeconds(0.05f);

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

    public void SpawnExplodeHintVFXObject()
    {
        if (hintExist == false)
        {
            GameObject myVFXObject = Instantiate(ExplodeHintGameObject);
            myVFXObject.transform.SetParent(transform, false);
            myVFXObject.transform.localPosition = ExplodeHintGameObjectOffset;

            hintExist = true;   
        }
    }

    public void RoundProcess()
    {
        CountDown--;
        
        if (CountDown <= 1)
        {
            SpawnExplodeHintVFXObject();
        }

        if (CountDown <= 0)
        {
            roundManager.EnemyAnimationCoroutineEnd = false;
            StartCoroutine(DoSuicide());
        }
    }
    private void OnDestroy()
    {
        myTroop.Action_OnRoundEnd -= RoundProcess;
    }
}
