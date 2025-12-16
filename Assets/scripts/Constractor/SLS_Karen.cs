using UnityEngine;

public class SLS_Karen: SpecialLevelScript
{
    [Header("Manager Refs")]
    public SO_Level sO_Level;
    public GameManager gameManager;
    public RoundManager roundManager;

    public int bossHealth = 3;

    public SO_Chess KarenSpawnTroop;

    public void Start()
    {
          
    }
    public override void DO_GameInit()
    {
        GameInit();
    }
    public void SpawnKarenTroop()
    {
        Debug.Log("Spawn Karen Troop Call");
        roundManager.RandomSpawnEnemy(KarenSpawnTroop, false);
    }
    public void OnKarenTroopDie()
    {
        bossHealth -= 1;
        if (bossHealth <= 0)
        {
            //boss die, player win.
            roundManager.Win();
        }
        else
        {
            //Spawn another troop
            SpawnKarenTroop();
        }
    }

    public void GameInit()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        roundManager = FindAnyObjectByType<RoundManager>();

        KarenSpawnTroop = sO_Level.special_SpawnChessData[0];

        bossHealth = 3;
        SpawnKarenTroop();
    }
    public override string LevelTargetString()
    {
        return "±þ¦º¥Ø¼Ð";
    }
}
