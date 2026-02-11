using UnityEngine;

public class GameESCManager : MonoBehaviour
{
    public GameObject ESCCanvas;
    public RoundManager rm;

    public bool openState = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rm = FindFirstObjectByType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchESC();
        }
    }
    public void SwitchESC()
    {
        openState = !openState;
        ESCCanvas.SetActive(openState);
    }
    public void QuitInspectESCCanvas()
    {
        openState = false;
        ESCCanvas.SetActive(openState);
    }
    public void backToLobby()
    {
        rm.MakePlayerDie();
    }
}

