using JetBrains.Annotations;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraObject;
    public Camera myCamera;

    public CameraPosition startPosition = new CameraPosition();
    public CameraPosition cameraConstPosition = new CameraPosition();

    public CameraPosition currentCameraPosition = new CameraPosition();
    public CameraPosition targetCameraPosition = new CameraPosition();

    public float currentOrthographic;
    public float targetOrthographic;

    public float fluent = 0.2f;
    public GameObject targetLookingAt;

    Vector2 _FC_SaveLastCameraPosition;
    float _FC_SaveLastCameraOrthographic;

    private void Awake()
    {
    }

    void Start()
    {
        //Set Camera
        EnterGameCamera();
    }

    // Update is called once per frame
    void Update()
    {
        CameraObject.transform.position = currentCameraPosition.position;
        //CameraObject.transform.rotation = Quaternion.Euler(currentCameraPosition.rotation);

        //currentCameraPosition.position = CameraObject.transform.position;
        //currentCameraPosition.rotation = CameraObject.transform.rotation.eulerAngles;

        //gameObject.transform.LookAt(targetLookingAt.transform.position);

        currentCameraPosition.position = Vector3.Lerp(currentCameraPosition.position, targetCameraPosition.position, fluent * Time.deltaTime);

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

    public void CloseUp(Vector2 closeUpPos, float orthographic, float T)
    {
        _FC_SaveLastCameraPosition = currentCameraPosition.position;
        _FC_SaveLastCameraOrthographic = currentOrthographic;

        targetCameraPosition.position = closeUpPos;
        targetOrthographic = orthographic;
    }
    public void CloseUp(Vector2 closeUpPos)
    {
        CloseUp(closeUpPos, 2, 0.8f);
    }
}
[System.Serializable]
public class CameraPosition
{
    public Vector3 position;
    public Vector3 rotation;

    public float orthographic;
}