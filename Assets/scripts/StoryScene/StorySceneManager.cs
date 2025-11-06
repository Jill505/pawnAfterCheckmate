using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StorySceneManager : MonoBehaviour
{
    public AKSO_Story onLoadAKSO_Story;

    public bool hasStoryLoad;

    public int readingPage;
    public int maxPage;
    public int loadSceneSort = 1;

    public bool onReadingClog =false;
    public float wordGenDur = 0.12f;

    public Text conversationShowcase;
    public string conversationShowingText;
    public string conversationText;

    public Coroutine ReadingCoroutine;

    public string[] storyText;
    public Animator myAnimator;

    public bool allowSkipSwitch;//設定控制的是這個！！
    bool allowSkipClog;

    [Header("全端接收站")]
    static public int ADR_LoadStorySort;
    static public bool ADR_IsStoryLoading = false;  

    public AKSO_Story[] RegisterAKSO_Story;
    public GameObject[] ASKO_Ref_StoryAnimatorObject;

    [Header("測試")]
    public bool isTesting = false;
    public int testScene = 0;

    private bool autoSkipClog;

    private void Awake()
    {
        if (isTesting)
        {
            ADR_IsStoryLoading  =true;
            ADR_LoadStorySort = testScene;

            Debug.Log("YESA");
        }
    }
    private void Start()
    {
        readingPage = -1;
        //ReceiveAKSO Message;

        if (ADR_IsStoryLoading)
        {
            ADR_IsStoryLoading = false;
            ReceiveAKSO_Story(RegisterAKSO_Story[ADR_LoadStorySort]);
            ASKO_Ref_StoryAnimatorObject[ADR_LoadStorySort].SetActive(true);

            NextPage();

            Debug.Log("YES");
        }
    }

    public void ReceiveAKSO_Story(AKSO_Story AKSOS)
    {
        loadSceneSort = AKSOS.toScene;
        readingPage = -1;
        maxPage = AKSOS.StoryLine.Length;

        storyText = AKSOS.StoryLine;
        myAnimator = AKSOS.myAnimator;

        hasStoryLoad= true;

        ADR_IsStoryLoading = false;
    }

    public void NextPage()
    {
        readingPage++;

        if (readingPage >= maxPage)
        {
            readingPage = maxPage;
            Debug.Log("故事播放完畢");
            TeleportToTargetScene();
            return;
        }

        //Debug.Log("載入 - " + storyText[readingPage]);
        if (ReadingCoroutine == null && autoSkipClog == false)
        {
            Debug.Log("開啟對話CLOG" + readingPage);
            ReadingCoroutine = StartCoroutine(ReadingLine(storyText[readingPage]));
        }
        else
        {
            autoSkipClog = false;
        }


        //if (onReadingClog) onReadingClog = false;
    }

    private void Update()
    {
        if (hasStoryLoad)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (onReadingClog)
                {
                    if (allowSkipSwitch)
                    {
                        if (allowSkipClog)
                        {
                            onReadingClog = false;
                        }
                    }
                }
                else
                {
                    NextPage();
                }
            }
            else
            {

            }
        }
        else
        {
           //Debug.Log("無載入中故事");
        }
    }

    public IEnumerator ReadingLine(string str)
    {
        bool selfDebugClog = false;

        conversationText = str;
        conversationShowingText = "";
        conversationShowcase.text = "";

        if (str == "AKComm/NextPage")
        {
            myAnimator.SetTrigger("NextPage");

            Coroutine SC = ReadingCoroutine;
            ReadingCoroutine = null;

            //讀到指令自動下一行

            selfDebugClog = true;
            NextPage();
            if (SC != null)
            {
                StopCoroutine(SC);
            }
        }

        onReadingClog = true;
        allowSkipClog = true;

        if (!selfDebugClog)
        {
            for (int i = 0; i < conversationText.Length; i++)
            {
                if (onReadingClog == true)
                {
                    Debug.Log("問問問" + i);
                    conversationShowingText += conversationText[i];
                    conversationShowcase.text = conversationShowingText;
                    yield return new WaitForSeconds(wordGenDur);
                }
                else
                {
                    conversationShowingText = conversationText;
                    conversationShowcase.text = conversationShowingText;
                    break;
                }
            }
        }
        onReadingClog = false;
        allowSkipClog = false;
        yield return new WaitForSeconds(wordGenDur);
        ReadingCoroutine = null;
    }

    public void TeleportToTargetScene()
    {
        StartCoroutine(TeleportToTargetSceneCoroutine());
    }
    public IEnumerator TeleportToTargetSceneCoroutine()
    {
        yield return null;
        Debug.Log("Y");
        SceneManager.LoadScene(loadSceneSort);
    }
}
