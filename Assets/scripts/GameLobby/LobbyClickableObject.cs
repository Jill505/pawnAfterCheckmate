using UnityEngine;

public class LobbyClickableObject : MonoBehaviour
{
    public GameLobbyConversationSystem GLCS;

    public SO_LobbyClickableObject mySO_LCO;
    
    public SpriteRenderer mySr;
    public Sprite mySprite;
    
    public void Start()
    {
        GLCS = FindAnyObjectByType<GameLobbyConversationSystem>();
    }

    public void OnMouseDown()
    {
        //Trigger it
        GLCS.StartConversation(mySO_LCO);
    }

    public void InitMySelf()
    {
        if (mySO_LCO == null)
        {
            Debug.LogError("存在不合法物件");
            return;
        }

        Vector2 myPos = new Vector2(mySO_LCO.myObjectX, mySO_LCO.myObjectY);
        gameObject.transform.position = myPos;

        mySr.sprite = mySO_LCO.myObjectSprite;


        // ===== 新增 Collider2D =====
        BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }

        // 取得 sprite 實際大小（世界單位）
        Vector2 spriteSize = mySr.sprite.bounds.size;

        col.size = spriteSize;
        col.offset = Vector2.zero; // 對齊中心
    }
}
