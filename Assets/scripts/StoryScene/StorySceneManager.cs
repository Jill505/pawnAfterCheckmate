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
    public bool allowSkipClog;

    [Header("全端接收站")]
    static public int ADR_LoadStorySort;
    static public bool ADR_IsStoryLoading = false;  

    public AKSO_Story[] RegisterAKSO_Story;

    [Header("測試")]
    public bool isTesting = false;

    private void Awake()
    {
        if (isTesting)
        {
            ADR_IsStoryLoading  =true;
            ADR_LoadStorySort = 0;

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

        Debug.Log("載入 - " + storyText[readingPage]);
        if (ReadingCoroutine == null)
        {
            ReadingCoroutine = StartCoroutine(ReadingLine(storyText[readingPage]));
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
        conversationText = str;
        conversationShowingText = "";
        conversationShowcase.text = "";

        myAnimator.SetTrigger("NextPage");

        onReadingClog = true;
        allowSkipClog = true;

        for (int i = 0; i < conversationText.Length; i++)
        {
            if (onReadingClog == true)
            {
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
