using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Mono.Cecil;
using Unity.VisualScripting;

public class Troop : MonoBehaviour
{
    public GameManager gameManager;
    public SO_Chess myChessData;
    public bool isPlayer = false;
    public RoundManager roundManager;
    public SoundManager soundManager;

    public Camp myCamp;

    public int surviveRound = 0;

    [Range(0, 9)]
    public int myNowX = 0;
    [Range(0, 9)]
    public int myNowY = 0;

    public int hp = 1;

    public SpriteRenderer mySr;

    public gear holdingGear = gear.noGear;
    public bool isPassable = false;
    public BucketType bucketType;

    [Header("棋子UI顯示")]
    public Text MySurviveRoundShowCase;

    [Header("能力")]
    public ability[] myAbilities;


    [Header("旗子移動數值")]
    public int horBlockMoveAbility = 0;
    public int verticalBlockMoveAbility = 0;
    public int diagonalBlockMoveAbility = 0;
    public int knightBlockMoveAbility = 0;
    public int AttackStr = 1;

    [Header("盾牌")]
    public bool hasUpperShield = false;
    public bool hasLowerShield = false;
    public bool hasLeftShield = false;  
    public bool hasRightShield = false;

    public void LoadSOData()
    {
        hp = myChessData.hp;

        mySr.sprite = myChessData.skin;

        myCamp = myChessData.myCamp;
        holdingGear = myChessData.spawnGear;
        bucketType = myChessData.bucketType;

        horBlockMoveAbility = myChessData.horBlockMoveAbility;
        verticalBlockMoveAbility = myChessData.verticalBlockMoveAbility;
        diagonalBlockMoveAbility = myChessData.diagonalBlockMoveAbility;
        knightBlockMoveAbility = myChessData.knightBlockMoveAbility;

        AttackStr = myChessData.AttackStr;

        myAbilities = myChessData.abilities;
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
        soundManager = FindFirstObjectByType<SoundManager>();
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
        myUISync();
    }
    public void myPosSync()
    {
        if (gameManager.chessBoardObjectRefArr.GetLength(0) > myNowX && gameManager.chessBoardObjectRefArr.GetLength(1) > myNowY && myNowX >= 0 && myNowY >= 0)
        {
            Vector2 vec = gameManager.chessBoardObjectRefArr[myNowY, myNowX].transform.position;
            transform.position = vec;
        }
    }

    public void myUISync()
    {
        if (myCamp == Camp.Player)
        {
            MySurviveRoundShowCase.text = "";
        }
        else
        {
            if (MySurviveRoundShowCase != null)
            {
                MySurviveRoundShowCase.text = surviveRound + "";
            }
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
            roundManager.specialClogAutoSelectionClog = true;
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

        //Play sound effect
        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");

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
        //For Mob Reduce
        ReduceOnSelectChessAllowMoveVector();
    }
    public void ReduceOnSelectChessAllowMoveVector()
    {
        for (int i = OnSelectChessAllowMoveVector.Count - 1; i >= 0; i--)
        {
            // 先把目前要比的目標存起來，避免移除後再用索引取值
            var target = OnSelectChessAllowMoveVector[i];

            foreach (var obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj == null) continue;
                if (!obj.TryGetComponent<unit>(out unit u)) continue;
                if (u.TroopsOnMe == null) continue;//二次檢查

                if (u.TroopsOnMe.myCamp == Camp.Player) continue;

                // 建議整個專案改用 Vector2Int；若現在列表是 Vector2，可先 RoundToInt
                // 這段GPT改的
                var pos = new Vector2Int(u.myX, u.myY);
                if (pos == Vector2Int.RoundToInt(target) )
                {
                    OnSelectChessAllowMoveVector.RemoveAt(i);
                    break; // 避免在同一個 i 上繼續讀取已被縮短的 List
                }
            }
        }

        /*for (int i = OnSelectChessAllowMoveVector.Count - 1; i >= 0; i--)
        {
            foreach (GameObject obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj.GetComponent<unit>().TroopsOnMe != null && new Vector2(obj.GetComponent<unit>().myX, obj.GetComponent<unit>().myY) == OnSelectChessAllowMoveVector[i])
                {
                    OnSelectChessAllowMoveVector.Remove(OnSelectChessAllowMoveVector[i]);
                }
            }
        }*/
    }
    public void EnemyOnMouseDownEvent(unit tarUnit) //此方法與unit.cs中的PlayerOnMouseDownEvent相似 修改時請考慮到另外一邊
    {
        //Debug.Log("tarUnit xy - " + tarUnit.myX + "/" + tarUnit.myY);
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
                    roundManager.SelectObjectTroop = this;
                    roundManager.SelectObjectTroop.holdingGear = tarUnit.TroopsOnMe.holdingGear;
                    gameManager.hintManager.SpawnHintWordPrefab("MOB 得到道具 - " + tarUnit.TroopsOnMe.myChessData.chessName);
                    roundManager.SelectObjectTroop = null;
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
        for (int i = 1; T.horBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y));
        }

        for (int i = 1; T.verticalBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y - i));
        }

        for (int i = 1; T.diagonalBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y - i));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y - i));
        }

        for (int i = 1; T.knightBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x - 1, currentXY.y + 2));
            Vec2List.Add(new Vector2(currentXY.x + 1, currentXY.y + 2));
            Vec2List.Add(new Vector2(currentXY.x - 2, currentXY.y + 1));
            Vec2List.Add(new Vector2(currentXY.x - 2, currentXY.y + -1));
            Vec2List.Add(new Vector2(currentXY.x + 2, currentXY.y + 1));
            Vec2List.Add(new Vector2(currentXY.x + 2, currentXY.y - 1));
            Vec2List.Add(new Vector2(currentXY.x + 1, currentXY.y - 2));
            Vec2List.Add(new Vector2(currentXY.x - 1, currentXY.y - 2));
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

        GeneralReduceRule(Vec2List, T);
    }
    public void GeneralReduceRule(List<Vector2> Vec2List, Troop T)
    {
        Debug.Log("學士路東觸發");
        //shield reduce - 上盾牌 抵擋來自上方的攻擊 即檢測玩家正下方地塊目標
        #region 上盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測為玩家下方, 即x相同, y小於的區塊, 這個區域內的上盾牌可以發揮作用
            if (Vec2List[i].x == T.myNowX && Vec2List[i].y < T.myNowY)
            {
                //檢測該座標是否存在
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasUpperShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 上");
                }
            }
        }
        #endregion

        //shield reduce - 下盾牌 抵擋來自下方的攻擊 即檢測玩家正上方地塊目標
        #region 下盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測為玩家下方, 即x相同, y小於的區塊, 這個區域內的上盾牌可以發揮作用
            if (Vec2List[i].x == T.myNowX && Vec2List[i].y > T.myNowY)
            {
                //檢測該座標是否存在
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLowerShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 下");
                }
            }
        }
        #endregion

        //shield reduce - 左盾牌 抵擋來自左方的攻擊 即檢測玩家正右方地塊目標
        #region 左盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            if (Vec2List[i].x > T.myNowX && Vec2List[i].y == T.myNowY)
            {
                //檢測該座標是否存在
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLeftShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 左");
                }
            }
        }
        #endregion
        #region 右盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            if (Vec2List[i].x < T.myNowX && Vec2List[i].y == T.myNowY)
            {
                //檢測該座標是否存在
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasRightShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 右");
                }
            }
        }
        #endregion
    }

    public void PlayerDieReaction() //千萬別從這個腳本直接呼叫！！
    {
        gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void EnemyEvo()
    {
        if (myCamp == Camp.Enemy)
        {
            for (int i = 0; i < myAbilities.Length; i++)
            {
                EvoFunction(myAbilities[i]);
            }
        }
    }
    public void EvoFunction(ability abi)
    {
        switch (abi)
        {
            case ability.evo_HorMoveAbility:
                horBlockMoveAbility++;
                break;


            case ability.evo_VarMoveAbility:
                verticalBlockMoveAbility++;
                break;


            case ability.evo_XMoveAbility:
                diagonalBlockMoveAbility++;
                break;
        }
    }
}


