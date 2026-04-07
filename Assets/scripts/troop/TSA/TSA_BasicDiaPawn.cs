using UnityEngine;

public class TSA_BasicDiaPawn : TroopSpecialAbility
{
    public int myEnemyUID = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TSAInit();
        myTroop.OnDieAction += DieStatInject;
    }

    public void DieStatInject()
    {
        EnemyDieStatAdd(myEnemyUID);
    }
}
