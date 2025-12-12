using UnityEngine;

public class GameLobby_Door : MonoBehaviour
{
    [Header("Game Manager Ref")]
    public GameLobbyUIManager GLUIM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GLUIM == null)
        {
            GLUIM = FindAnyObjectByType<GameLobbyUIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (GLUIM.OnFocus == false)
        {
            Debug.Log("«ö¤U°Õ");
            GLUIM.OnFocusDoor();
        }
    }
}
