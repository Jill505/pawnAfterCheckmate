
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Troop : MonoBehaviour
{
    public GameManager gameManager;
    public SO_Chess myChessData;
    public bool isPlayer = false;

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

        if (myCamp == Camp.Enemy)
        {
            RoundManager RM = FindAnyObjectByType<RoundManager>();
            RM.EnemyAITroop.Add(this);
        }

        gameManager.Troops.Remove(gameObject);
        Destroy(gameObject);
    }

    [Header("敵人邏輯宣告")]
    public List<Vector2> OnSelectChessAllowMoveVector;

    public Vector2 myNextDes;
    #region 敵人邏輯

    public void MoveToNext()
    {
        EnemyLogic();
        myNextDes = ClosestVector();
        myNowX = (int)myNextDes.x;
        myNowY = (int)myNextDes.y;
    }
    
    public Vector2 ClosestVector()
    {
        Vector2 tar = new Vector2();
        Vector2 compareVec = new Vector2(gameManager.Troops[0].GetComponent<Troop>().myNowX, gameManager.Troops[0].GetComponent<Troop>().myNowY);

        float d = 100000f;

        if (OnSelectChessAllowMoveVector.Count > 0)
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
        OnSelectChessAllowMoveVector.Clear();
        UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, this);
        ReduceOnSelectChessAllowMoveVector();
    }
    public void ReduceOnSelectChessAllowMoveVector()
    {
        foreach (Vector2 vec in OnSelectChessAllowMoveVector)
        {
            foreach (GameObject obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj.GetComponent<unit>().TroopsOnMe != null && new Vector2(obj.GetComponent<unit>().myX, obj.GetComponent<unit>().myY) == vec)
                {
                    OnSelectChessAllowMoveVector.Remove(vec);
                }
            }
        }
    }

    public void UpdateOnSelectChessAllowMoveVector(List<Vector2> Vec2List, Troop T) // 可移動地塊更新 重要函式！！！
    {
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
    #endregion
}

