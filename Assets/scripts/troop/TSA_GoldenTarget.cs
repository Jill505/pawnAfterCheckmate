using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class TSA_GoldenTarget : MonoBehaviour
{
    [Header("Ref Components")]
    public GameManager gameManager;
    public RoundManager roundManager;

    [Header("Calculate Variables")]
    public Vector2 StartPt = new Vector2();
    public HashSet<Vector2> PassPt = new HashSet<Vector2>();
    public List<Vector2> LinkKillList = new List<Vector2>();
    public List<Vector2> AttackRange = new List<Vector2>();
    
    public Troop myTroop;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
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
        
        if (DFS_RecursiveSearchPlayer(StartPt, AttackRange))
        {
            //玩家在範圍內，執行連殺
            Debug.Log("玩家在檢測範圍內");
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
}
