using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyConversationSystem : MonoBehaviour
{
    [Header("UI Element")]
    public Text TextShowcase;

    public GameObject ConversationCanvas;

    public float wordInterval = 0.15f;
    string[] nowLoading;

    bool onLoading = false;
    bool onActive = false;

    public bool fastFinReadingTrigger;

    public bool readTheNextLineTrigger;

    public void Update()
    {
        if (onActive)
        {
            ClickReader();
        }
    }

    public void StartConversation(SO_LobbyClickableObject SO_LCO)
    {
        nowLoading = SO_LCO.Narratives;
        StartCoroutine(StartConversationCoroutine());
    }

    public IEnumerator StartConversationCoroutine()
    {
        CanvasOn();
        onActive = true;
        for (int i = 0; i < nowLoading.Length; i++)
        {
            onLoading = true;
            TextShowcase.text = "";


            for (int j = 0; j < nowLoading[i].Length; j++)
            {
                TextShowcase.text += nowLoading[i][j];
                if (fastFinReadingTrigger)
                {
                    fastFinReadingTrigger = false;
                    TextShowcase.text = nowLoading[i];
                    break;
                }
                yield return new WaitForSeconds(wordInterval);
            }

            onLoading = false;  
            yield return new WaitUntil(()=> readTheNextLineTrigger == true);
            readTheNextLineTrigger = false;
        }

        yield return null;
        onActive = false;
        CanvasOff();
    }
    public void ReadNextLineCall()
    {
        readTheNextLineTrigger = true;
    }

    public void CanvasOn()
    {
        ConversationCanvas.SetActive (true);
    }
    public void CanvasOff()
    {
        ConversationCanvas.SetActive(false);
    }

    public void ClickReader()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (onLoading)
            {
                fastFinReadingTrigger = true;
            }
            else
            {
                readTheNextLineTrigger = true;
            }
        }
    }
}
