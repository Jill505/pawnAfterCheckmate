using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;

public class StoryManager : MonoBehaviour
{
    [Header("Ref")]
    public GameObject fatherCanvas;
    public SoundManager soundManager;

    public TextMeshProUGUI SpeakerName;
    public TextMeshProUGUI SpeakContext;

    public Image BackGroundImage;

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

    [Header("System Assist Refs")]
    public Image[] ImagePosArr;
    public Animator BlackScreenAnimator;

    public void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
    }

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
            StoryToLoad.LoadLangData();
            Debug.Log("The story coroutine is loaded succ");
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

        Debug.Log("StoryToLoad.strs.Length: " + StoryToLoad.conversationContext.Length);

        //解析指令
        for (int i = 0; i < StoryToLoad.conversationContext.Length; i++)
        {
            string strRead = StoryToLoad.conversationContext[i];

            //檢測屬性
            //假如是指令
            if (strRead.StartsWith("Comm/"))
            {
                string[] parts = strRead.Split('/');

                Debug.Log("Readline Command, the context is " + parts);

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
        Scene currentScene = SceneManager.GetActiveScene();
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
                if (currentScene.buildIndex == 2)
                {
                    FindFirstObjectByType<TimerManager>().isPause = true;
                }
                else {
                    Debug.LogError("You are not in fight");
                }

                break;

            case "FightStartTimer":
                if (currentScene.buildIndex == 2)
                {
                    FindFirstObjectByType<TimerManager>().isPause = false;
                    //FindFirstObjectByType<RoundManager>().
                }
                break;
            case "SetImage":
                SetImageAt(int.Parse(commStr[2]), commStr[3]);
                break;

            case "GoNextNode":
                if (currentScene.buildIndex == 2) {
                    FindFirstObjectByType<RoundProcessManager>().GoNextNode();
                }
                break;

            case "LoadScene":
                BlackScreenAnimator.SetBool("BlackScreenControl", true);
                int sceneSort = int.Parse(commStr[2]);
                Delay(1.7f, () => { SceneManager.LoadScene(sceneSort); });
                break;

            case "BlackIn":
                BlackScreenAnimator.SetBool("BlackScreenControl", true);
                break;
            case "BlackOut":
                BlackScreenAnimator.SetBool("BlackScreenControl", false);
                break;

            case "Wait":
                break;

            case "SwitchBackgroundImage":
                StartCoroutine(SwitchBackgroundImageCoroutine(Resources.Load<Sprite>("StorySprite/" + commStr[2])));
                break;

            case "SetBlackScreenOff":
                BlackScreenAnimator.gameObject.SetActive(false);
                break;
            case "SetBlackScreenON":
                BlackScreenAnimator.gameObject.SetActive(true);
                break;

            case "PlaySFX":
                soundManager.PlaySFX(commStr[2]);
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
        if (Resources.Load<Sprite>("StorySprite/" + name) == null)
        {
            ImagePosArr[posIndex].sprite = Resources.Load<Sprite>("MISC/alphaPicutre");
        }
        else
        {
            ImagePosArr[posIndex].sprite = Resources.Load<Sprite>("StorySprite/" + name);
        }
    }

    public IEnumerator SwitchBackgroundImageCoroutine(Sprite Sprite)
    {
        BlackScreenAnimator.speed = 2;
        BlackScreenAnimator.SetBool("BlackScreenControl", true);
        yield return new WaitForSeconds(1.2f);
        BackGroundImage.sprite = Sprite;
        
        BlackScreenAnimator.SetBool("BlackScreenControl", false);
        yield return new WaitForSeconds(1.2f);
        BlackScreenAnimator.speed = 1;
    }
    public Coroutine Delay(float sec, Action func)
    {
        return StartCoroutine(LateFuncCoroutine(sec, func));
    }
    public IEnumerator LateFuncCoroutine(float sec, Action func)
    {
        yield return new WaitForSeconds(sec);
        func();
    }
}
