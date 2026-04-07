using UnityEngine;

public class TSA_KarenBorn : TroopSpecialAbility
{
    public int myEnemyUID = 6;

    //public Troop myTroop;
    public SLS_Karen SLSK;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TSAInit();
        SLSK = FindAnyObjectByType<SLS_Karen>();
        myTroop.OnDieAction += InjectOnDie;
        myTroop.OnDieAction += DieStatInject;
    }
    public void InjectOnDie()
    {
        SLSK.OnKarenTroopDie();
    }

    public void DieStatInject()
    {
        EnemyDieStatAdd(myEnemyUID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
