
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class GameLobbyUIManager : MonoBehaviour
{
    [Header("Ref Component")]
    public GameLobbyCameraController gameLobbyCameraController;
    public SoundManager soundManager;

    public GameObject mouseRefGameObject;
    public Vector2 zeroVector = new Vector2();
    public Vector2 mousePos;

    [Header("Fade Effect Variables")]
    public float fadeEffectCal_factor = 0.25f;
    public float fadeEffectCal_Var_Facter = 0.2f;

    public GameObject fadeEffectCalRefPt_L;
    public float fadeEffectCalRefPt_L_Distance =1f;
    public Image fadeEffect_Image_L;

    public GameObject fadeEffectCalRefPt_R;
    public float fadeEffectCalRefPt_R_Distance = 1f;
    public Image fadeEffect_Image_R;

    public GameObject fadeEffectCalRefPt_T;
    public float fadeEffectCalRefPt_T_Distance = 1f;
    public Image fadeEffect_Image_T;
    public Image ToNextLevelButton_Image;

    public GameObject fadeEffectCalRefPt_B;
    public float fadeEffectCalRefPt_B_Distance = 1f;
    public Image fadeEffect_Image_B;
    public Image ToLastLevelButton_Image;

    [Header("Level Name")]
    public Text LevelName;
    public Animator LevelNameAnimator;

    [Header("Start Game Button")]
    public Animator StartGameButtonAnimator;
    public Button StartGameButton;
    public Text StartGameButtonText;

    [Header("Focus Mode")]
    public bool OnFocus = false;
    public GameLobbyDecArea TrickShowArea;
    public GameLobbyDecArea LevelInformationShowArea;

    [Header("Fader Animator")]
    public Animator faderAnimator;

    [Header("CLOG")]
    public bool onSwitchingAnimationClog = false;

    void Awake()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGameButtonAnimator.Play("StartGameButtonHide", -1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (OnFocus)
        {
            mousePos = zeroVector;

            FadeElementReset();
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                QuitFocusDoor();
            }
        }
        else
        {
            MousePosSync();

            VisualEffect_fadeEffectCa_L();
            VisualEffect_fadeEffectCa_R();
            VisualEffect_fadeEffectCa_T();
            VisualEffect_fadeEffectCa_B();
        }
    }

    public void MousePosSync()
    {
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseRefGameObject.transform.position = mousePos;
    }

    public void OnFocusDoor()
    {
        OnFocus = true;
        TrickShowArea.OnInspectShow_On();
        LevelInformationShowArea.OnInspectShow_On();
        gameLobbyCameraController.allowFloatingCamera = false;
        //Call camera pull in;
        gameLobbyCameraController.targetOrthographic = gameLobbyCameraController.FocusOrthographic;

        //Make Enter Game Button Show
        StartGameButtonAnimator.Play("StartGameButtonShow", -1, 0);

        //Make Trick Selector and Level Information Show(Call Animator)
        TrickShowArea.OnInspectShow_On();
        LevelInformationShowArea.OnInspectShow_On();
    }
    public void QuitFocusDoor()
    {
        OnFocus = false;
        gameLobbyCameraController.allowFloatingCamera = true;
        //Call camera pull out;
        gameLobbyCameraController.targetOrthographic = gameLobbyCameraController.NormalOrthographic;

        //Make Enter Game Button Hide
        StartGameButtonAnimator.Play("StartGameButtonHide", -1, 0);

        //Make Trick Selector and Level Information Hide(Call Animator)
        TrickShowArea.OnInspectShow_Off();
        LevelInformationShowArea.OnInspectShow_Off();
    }

    public void FadeElementReset() //使所有互動元素為初始狀態
    {

    }

    public void LoadGame_Func(Action delegateFunc)
    {
        StartCoroutine(LoadGame_Ani_Coroutine(delegateFunc));
    }
    IEnumerator LoadGame_Ani_Coroutine(Action delegateFunc)
    {
        //Call UI Back
        //TrickShowArea.OnInspect = false;
        //LevelInformationShowArea.OnInspect = false;
        OnFocus = false;
        TrickShowArea.OnInspectShow_Off();
        LevelInformationShowArea.OnInspectShow_Off();
        yield return new WaitForSeconds(0.3f);
        faderAnimator.Play("Lobby_Glow_GetInDoor", -1, 0);
        //Set Target
        gameLobbyCameraController.targetOrthographic = 1f;
        
        yield return null;
        //Wait Animation Finish
        yield return new WaitForSeconds(0.6f);
        delegateFunc();
    }

    public void LoadNextRoom_Func(Action delegateFunc)
    {
        if (onSwitchingAnimationClog == true)
        {

        }
        else
        {
            onSwitchingAnimationClog = true;
            StartCoroutine(LoadNextRoom_Ani_Coroutine(delegateFunc));
        }
    }
    IEnumerator LoadNextRoom_Ani_Coroutine(Action delegateFunc)
    {
        onSwitchingAnimationClog = true;

        soundManager.PlaySFX("temp_wind_(wammm)");

        faderAnimator.Play("Lobby_Glow_GetInDoor", -1, 0);
        gameLobbyCameraController.targetOrthographic = 1f;
        yield return new WaitForSeconds(0.6f);
        //blackOut;
        delegateFunc();
        yield return null;
        gameLobbyCameraController.targetOrthographic = gameLobbyCameraController.NormalOrthographic;
        gameLobbyCameraController.nowOrthographic = 10f;
        faderAnimator.Play("Lobby_Glow_GetInRoom", -1, 0);

        onSwitchingAnimationClog = false;   
    }
    public void LoadLastRoom_Func(Action delegateFunc)
    {
        if (onSwitchingAnimationClog)
        {

        }
        else
        {
            onSwitchingAnimationClog = true;
            StartCoroutine(LoadLastRoom_Ani_Coroutine(delegateFunc));
        }
    }
    IEnumerator LoadLastRoom_Ani_Coroutine(Action delegateFunc)
    {
        onSwitchingAnimationClog = true;

        soundManager.PlaySFX("temp_wind_(hooom)");
        gameLobbyCameraController.targetOrthographic = 10f;
        faderAnimator.Play("Lobby_Glow_GetOutRoom", -1, 0);
        yield return new WaitForSeconds(0.5f);
        delegateFunc();
        yield return null;
        gameLobbyCameraController.targetOrthographic = gameLobbyCameraController.NormalOrthographic;
        gameLobbyCameraController.nowOrthographic = 1;
        yield return new WaitForSeconds(0.2f);
        faderAnimator.Play("Lobby_Glow_Idle", -1, 0);

        onSwitchingAnimationClog = false;
    }


    public void VisualEffect_fadeEffectCa_L()
    {
        float distance = Vector2.Distance(mousePos, fadeEffectCalRefPt_L.transform.position);

        float calAlpha = (fadeEffectCalRefPt_L_Distance - distance) * fadeEffectCal_factor;
        fadeEffect_Image_L.color = new Color(1f, 1f, 1f, calAlpha);
    }

    public void VisualEffect_fadeEffectCa_R()
    {
        float distance = Vector2.Distance(mousePos, fadeEffectCalRefPt_R.transform.position);

        float calAlpha = (fadeEffectCalRefPt_R_Distance - distance) * fadeEffectCal_factor;
        fadeEffect_Image_R.color = new Color(1f, 1f, 1f, calAlpha);
    }

    public void VisualEffect_fadeEffectCa_T()
    {
        float distance = Vector2.Distance(mousePos, fadeEffectCalRefPt_T.transform.position);

        float calAlpha = (fadeEffectCalRefPt_T_Distance - distance) * fadeEffectCal_Var_Facter;
        fadeEffect_Image_T.color = new Color(1f, 1f, 1f, calAlpha);
        ToNextLevelButton_Image.color = new Color(1f, 1f, 1f, calAlpha);

    }
    public void VisualEffect_fadeEffectCa_B()
    {
        float distance = Vector2.Distance(mousePos, fadeEffectCalRefPt_B.transform.position);

        float calAlpha = (fadeEffectCalRefPt_B_Distance - distance) * fadeEffectCal_Var_Facter;
        fadeEffect_Image_B.color = new Color(1f, 1f, 1f, calAlpha);
        ToLastLevelButton_Image.color = new Color(1f, 1f, 1f, calAlpha);

    }


    public void Do_LevelNameFadeIn()
    {
        LevelNameAnimator.Play("Show", -1, 0);
    }
}
