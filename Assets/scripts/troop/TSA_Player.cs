using UnityEngine;

public class TSA_Player : MonoBehaviour
{
    public Troop myTroop;
    public GameObject DieParticleVFX;

    public PlayerSecLieVFX PSLVFX;



    void Start()
    {
        DieParticleVFX = Resources.Load<GameObject>("VFX/Misc/PlayerDieParticleVFX");
        LoadPlayerSkill();
    }

    public void LoadPlayerSkill()
    {
        if (SaveSystem.SF.talentTreeUnlock[0])
        {
            myTroop.horBlockMoveAbility += 1;
            myTroop.verticalBlockMoveAbility += 1;
        }
        if (SaveSystem.SF.talentTreeUnlock[4])
        {
            myTroop.leftLife += 1;
        }
    }

    public void SpawnBlackMist()
    {
        PSLVFX = Instantiate(DieParticleVFX, transform.position, Quaternion.identity).GetComponent<PlayerSecLieVFX>();
    }

    public void KillBlackMist()
    {
        PSLVFX.KillAllParticles();
    }


    public void LoadTalent()
    {
        //如果有第二條命 = ture
        myTroop.leftLife += 1;
    }
}
