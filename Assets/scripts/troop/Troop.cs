
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class Troop : MonoBehaviour
{
    public GameManager gameManager;
    public SO_Chess myChessData;
    public bool isPlayer = false;
    public RoundManager roundManager;

    public Camp myCamp;

    [Range(0, 9)]
    public int myNowX = 0;
    [Range(0, 9)]
    public int myNowY = 0;

    public int hp = 1;

    public SpriteRenderer mySr;

    public gear holdingGear = gear.noGear;
    public bool isPassable = false;
    public BucketType bucketType;

    public void LoadSOData()
    {
        hp = myChessData.hp;

        mySr.sprite = myChessData.skin;

        myCamp = myChessData.myCamp;
        holdingGear = myChessData.spawnGear;
        bucketType = myChessData.bucketType;
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindAnyObjectByType<RoundManager>();
        LoadSOData();

        if (myCamp == Camp.Enemy)
        {
            RoundManager RM = FindAnyObjectByType<RoundManager>();
            RM.EnemyAITroop.Add(this);
        }
    }

    void Update()
    {
        myPosSync();
    }
    public void myPosSync()
    {
        if (gameManager.chessBoardObjectRefArr.GetLength(0) > myNowX && gameManager.chessBoardObjectRefArr.GetLength(1) > myNowY && myNowX >=0 && myNowY >=0)
        {
            Vector2 vec = gameManager.chessBoardObjectRefArr[myNowY, myNowX].transform.position;
            transform.position = vec;
        }
    }

    public void killTroop()
    {
        //TODO: 將自己從註冊表中移除
        if (myChessData.isGoldenTarget)
        {
            //Let Player Win.
            roundManager.Win();
        }

        if (myCamp == Camp.Enemy)
        {
            RoundManager RM = FindAnyObjectByType<RoundManager>();
            RM.EnemyAITroop.Remove(this);
        }

        gameManager.Troops.Remove(gameObject);
        Destroy(gameObject);
    }

    [Header("敵人邏輯宣告")]
    public List<Vector2> OnSelectChessAllowMoveVector = new List<Vector2>();

    public Vector2 myNextDes;
    #region 敵人邏輯

    public void MoveToNext()
    {
        EnemyLogic();
        myNextDes = ClosestVector();
        myNowX = (int)myNextDes.x;
        myNowY = (int)myNextDes.y;

        //傷害判定
        EnemyOnMouseDownEvent(gameManager.chessBoardObjectRefArr[myNowY, myNowX].GetComponent<unit>());
    }
    
    public Vector2 ClosestVector()
    {
        Vector2 tar = new Vector2();
        Vector2 compareVec = new Vector2(gameManager.Troops[0].GetComponent<Troop>().myNowX, gameManager.Troops[0].GetComponent<Troop>().myNowY);

        float d = 100000f;

        if (OnSelectChessAllowMoveVector.Count <= 0)
        {
            return new Vector2(-1, -1);
        }
        else
        {
            foreach (Vector2 vec in OnSelectChessAllowMoveVector)
            {
                if (Vector2.Distance(vec, compareVec) < d)
                {
                    d = Vector2.Distance(vec, compareVec);
                    tar = vec;
                }
            }
        }

        return tar; 
    }


    public void EnemyLogic()
    {
        Debug.Log("Enemy Logic Trigger");
        OnSelectChessAllowMoveVector.Clear();
        UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, this);
        ReduceOnSelectChessAllowMoveVector();
    }
    public void ReduceOnSelectChessAllowMoveVector()
    {
        for (int i = OnSelectChessAllowMoveVector.Count - 1; i >= 0; i--)
        {
            foreach (GameObject obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj.GetComponent<unit>().TroopsOnMe != null && new Vector2(obj.GetComponent<unit>().myX, obj.GetComponent<unit>().myY) == OnSelectChessAllowMoveVector[i])
                {
                    //OnSelectChessAllowMoveVector.Remove(OnSelectChessAllowMoveVector[i]);
                }
            }
        }
        
            /*
        foreach (Vector2 vec in OnSelectChessAllowMoveVector)
        {
            foreach (GameObject obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj.GetComponent<unit>().TroopsOnMe != null && new Vector2(obj.GetComponent<unit>().myX, obj.GetComponent<unit>().myY) == vec)
                {
                    OnSelectChessAllowMoveVector.Remove(vec);
                }
            }
        }*/
    }
    public void EnemyOnMouseDownEvent(unit tarUnit) //此方法與unit.cs中的PlayerOnMouseDownEvent相似 修改時請考慮到另外一邊
    {
        Debug.Log("tarUnit xy - " + tarUnit.myX + "/" + tarUnit.myY);
        if (tarUnit.TroopsOnMe == null)
        {
            //不攻擊純移動
        }
        else
        {

            switch (tarUnit.TroopsOnMe.myCamp)
            {
                case Camp.Enemy: //友方棋子 不該行動或是造成傷害

                    /*TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;
                    //若攻擊未殺死目標，則留在前一格
                    if (TroopsOnMe.hp <= 0)
                    {
                        //被殺死
                        roundManager.SelectObjectTroop.myNowX = myX;
                        roundManager.SelectObjectTroop.myNowY = myY;
                        TroopsOnMe.killTroop();

                        gameManager.hintManager.SpawnHintWordPrefab("擊破 - " + TroopsOnMe.myChessData.chessName);

                        //奪取道具？
                        if (TroopsOnMe.holdingGear != gear.noGear)
                        {
                            roundManager.SelectObjectTroop.holdingGear = TroopsOnMe.holdingGear;
                            gameManager.hintManager.SpawnHintWordPrefab("搶奪道具 - " + TroopsOnMe.holdingGear);
                        }
                    }
                    else
                    {
                        //沒被殺死
                        //先不管
                    }*/
                    break;

                case Camp.Bucket: //場地互動道具 如.爆破桶等
                    tarUnit.TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;
                    gameManager.hintManager.SpawnHintWordPrefab("MOB擊破桶子 - " + tarUnit.TroopsOnMe.myChessData.chessName);

                    //被殺死
                    tarUnit.TroopsOnMe.killTroop();

                    //依照種類觸發效果 記得可以合併到unit裡面觸發
                    switch (tarUnit.TroopsOnMe.bucketType)
                    {
                        case BucketType.noType:
                            Debug.Log("MOB 無事發生 歲月靜好 你選擇了一個Bucket類 但沒有選擇觸發效果");
                            break;

                    }
                    break;

                case Camp.Item: //道具

                    //被殺死
                    tarUnit.TroopsOnMe.killTroop();

                    roundManager.SelectObjectTroop.holdingGear = tarUnit.TroopsOnMe.holdingGear;
                    gameManager.hintManager.SpawnHintWordPrefab("MOB 得到道具 - " + tarUnit.TroopsOnMe.myChessData.chessName);
                    break;

                case Camp.Player:
                    //Kill Player
                    roundManager.MakePlayerDie();
                    break;
            }
        }
    }
    #endregion

    public void UpdateOnSelectChessAllowMoveVector(List<Vector2> Vec2List, Troop T) // 可移動地塊更新 重要函式！！！
    {
        Debug.Log(gameObject.name + "\n camp is:" + myCamp + "\nUpdateOnSelectChessAllowMoveVector Triggered.");
        Vec2List.Clear();
        //get current XY
        Vector2 currentXY = new Vector2(T.myNowX, T.myNowY);
        //Hor select cal
        for (int i = 1; T.myChessData.horBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y));
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y - i));
        }

        switch (T.holdingGear)
        {
            case gear.car:
                for (int i = 1; 8 >= i; i++)
                {
                    Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y));
                    Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y));
                    Vec2List.Add(new Vector2(currentXY.x, currentXY.y + i));
                    Vec2List.Add(new Vector2(currentXY.x, currentXY.y - i));
                }

                T.holdingGear = gear.noGear;
                break;
        }
    }

    public void PlayerDieReaction() //千萬別從這個腳本直接呼叫！！
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}

