
using UnityEngine;
public class GameLobbyCameraController : MonoBehaviour
{
    [Header("Manager Refs")]
    public GameLobbyManager gameLobbyManager;
    public GameLobbyUIManager gameLobbyUIManager;

    public Vector2 targetPos;

    public Camera mainCamera;
    public ScrollViewGameLobby SVGL;

    public GameObject LevelInfoCanvas;

    public bool OnAutoCamera = false;

    public float OnAutoOrthographicSize = 5f;
    public float ZoomSpeed = 0.2f;

    public float lerpSpeed = 0.2f;
    public Vector2 offset;
    public GameObject CameraOffsetRefObject;

    float movingCount = 0;

    [Header("Floating Camera Effect")]
    public bool allowFloatingCamera = true; 

    public float floatingCamera_StrengthFactor = 0.02f;
    public float floatingCamera_SmoothFactor;
    public Vector2 RefStartPt; //參照點
    public Vector2 TargetRefStartPt; //Lerp參照點
    public Vector2 T_floatingCameraOffsetVector;
    public Vector2 floatingCameraOffsetVector;
    public GameObject MainCameraCarrier;

    [Header("Orthographic Variable")]
    public float nowOrthographic;
    public float targetOrthographic = 5f;

    public float NormalOrthographic = 5.49f;
    public float FocusOrthographic = 4.28f;

    public float CameraSmooth = 0.2f;

    [Header("Ref CameraPoints")]
    public GameObject RefObject_Library;
    public GameObject RefObject_Shop;
    public GameObject RefObject_Lobby;
    public GameObject RefObject_Mission;
    public GameObject RefObject_Talent;

    LobbyCameraPos _realCameraPos;
    public LobbyCameraPos MyCameraPos
    {
        get { return _realCameraPos; }
        set { 
            _realCameraPos = value;
            switch (value)
            {
                case LobbyCameraPos.Library:
                    TargetRefStartPt = RefObject_Library.transform.position;
                    break;
                case LobbyCameraPos.Shop:
                    TargetRefStartPt = RefObject_Shop.transform.position;
                    break;
                case LobbyCameraPos.Lobby:
                    TargetRefStartPt = RefObject_Lobby.transform.position;
                    break;
                case LobbyCameraPos.Mission:
                    TargetRefStartPt = RefObject_Mission.transform.position;
                    break;
                case LobbyCameraPos.Talent:
                    TargetRefStartPt = RefObject_Talent.transform.position;
                    break;
            }
        }
    }

    /// <summary>
    /// -2 = Library
    /// -1 = Shop
    ///  0 = Lobby
    ///  1 = Mission 
    ///  2 = Talent
    /// </summary>
    public int NowLobbyCameraPosIndex = 0;
    
    private void Start()
    {
        nowOrthographic = NormalOrthographic;
        targetOrthographic = NormalOrthographic;

        //RefStartPt = MainCameraCarrier.transform.position;
        RefStartPt = RefObject_Lobby.transform.position;

        //OnAutoCamera = true;
        if (gameLobbyManager == null)
        {
            gameLobbyManager = GameObject.Find("GameLobbyManager").GetComponent<GameLobbyManager>();
        }
        if (gameLobbyUIManager == null)
        {
            gameLobbyUIManager = FindAnyObjectByType<GameLobbyUIManager>();
        }
    }

    private void Update()
    {
        if (allowFloatingCamera)
        {
            FloatingCameraEffect();
        }
        else
        {
            floatingCameraOffsetVector = new Vector2();
        }

        nowOrthographic = Mathf.Lerp(nowOrthographic, targetOrthographic, CameraSmooth * Time.deltaTime);
        mainCamera.orthographicSize = nowOrthographic;

    }
    private void FixedUpdate()
    {
        T_floatingCameraOffsetVector = Vector2.Lerp(T_floatingCameraOffsetVector, floatingCameraOffsetVector, floatingCamera_SmoothFactor);
        MainCameraCarrier.transform.position = RefStartPt + T_floatingCameraOffsetVector;
        RefStartPt = Vector2.Lerp(RefStartPt, TargetRefStartPt, floatingCamera_SmoothFactor);
    }

    public void FloatingCameraEffect()
    {
        Vector2 dist = gameLobbyUIManager.mousePos - RefStartPt;
        floatingCameraOffsetVector = dist * floatingCamera_StrengthFactor;
    }


    public void CancelLevelInspect()
    {
        SVGL.AllowScroll = true;
        SVGL.AllowZoom = true;
        OnAutoOrthographicSize = 5f;
        LevelInfoCanvas.SetActive(false);
    }
    public void LerpMoveCamera(Vector2 pos)
    {
        //Set the camera
        // Debug.Log("Ey");
        OnAutoCamera = true;


        //gameLobbyManager.scrollViewManger.AllowScroll = false;
        //gameLobbyManager.scrollViewManger.AllowZoom = false;

        OnAutoOrthographicSize = 1.8f;

        movingCount = 0f;
        offset = -1 * (CameraOffsetRefObject.transform.localPosition);
        targetPos = pos + offset;
    }
    public void AutoCamera()
    {

        if (OnAutoCamera)
        {
            movingCount += Time.fixedDeltaTime;

            if (mainCamera.orthographicSize != OnAutoOrthographicSize)
            {
                //Debug.Log("Moving Ort");
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, OnAutoOrthographicSize, ZoomSpeed * Time.fixedDeltaTime);
            }
            else
            {
                //Debug.Log("Fin Ort");
            }

            //if (transform.position != (Vector3)targetPos || movingCount < 5)
            if (transform.position != (Vector3)targetPos)
            {
                //Debug.Log("Moving");
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // Debug.Log("FinishMoving");

                //mainCamera.orthographicSize = OnAutoOrthographicSize;
                //transform.position = targetPos;
            }
            if (transform.position == (Vector3)targetPos && mainCamera.orthographicSize == OnAutoOrthographicSize)
            {
                OnAutoCamera = false;
            }
        }
    }

    public void MoveToLeftRoom()
    {
        NowLobbyCameraPosIndex -= 1;
        if (NowLobbyCameraPosIndex < -2)
        {
            NowLobbyCameraPosIndex = -2;
        }
        SetLobbyCameraPos(NowLobbyCameraPosIndex);
    }
    public void MoveToRightRoom()
    {
        NowLobbyCameraPosIndex += 1;
        if (NowLobbyCameraPosIndex > 2)
        {
            NowLobbyCameraPosIndex = 2;
        }
        SetLobbyCameraPos(NowLobbyCameraPosIndex);
    }
 
 
    public void SetLobbyCameraPos(int index)
    {
        switch (index)
        {
            case -2:
                TargetRefStartPt = RefObject_Library.transform.position;
                break;
            case -1:
                TargetRefStartPt = RefObject_Shop.transform.position;
                break;
            case 0:
                TargetRefStartPt = RefObject_Lobby.transform.position;
                break;
            case 1:
                TargetRefStartPt = RefObject_Mission.transform.position;
                break;
            case 2:
                TargetRefStartPt = RefObject_Talent.transform.position;
                break;
        }
    }
}

public enum LobbyCameraPos
{
    Library,
    Shop,
    Lobby,
    Mission,
    Talent
}