using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SLS_Karen: SpecialLevelScript
{
    [Header("Manager Refs")]
    public SO_Level sO_Level;
    public GameManager gameManager;
    public RoundManager roundManager;
    public CameraManager cameraManager;
    public SoundManager soundManager;

    public int bossHealth = 3;
    int _bossMaxHealth;

    public SO_Chess KarenSpawnTroop;

    public GameObject KarenObject;

    public GameObject KarenCanvas;

    public Animator KarenAnimator;
    public Image KarenBlood;
    public Text KarenBloodText;

    public void Start()
    {
        _bossMaxHealth = bossHealth;
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
        KarenBlood.fillAmount = (float)((float)bossHealth / (float)_bossMaxHealth);
        KarenBloodText.text = bossHealth + " / " + _bossMaxHealth;
        KarenAnimator.SetTrigger("Injurd");

        soundManager.PlaySFX("Karen_damage_1");
        soundManager.PlaySFX("Karen_damage_3");
        soundManager.PlaySFX("Karen_damage_3");
        soundManager.PlaySFX("Karen_damage_4");

        if (bossHealth <= 0)
        {
            //boss die, player win.
            //roundManager.Win();
            StartCoroutine(swapWinCoroutine());
        }
        else
        {
            //Spawn another troop
            SpawnKarenTroop();
        }
    }

    public IEnumerator swapWinCoroutine()
    {
        //make thank play object = true 
        yield return new WaitForSeconds(1.8f);

        Instantiate(sO_Level.special_GameObjectData[2]);

        yield return new WaitForSeconds(0.6f);


        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        roundManager.Win();
    }

    public void GameInit()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        roundManager = FindAnyObjectByType<RoundManager>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        soundManager = FindAnyObjectByType<SoundManager>();

        KarenSpawnTroop = sO_Level.special_SpawnChessData[0];
        KarenObject = sO_Level.special_GameObjectData[0];
        KarenCanvas = sO_Level.special_GameObjectData[1];

        bossHealth = 3;
        SpawnKarenTroop();

        //Update Camera Information
        CameraPosition CP = new CameraPosition();
        CP.position = new Vector3(0, -0.58f, -10.22f);
        cameraManager.cameraConstPosition = CP;

        GameObject KO = Instantiate(KarenObject, new Vector3(0,4.54f,0), Quaternion.identity);
        KarenAnimator = KO.GetComponent<Animator>();

        GameObject KC = Instantiate(KarenCanvas);
        KarenBlood = GameObject.Find("KarenBloodImage").GetComponent<Image>();
        KarenBloodText = GameObject.Find("KarenBloodText").GetComponent<Text>();
    }
    public override string LevelTargetString()
    {
        return "±þ¦º³Í­Ûªº¥l³êª«";
    }
}
