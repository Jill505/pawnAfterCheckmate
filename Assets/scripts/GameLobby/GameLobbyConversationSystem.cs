using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyConversationSystem : MonoBehaviour
{
    [Header("UI Element")]
    public Text TextShowcase;
    public Button GameConversationButton;

    public GameObject ConversationCanvas;
    public GameLobbyManager GLM;

    public float wordInterval = 0.15f;
    string[] nowLoading;

    bool onLoading = false;
    bool onActive = false;

    public bool fastFinReadingTrigger;

    public bool readTheNextLineTrigger;

    [Header("Conversation System Object")]
    public List<GameObject> SpawnSprites;
    public GameObject SpawnSpritePrefab;

    [Header("Buffer")]
    public Sprite[] bufferSprites;

    private void Awake()
    {
        GLM = FindAnyObjectByType<GameLobbyManager>();
    }

    public void Update()
    {
        if (onActive)
        {
            ClickReader();
        }

        if (GLM.LevelClickableObjectList.Count > 0)
        {
            GameConversationButton.interactable= true;
        }
        else
        {
            GameConversationButton.interactable = false;
        }
    }

    public void StartConversationButton()
    {
        StartConversation(GLM.LevelClickableObjectList[0].GetComponent<LobbyClickableObject>().mySO_LCO);
    }
    public void StartConversation(SO_LobbyClickableObject SO_LCO)
    {
        nowLoading = SO_LCO.Narratives;
        bufferSprites = SO_LCO.Sprites;

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

            if (nowLoading[i].Contains("Command/"))
            {
                TextBoxCommand(nowLoading[i]);
                readTheNextLineTrigger = true;
                onLoading = false;
            }
            else
            {

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
            }
            onLoading = false;  
            yield return new WaitUntil(()=> readTheNextLineTrigger == true);
            readTheNextLineTrigger = false;
        }

        yield return null;
        onActive = false;
        CanvasOff();
    }

    /// <summary>
    /// 莱赣
    /// Command/よ猭/把计
    /// </summary>
    /// <param name="Command"> セō 莱宽碻 Command/よ猭/把计 砏玥 </param>
    public void TextBoxCommand(string Command)
    {
        string[] parts = Command.Split('/');

        if (parts.Length < 3)
        {
            Debug.LogError("獶猭");
            return;
        }

        switch (parts[1])
        {
            case "SpriteCreate":
                if (parts.Length < 3)
                {
                    Debug.LogError("把计岿粇");
                    Debug.Break();
                    return;
                }

                if (parts.Length == 3) 
                {
                    SpriteCreate(int.Parse(parts[2]));
                }

                if (parts.Length == 5)
                {
                    SpriteCreate(int.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                }

                if (parts.Length == 7)
                {
                    SpriteCreate(int.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
                }
                break;

            case "CleanSprite":
                if (parts.Length < 3) {
                    return;
                }
                SpriteClean(parts[2]);
                break;

        }
    }
    public void SpriteCreate(int spriteIndex)
    {
        GameObject obj = Instantiate(SpawnSpritePrefab, new Vector3(960f, 540f, 0), Quaternion.identity);
        SpawnSprites.Add(obj);
        obj.transform.SetParent(ConversationCanvas.transform);
        //obj.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
        obj.GetComponent<Image>().sprite = bufferSprites[spriteIndex];
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1920f, 1080f);
    }

    public void SpriteCreate(int spriteIndex, float x, float y)
    {
        GameObject obj = Instantiate(SpawnSpritePrefab, new Vector3(x, y, 0), Quaternion.identity);
        SpawnSprites.Add(obj);
        obj.transform.SetParent(ConversationCanvas.transform);
        //obj.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
        obj.GetComponent<Image>().sprite = bufferSprites[spriteIndex];
    }
    public void SpriteCreate(int spriteIndex, float x, float y, float pictureX, float pictureY)
    {
        GameObject obj = Instantiate(SpawnSpritePrefab, new Vector3(x, y, 0), Quaternion.identity);
        SpawnSprites.Add(obj);
        obj.transform.SetParent(ConversationCanvas.transform);
        //obj.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
        obj.GetComponent<Image>().sprite = bufferSprites[spriteIndex];
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(pictureX, pictureY);
        //obj.GetComponent<RectTransform>().rect.width = pictureX;
        //obj.GetComponent<RectTransform>().rect.height = pictureY;
    }
    public void SpriteClean(string argument)
    {
        if (argument == "All")
        {
            foreach (GameObject obj in SpawnSprites)
            {
                Destroy(obj);
            }
        }
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
