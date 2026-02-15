using UnityEngine;

public class TSA_Player : MonoBehaviour
{
    public Troop myTroop;

    public GameObject DieParticleVFX;
    
    void Start()
    {
        DieParticleVFX = Resources.Load<GameObject>("VFX/Misc/PlayerDieParticleVFX");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTalent()
    {
        //如果有第二條命 = ture
        myTroop.leftLife += 1;
    }
}
