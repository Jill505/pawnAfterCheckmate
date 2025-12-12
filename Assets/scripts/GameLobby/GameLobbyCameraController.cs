using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public Vector2 RefStartPt;
    public Vector2 T_floatingCameraOffsetVector;
    public Vector2 floatingCameraOffsetVector;
    public GameObject MainCameraCarrier;

    [Header("Orthographic Variable")]
    public float NormalOrthographic = 5.49f;
    public float FocusOrthographic = 4.28f;

    private void Start()
    {
        RefStartPt = MainCameraCarrier.transform.position;

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
    }
    private void FixedUpdate()
    {
        T_floatingCameraOffsetVector = Vector2.Lerp(T_floatingCameraOffsetVector, floatingCameraOffsetVector, floatingCamera_SmoothFactor);
        MainCameraCarrier.transform.position = RefStartPt + T_floatingCameraOffsetVector;
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


        gameLobbyManager.scrollViewManger.AllowScroll = false;
        gameLobbyManager.scrollViewManger.AllowZoom = false;

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
}
