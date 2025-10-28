using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraObject;
    public Camera myCamera;

    public CameraPosition startPosition = new CameraPosition();
    public CameraPosition cameraConstPosition = new CameraPosition();

    public CameraPosition currentCameraPosition = new CameraPosition();
    public CameraPosition targetCameraPosition = new CameraPosition();

    public float fluent = 0.2f;
    public GameObject targetLookingAt;

    private void Awake()
    {
        cameraConstPosition.position = new Vector3(0, 0, -11.35f);
    }

    void Start()
    {
        //Set Camera
        EnterGameCamera();
    }

    // Update is called once per frame
    void Update()
    {
        currentCameraPosition.position = CameraObject.transform.position;
        currentCameraPosition.rotation = CameraObject.transform.rotation.eulerAngles;

        gameObject.transform.LookAt(targetLookingAt.transform.position);

        CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, targetCameraPosition.position, fluent * Time.deltaTime);

    }

    public void EnterGameCamera()
    {
        CameraObject.transform.position = startPosition.position;
        CameraObject.transform.rotation = Quaternion.Euler(startPosition.rotation);

        targetCameraPosition = cameraConstPosition;
    }

    public void ExitGameCamera()
    {
        targetCameraPosition = startPosition;
    }
}
[System.Serializable]
public class CameraPosition
{
    public Vector3 position;
    public Vector3 rotation;
}