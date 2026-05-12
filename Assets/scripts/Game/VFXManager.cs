using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{
    public Vector2 spawnArea_HitHint_A_LB;
    public Vector2 spawnArea_HitHint_A_RT;
    
    public Vector2 spawnArea_HitHint_B_LB;
    public Vector2 spawnArea_HitHint_B_RT;

    public GameObject HitHintGameObject;
    public Sprite[] hitHintGameObjectSprites;

    public GameObject SlashObject_L;
    public GameObject SlashObject_R;

    public GameObject SlashKillVFX;
    public Kill_Direction currentKillDirection;
    
    public GameObject GameEndCanvasCarrier;
    public Animator GameEndAnimator;
    public Image GameEndCanvasBg_L;
    public Image GameEndCanvasBg_R;

    public GameObject GameEndCanvasMaskCarrier;
    public GameObject GameEndCanvasReverser;

    public float playerAttackEventDirection = 0f;
    
    public void SpawnHintGameObject(int ComboNum)
    {
        int currentIndex = 0;
        currentIndex = ComboNum - 1;
        
        if (hitHintGameObjectSprites.Length <= 0)
        {
            Debug.LogError("AKERR - 特效物件缺失");
            Debug.Break();
            return;
        }

        if (currentIndex > (hitHintGameObjectSprites.Length - 1))
        {
            currentIndex = (hitHintGameObjectSprites.Length - 1);
        }

        int ranSpot = UnityEngine.Random.Range(0,1);
        Vector2 currentSpawnSpot = new Vector2() ;
        if (ranSpot == 0)
        {
            currentSpawnSpot = new Vector2(UnityEngine.Random.Range(spawnArea_HitHint_A_LB.x, spawnArea_HitHint_A_RT.x), UnityEngine.Random.Range(spawnArea_HitHint_A_LB.y, spawnArea_HitHint_A_RT.y));
        }
        else
        {
            currentSpawnSpot = new Vector2(UnityEngine.Random.Range(spawnArea_HitHint_B_LB.x, spawnArea_HitHint_B_RT.x), UnityEngine.Random.Range(spawnArea_HitHint_B_LB.y, spawnArea_HitHint_B_RT.y));
        }

        currentSpawnSpot = new Vector2(8.09f, -1.94f);

        GameObject SpawnObj =Instantiate(HitHintGameObject, currentSpawnSpot, Quaternion.identity);
        float ramdomRotation = Random.Range(-27f, 27f);
        SpawnObj.GetComponent<SpriteRenderer>().sprite = hitHintGameObjectSprites[currentIndex];
        SpawnObj.transform.rotation = Quaternion.Euler(0,0, ramdomRotation);
    }

    public void VFX_SlashInHalf(Troop targetTroop)
    {
        //get sprite
        Sprite targetSprite = targetTroop.mySr.sprite;

        GameObject swapL = Instantiate(SlashObject_L, targetTroop.transform.position, Quaternion.identity);
        GameObject swapR = Instantiate(SlashObject_R, targetTroop.transform.position, Quaternion.identity);

        swapL.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite=targetSprite;
        swapR.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite=targetSprite;

        //Set Random Angle;
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        swapL.transform.rotation = Quaternion.Euler(0,0,randomAngle);
        swapR.transform.rotation = Quaternion.Euler(0,0,randomAngle);
        swapL.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -1 * randomAngle);
        swapR.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -1 * randomAngle);

        //Get random force
        Rigidbody2D swapLRb = swapL.GetComponent<Rigidbody2D>();
        Rigidbody2D swapRRb = swapR.GetComponent<Rigidbody2D>();

        // Add Force
        swapLRb.AddForce(
            new Vector2(Random.Range(-10f, 10f), Random.Range(2f, 20f)),
            ForceMode2D.Impulse
        );

        swapRRb.AddForce(
            new Vector2(Random.Range(-10f, 10f), Random.Range(2f, 20f)),
            ForceMode2D.Impulse
        );

        //Add random Angular momentum
        swapLRb.angularVelocity = Random.Range(-720f, 720f);
        swapRRb.angularVelocity = Random.Range(-720f, 720f);

        Destroy(swapL, 5f);
        Destroy(swapR, 5f);
    }

    public void VFX_Slash(float direction, Vector2 position, bool isBlack = false)
    {
        direction += Random.Range(-5f, 5f); //wiggle

        Instantiate(SlashKillVFX, position, Quaternion.Euler(0,0,direction));

        Kill_Direction KD = SlashKillVFX.GetComponent<Kill_Direction>();

        GameEndCanvasCarrier.transform.rotation = Quaternion.Euler(0,0,direction + 90f);
        GameEndCanvasCarrier.transform.position = position;

        GameEndCanvasReverser.transform.rotation = Quaternion.Euler(0, 0, -direction - 90f);

        if (KD == null)
        {
            Debug.Log("KD null");
        }
        else
        {
            Debug.Log("KD not null");
        }

        if (isBlack)
        {
            Debug.Log("Triggered, black");
            KD.mainSlashEffect.color = Color.black;
            KD.slashEffectLine.color = Color.black;
        }
        else
        {
            Debug.Log("Triggered, not black");
            KD.mainSlashEffect.color = Color.white;
            KD.slashEffectLine.color = Color.white;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
