using Unity.VisualScripting;
using UnityEngine;

public class GameLobbyCameraController : MonoBehaviour
{
    public GameLobbyManager gameLobbyManager;


    public Vector2 targetPos;

    public Camera mainCamera;

    public bool OnAutoCamera = false;

    public float OnAutoOrthographicSize = 5f;
    public float ZoomSpeed = 0.2f;

    public float lerpSpeed = 0.2f;
    public Vector2 offset;
    public GameObject CameraOffsetRefObject;

    float movingCount = 0;

    private void Start()
    {
        if (gameLobbyManager == null)
        {
            gameLobbyManager = GameObject.Find("GameLobbyManager").GetComponent<GameLobbyManager>();
        }
    }

    private void FixedUpdate()
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
                transform.position = Vector3.Lerp(transform.position,targetPos, lerpSpeed * Time.fixedDeltaTime) ;
            }
            else
            {
               // Debug.Log("FinishMoving");
                OnAutoCamera = false;
                //mainCamera.orthographicSize = OnAutoOrthographicSize;
                //transform.position = targetPos;
            }
        }
    }
    public void LerpMoveCamera(Vector2 pos)
    {
        //Set the camera
       // Debug.Log("Ey");
        OnAutoCamera = true;


        gameLobbyManager.scrollViewManger.AllowScroll = false;
        gameLobbyManager.scrollViewManger.AllowZoom = false;

        movingCount = 0f;
        offset = -1*(CameraOffsetRefObject.transform.localPosition);
        targetPos = pos + offset;
    }
}
