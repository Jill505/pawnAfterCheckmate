using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

public class TSA_Rager : MonoBehaviour
{
    public RoundManager roundManager;

    public int CountDown = 2;
    public Troop myTroop;

    public bool RageClog = false;


    public void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
    }

    public void Start()
    {
        myTroop.Action_OnRoundEnd += RoundProcess;
    }
    public void RageModeActive()
    {
        if (!RageClog)
        {
            //²¾°£Retard

            for (int i = 0; i < myTroop.myAbilities.Length; i++)
            {
                if (myTroop.myAbilities[i] == ability.Retard)
                {
                    int target = i;
                    ability[] swapAbilityArr = new ability[myTroop.myAbilities.Length - 1];
                    for (int j = 0, k = 0; j < myTroop.myAbilities.Length; j++)
                    {
                        if (j == i) continue;
                        swapAbilityArr[k++] = myTroop.myAbilities[j];
                    }

                    myTroop.myAbilities = swapAbilityArr;
                    break;
                }
            }

            RageClog = true;

            //Do rage
            myTroop.horBlockMoveAbility = 3;
            myTroop.verticalBlockMoveAbility = 3;
            myTroop.diagonalBlockMoveAbility = 2;
            myTroop.knightBlockMoveAbility = 1;

            //Skin Switch
            myTroop.mySr.sprite = Resources.Load<Sprite>("Chess/Scissors_1");
        }
    }

    public void RoundProcess()
    {
        CountDown--;

        if (CountDown <= 0)
        {
            RageModeActive();
        }
    }

    private void OnDestroy()
    {
        myTroop.Action_OnRoundEnd -= RoundProcess;
    }
}
