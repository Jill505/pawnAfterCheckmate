using UnityEngine;

public class EnergyHighSpot : MonoBehaviour
{
    public Structure myStructure;
    public RoundManager roundManager;

    public int SpawnCoolDown =3;
    public int CurrentCoolDown;

    public bool isHaveEnergy;

    public void Awake()
    {
        roundManager = FindFirstObjectByType<RoundManager>();
    }

    public void Start()
    {
        //Register Each Round pass function into the round finish actionj
        roundManager.Action_OnRoundEnd += EachRoundPass;
    }

    public void EachRoundPass()
    {
        CurrentCoolDown -=1 ;
        if (CurrentCoolDown <= 0)
        {
            CurrentCoolDown = 0 ;
            isHaveEnergy = true ;
        }
    }

    public void GiveTroopEnergyHigh(Troop t)
    {
        if (isHaveEnergy)
        {
            t.energyHigh = true;
            isHaveEnergy = false;
        }
    }

    public void OnDestroy()
    {
        //remove function form round fin action
        roundManager.Action_OnRoundEnd -= EachRoundPass;
    }
}
