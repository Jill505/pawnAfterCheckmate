using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    public bool isRequireEnergyHigh = false;

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
            Debug.LogWarning("SO_Structure 缺失");
            return;
        }

        myAbilities = mySO_S.myAbilities;

        isAllowStanding = mySO_S.isAllowStanding;
        isAttackableTarget = mySO_S.isAttackableTarget;
        isRequireEnergyHigh = mySO_S.isRequireEnergyHigh;

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
            case StructureAbility.EnergyHighStructure:
                EnergyHighSpot EHS =  gameObject.AddComponent<EnergyHighSpot>();
                myOutFit.mySR.sortingOrder = 6;
                EHS.myStructure = this;
                break;
        }
    }

    public bool isContainAbility(StructureAbility SA)
    {
        //Debug.Log("觸發檢測");
        for (int i = 0; i < myAbilities.Length; i++)
        {
            if (myAbilities[i] == SA)
            {
                //Debug.Log("返回檢測結果 _ 有");
                return true;
            }
        }
        //Debug.Log("返回檢測結果 _ 無");
        return false;
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