using UnityEngine;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{
    public Vector2 spawnArea_HitHint_A_LB;
    public Vector2 spawnArea_HitHint_A_RT;
    
    public Vector2 spawnArea_HitHint_B_LB;
    public Vector2 spawnArea_HitHint_B_RT;

    public GameObject HitHintGameObject;
    public Sprite[] hitHintGameObjectSprites;
    
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

        GameObject SpawnObj =Instantiate(HitHintGameObject, currentSpawnSpot, Quaternion.identity);
        float ramdomRotation = Random.Range(-27f, 27f);
        SpawnObj.GetComponent<SpriteRenderer>().sprite = hitHintGameObjectSprites[currentIndex];
        SpawnObj.transform.rotation = Quaternion.Euler(0,0, ramdomRotation);
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
