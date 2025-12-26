using System.Collections;
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

    [Header("Shake Target")]
    public GameObject shakeObject;

    [Header("Shake Settings")]
    public float duration = 0.3f;     // 震動時間
    public float magnitude = 0.15f;   // 震動強度
    public float frequency = 25f;     // 震動頻率（次/秒

    private Vector3 originalPosition;


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
        if (Input.GetKeyDown(KeyCode.T))
        {
            Shake();
        }

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

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude, frequency));
    }

    public void Shake(float factor)
    {
        StartCoroutine(ShakeCoroutine(duration * factor, magnitude * factor, frequency * factor));
    }

    public void Shake(float d, float m, float f)
    {
        StartCoroutine(ShakeCoroutine(d, m, f));
    }
    public IEnumerator ShakeCoroutine(float P_duration, float P_magnitude, float P_frequency)
    {
        if (shakeObject == null)
        {
            Debug.LogWarning("Shake Object is NULL!");
            yield break;
        }

        originalPosition = shakeObject.transform.localPosition;

        float elapsed = 0f;

        while (elapsed < P_duration)
        {
            elapsed += Time.deltaTime;

            float x = Mathf.PerlinNoise(Time.time * P_frequency, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * P_frequency) * 2f - 1f;
            float z = Mathf.PerlinNoise(Time.time * P_frequency, Time.time * P_frequency) * 2f - 1f;

            Vector3 offset = new Vector3(x, y, z) * P_magnitude;

            shakeObject.transform.localPosition = originalPosition + offset;

            yield return null;
        }

        shakeObject.transform.localPosition = originalPosition;
    }
}
[System.Serializable]
public class CameraPosition
{
    public Vector3 position;
    public Vector3 rotation;

    public float orthographic;
}