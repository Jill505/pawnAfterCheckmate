using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TSA_GoldenTarget : TroopSpecialAbility
{
    public int myEnemyUID = 2;

    [Header("Ref Components")]
    public GameManager gameManager;
    public RoundManager roundManager;
    //public Troop myTroop;

    [Header("Calculate Variables")]
    public Vector2 StartPt = new Vector2();
    public HashSet<Vector2> PassPt = new HashSet<Vector2>();
    public List<Vector2> LinkKillList = new List<Vector2>();
    public List<Vector2> AttackRange = new List<Vector2>();
    

    void Start()
    {
        TSAInit();
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
        myTroop.isGoldenTarget = true;
        myTroop.OnDieAction += DieStatInject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CalculateAttackPlayerPath();
        }
    }

    public void CalculateAttackPlayerPath()
    {
        PassPt.Clear();
        LinkKillList.Clear();
        AttackRange.Clear();

        myTroop.EnemyCalculateAttackRange();

        for (int i = 1; i <= myTroop.horBlockMoveAbility; i++)
        {
            AttackRange.Add(new Vector2(i, 0));
            AttackRange.Add(new Vector2(-i, 0));
        }

        for (int i = 1; i <= myTroop.verticalBlockMoveAbility; i++)
        {
            AttackRange.Add(new Vector2(0, i));
            AttackRange.Add(new Vector2(0, -i));
        }

        for (int i = 1; i <= myTroop.diagonalBlockMoveAbility; i++)
        {
            AttackRange.Add(new Vector2(i, i));
            AttackRange.Add(new Vector2(i, -i));
            AttackRange.Add(new Vector2(-i, i));
            AttackRange.Add(new Vector2(-i, -i));
        }


        for (int i = 1; i <= myTroop.knightBlockMoveAbility; i++)
        {
            AttackRange.Add(new Vector2(-1, +2));
            AttackRange.Add(new Vector2(1, 2));
            AttackRange.Add(new Vector2(-2,1));
            AttackRange.Add(new Vector2(-2, -1));
            AttackRange.Add(new Vector2(2,1));
            AttackRange.Add(new Vector2(2,-1));
            AttackRange.Add(new Vector2(1,-2));
            AttackRange.Add(new Vector2(-1,-2));
        }


        StartPt = new Vector2(myTroop.myNowX, myTroop.myNowY);
        PassPt.Add(new Vector2((int)myTroop.myNowX, (int)myTroop.myNowY));
        
        if (DFS_RecursiveSearchPlayer(StartPt, AttackRange))
        {
            //玩家在範圍內，執行連殺
            Debug.Log("玩家在檢測範圍內");
            int n = 0;
            foreach (Vector2 vec in LinkKillList)
            {
                Debug.Log("第" + n + "個目標：" + vec);
                n++;
            }
        }
        else
        {
            //玩家不在範圍內，執行普通移動
            Debug.Log("玩家不在檢測範圍內");
        }
    }

    public bool DFS_RecursiveSearchPlayer(Vector2 RefPt, List<Vector2> atkRange)
    {
        foreach (Vector2 atkVec in atkRange)
        {
            Vector2 checkVec = RefPt + atkVec;

            if (!gameManager.isVectorLegal(checkVec)) continue;

            if (gameManager.GetUnitAt((int)checkVec.x, (int)checkVec.y).TroopsOnMe == null) continue;

            if (PassPt.Contains(checkVec))
            {
                //pass
                continue;
            }
            else
            {
                PassPt.Add(checkVec);
                if (gameManager.GetUnitAt((int)checkVec.x, (int)checkVec.y).TroopsOnMe.isPlayer == true)
                {
                    Troop checkT = gameManager.GetUnitAt((int)checkVec.x, (int)checkVec.y).TroopsOnMe;
                    //Check if it is legal from the spot
                    //目標在左側並且檢測擁有右盾牌
                    if (RefPt.x > checkT.myNowX && RefPt.y == checkT.myNowY && checkT.hasRightShield)
                    {
                        continue;
                    }
                    //目標在右側並且檢測擁有左盾牌
                    if (RefPt.x < checkT.myNowX && RefPt.y == checkT.myNowY && checkT.hasLeftShield)
                    {
                        continue;
                    }
                    //目標在下方並且檢測擁有上盾牌
                    if (RefPt.x == checkT.myNowX && RefPt.y > checkT.myNowY && checkT.hasUpperShield)
                    {
                        continue;
                    }
                    //目標在上方並且檢測擁有下盾牌
                    if (RefPt.x == checkT.myNowX && RefPt.y < checkT.myNowY && checkT.hasLowerShield)
                    {
                        continue;
                    }

                    LinkKillList.Add(checkVec);
                    return true;
                }
            }

            if (DFS_RecursiveSearchPlayer(checkVec, atkRange))
            {
                LinkKillList.Add(checkVec);
                return true;
            }

        }
        return false;
    }

    public void CycleKillCall()
    {
        StartCoroutine(CycleKillCoroutine());
    }
    public IEnumerator CycleKillCoroutine()
    {
        CalculateAttackPlayerPath();
        
        if (LinkKillList.Count > 0 )
        {
            //execute cycle kill
            for (int i = LinkKillList.Count - 1; i >= 0; i--)
            {
                //DO kill by List
                myTroop.EnemyOnMouseDownEvent(gameManager.GetUnitAt((int)LinkKillList[i].x, (int)LinkKillList[i].y));
                yield return new WaitForSeconds(0.6f);
            }
        }
        else
        {
            myTroop.EnemyAutoMoveToNext();
            roundManager.EnemyAnimationCoroutineEnd = true;
        }
        roundManager.EnemyAnimationCoroutineEnd = true;
        yield return null;
    }
    public void DieStatInject()
    {
        EnemyDieStatAdd(myEnemyUID);
    }
}
