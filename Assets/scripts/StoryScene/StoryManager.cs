using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("Ref")]
    public GameObject fatherCanvas;

    public TextMeshProUGUI SpeakerName;
    public TextMeshProUGUI SpeakContext;

    [Header("Story Load Info")]
    public SO_Story StoryToLoad;

    [Header("Settings")]
    public bool AllowSkip;
    public bool AllowSkipOneLine;

    public float speakInterval_En = 0.1f;
    public float speakInterval_Cn = 0.2f;

    [Header("Calculate stuff")]
    public Action StoryEndAction;
    public Coroutine myStoryCoroutine;

    [Header("Loading Calculate Variables")]
    public bool isOnStoryProcessing;
    public bool isTextFinishLoad;
    public bool CheckFlag;

    bool _finFlag;

    [Header("System string")]
    public string speakingName;
    public string speakingContext;

    [Header("TestStory")]
    public SO_Story tStory;

    [Header("ImagePos")]
    public Image[] ImagePosArr;

    public void Update()
    {
        if (isOnStoryProcessing)
        {
            InteractWithStory();
        }

        SyncUi();

        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Start Test Story");
            LoadStory(tStory);
        }*/
    }

    public void SyncUi()
    {
        SpeakerName.text = speakingName;
        SpeakContext.text = speakingContext;
    }

    public void LoadStory()
    {
        if (StoryToLoad == null)
        {
            Debug.LogError("There's no story to load");
        }

        if (myStoryCoroutine == null)
        {
            myStoryCoroutine = StartCoroutine(StoryCoroutine());
        }
        else
        {
            Debug.Log("The Story is loading!");
        }
    }

    public void LoadStory(SO_Story SO_S)
    {
        StoryToLoad = SO_S;
        LoadStory();
    }

    public IEnumerator StoryCoroutine()
    {
        isOnStoryProcessing = true;
        StoryToLoad.LoadLangData();
        //解析指令
        for (int i = 0; i < StoryToLoad.strs.Length; i++)
        {
            string strRead = StoryToLoad.strs[i];

            //檢測屬性
            //假如是指令
            if (strRead.StartsWith("Comm/"))
            {
                string[] parts = strRead.Split('/');
                CommandRead(parts);
                continue;
            }

            //假如是單純文字
            speakingContext = "";

            isTextFinishLoad = false;
            for (int j = 0; j < strRead.Length; j++)
            {
                if (_finFlag)
                {
                    _finFlag= false;
                    speakingContext = strRead;
                    break;
                }

                speakingContext += strRead[j];
                yield return new WaitForSeconds(speakInterval_Cn);
            }
            isTextFinishLoad = true;
            yield return new WaitUntil(() => CheckFlag);
            CheckFlag = false;
        }

        myStoryCoroutine = null;
        isOnStoryProcessing = false;
        yield return null;
    }

    public void InteractWithStory()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTextFinishLoad)
            {
                //Next line
                CheckFlag = true;
            }

            if (AllowSkipOneLine && !isTextFinishLoad)
            {
                _finFlag = true;
            }
        }
    }


    public void SkipButton()
    {
        if (AllowSkip)
        {

        }
    }

    public void CommandRead(string[] commStr)
    {
        switch (commStr[1])
        {
            case "CanvasOn":
                Comm_CanvasOn();
                break;

            case "CanvasOff":
                Comm_CanvasOff();
                break;

            case "SetSpeaker":
                SetSpeaker(commStr[2]);
                break;

            case "FightStopTimer":
                if (SceneManager.sceneCount == 2) 
                {
                    FindFirstObjectByType<TimerManager>().isPause = true;
                }
                break;

            case "FightStartTimer":
                if (SceneManager.sceneCount == 2)
                {
                    FindFirstObjectByType<TimerManager>().isPause = false;
                }
                break;
            case "SetImage":
                SetImageAt(int.Parse(commStr[2]), commStr[3]);
                break;

            case "GoNextNode":
                if (SceneManager.sceneCount == 2) {
                    FindFirstObjectByType<RoundProcessManager>().GoNextNode();
                }
                break;
        }
    }

    public void Comm_CanvasOn()
    {
        fatherCanvas.SetActive(true);
    }

    public void Comm_CanvasOff()
    {
        fatherCanvas.SetActive(false);
    }

    public void SetSpeaker(string name)
    {
        speakingName = name;    
    }

    public void SetImageAt(int posIndex, string name)
    {
        ImagePosArr[posIndex].sprite = Resources.Load<Sprite>("StorySprite/" + name);
    }
}
