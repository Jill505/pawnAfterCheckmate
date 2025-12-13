using UnityEngine;

public class GameLobbyDecArea : MonoBehaviour
{
    public GameLobbyUIManager gameLobbyUIManager;
    public bool OnInspect;

    public Animator myAnimator;

    public GameLobbyUIElement myGameLobbyUIElementSort;

    bool allowDetect = true;

    private void Awake()
    {
        if (gameLobbyUIManager == null)
        {
            gameLobbyUIManager = FindFirstObjectByType<GameLobbyUIManager>();
        }
        allowDetect = true;
    }

    public void Start()
    {
        switch (myGameLobbyUIElementSort)
        {
            case GameLobbyUIElement.TrickSelect:
                myAnimator.Play("Lobby_Trick_UI_Idle", -1, 1);
                break;
            case GameLobbyUIElement.LevelInformation:
                myAnimator.Play("Lobby_LevelInformation_UI_Idle", -1, 1);
                break;
        }
    }
    private void Update()
    {
         if (gameLobbyUIManager.OnFocus)
        {
            allowDetect = false;
            //OnInspect = true;
            //OnInspectShow();
        }
        else
        {
            allowDetect = true;
        }
    }

    public void OnInspectShow()
    {
        myAnimator.SetBool("isActive", OnInspect);

    }
    public void OnInspectShow_On()
    {
        OnInspect = true;
        OnInspectShow();
    }
    public void OnInspectShow_Off()
    {
        OnInspect = false;
        OnInspectShow();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (allowDetect)
        {
            if (collision.CompareTag("GameLobbyCursorRef"))
            {
                OnInspectShow_On();
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("B");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (allowDetect)
        {
            Debug.Log("Tri Exit Tri");
            if (collision.CompareTag("GameLobbyCursorRef"))
            {
                OnInspectShow_Off();
            }
        }
    }
}

public enum GameLobbyUIElement
{
    TrickSelect,
    LevelInformation,
}