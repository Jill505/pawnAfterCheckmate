using UnityEngine;

public class TSA_Player : MonoBehaviour
{
    public Troop myTroop;
    public GameObject DieParticleVFX;

    public PlayerSecLieVFX PSLVFX;

    void Start()
    {
        DieParticleVFX = Resources.Load<GameObject>("VFX/Misc/PlayerDieParticleVFX");
    }

    // Update is called once per frame
    void Update()
    {
        
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
