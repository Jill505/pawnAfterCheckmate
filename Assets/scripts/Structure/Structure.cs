using System;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public SO_Structure mySO_S;

    public StructureOutFit myOutFit;
    public SoundManager soundManager;
    public GameManager gameManager;
    public RoundManager roundManager;

    public unit myUnit;

    public StructureAbility[] myAbilities;
    public bool isAllowStanding = false;
    public bool isAttackableTarget = false;
    public bool isAllowBeingDestroyByEnergyHighState;

    public Action ActionOnRoundEnd = () => { };
    public Action OnRoundEnd = () => { };

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
    
    }
    public void SyncMyPositionToUnit()
    {
        transform.position = myUnit.transform.position;
    }

    public void LoadSO_Structure()
    {
        if (mySO_S == null)
        {
            Debug.LogWarning("SO_Structure п╩ев");
            return;
        }

        myAbilities = mySO_S.myAbilities;

        isAllowStanding = mySO_S.isAllowStanding;
        isAttackableTarget = mySO_S.isAttackableTarget;
        isAllowBeingDestroyByEnergyHighState = mySO_S.isAllowBeingDestroyByEnergyHighState;

        LoadStructureAbility();

        myOutFit.mySR.sprite = mySO_S.myStructureSprite;
    }

    public void LoadStructureAbility()
    {
        for (int i = 0; i < myAbilities.Length; i++)
        {
            ApplyStructAbility(myAbilities[i]);
        }
    }

    public void ApplyStructAbility(StructureAbility SA)
    {
        switch (SA)
        {

        }
    }

    public void DestroyStructure()
    {
        //Maybe consider to add VFX here?
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (gameManager.InGameStructureList.Contains(this))
        {
            gameManager.InGameStructureList.Remove(this);
        }
    }
}

public enum StructureAbility
{
    UnPassable,

    EnergyHighStructure
}