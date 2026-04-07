using UnityEngine;

public class TSA_Tank : TroopSpecialAbility
{
    public int myEnemyUID = 3;

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
