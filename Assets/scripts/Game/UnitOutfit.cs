using UnityEngine;

public class UnitOutfit : MonoBehaviour
{
    public GameObject ExplodeGameObject;

    public unit myUnit;

    public SpriteRenderer mySr;
    public Sprite myOriginalSprite;
    public Sprite myHighLightSprite;

    public SpriteRenderer MyEnemyHighLightSR;
    public SpriteRenderer MySkillHighLightSR;

    private void Awake()
    {
        if (mySr == null)
        {
            mySr = gameObject.GetComponent<SpriteRenderer>();
        }
    }
    public void SpawnExplode()
    {
        Instantiate(ExplodeGameObject, transform.position, transform.rotation);
    }

    void Update()
    {
        SkinLogic();    
    }

    public void SkinLogic()
    {
        //敵人攻擊範圍顯示
        if (myUnit.isEnemyAttackHighLighting)
        {
            MyEnemyHighLightSR.gameObject.SetActive(true);
        }
        else
        {
            MyEnemyHighLightSR.gameObject.SetActive(false);
        }


        //邏輯冗雜 可以優化
        if (myUnit.isPlaceableTarget)
        {
            myUnit.isSkillPlacementHighLighting = true;
        }
        else
        {
            myUnit.isSkillPlacementHighLighting = false;
        }


        if (myUnit.isSkillPlacementHighLighting)
        {
            MySkillHighLightSR.gameObject.SetActive(true);
        }
        else
        {
            MySkillHighLightSR.gameObject.SetActive(false);
        }
    }
}
