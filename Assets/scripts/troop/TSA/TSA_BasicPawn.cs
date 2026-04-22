using UnityEngine;

public class TSA_BasicPawn : TroopSpecialAbility
{
    public int myEnemyUID = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TSAInit();
        myTroop.OnDieAction += DieStatInject;
    }

    public void DieStatInject()
    {
        EnemyDieStatAdd(myEnemyUID);
        DieReport(myEnemyUID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
