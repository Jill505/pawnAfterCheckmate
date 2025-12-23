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
        roundManager.Action_OnRoundEnd += RoundProcess;
    }
    public void RageModeActive()
    {
        if (!RageClog)
        {
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
        roundManager.Action_OnRoundEnd -= RoundProcess;
    }
}
