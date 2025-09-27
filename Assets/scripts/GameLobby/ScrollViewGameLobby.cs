using UnityEngine;

public class ScrollViewGameLobby : MonoBehaviour
{
    public GameLobbyManager gameLobbyManager;

    public Transform MainCameraCarrier;
    Camera mainCamera;

    public bool AllowScroll = false;
    public bool AllowZoom = false;

    public float dragSpeed = 2.2f;
    public Vector3 mousePrePos = Vector3.zero;
    public float zoomSpeed = 10f;

    public float minZoomSize = 2f;
    public float maxZoomSize = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameLobbyManager == null)
        {
            gameLobbyManager = GameObject.Find("GameLobbyManager").GetComponent<GameLobbyManager>();
        }
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ScrollViewFunc();
    }

    void ScrollViewFunc()
    {
        if (AllowScroll)
        {

            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                mousePrePos = Input.mousePosition;
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.LeftAlt))
            {
                Vector3 currentMousePos = Input.mousePosition;
                Vector3 difference = mousePrePos - currentMousePos;

                Vector3 move = new Vector3(difference.x * dragSpeed * Time.deltaTime, difference.y * dragSpeed * Time.deltaTime, 0);

                //Camera.main.transform.Translate(move, Space.World);
                MainCameraCarrier.Translate(move, Space.World);

                mousePrePos = currentMousePos;
            }
        }

        if (AllowZoom)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                Camera.main.orthographicSize -= scroll * zoomSpeed;

                dragSpeed = (Camera.main.orthographicSize / 5) * 2.2f;

                //≠≠®ÓCamera§j§p
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoomSize, maxZoomSize);
            }
        }

        if (AllowScroll && AllowZoom)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                Camera.main.orthographicSize = 5f;
                dragSpeed = 2.2f;
            }
        }
    }
}
