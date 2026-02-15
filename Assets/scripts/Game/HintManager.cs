using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public GameObject hintWordPrefab;
    public GameObject hintWordPrefabCanvas;

    public GameObject refPtL;
    public GameObject refPtR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //SpawnHintWordPrefab("´ú¸Õ°T®§");
        }
    }

    public void SpawnHintWordPrefab(string message)
    {
        GameObject obj = Instantiate(hintWordPrefab);
        obj.transform.parent = hintWordPrefabCanvas.transform;

        obj.transform.GetChild(0).gameObject.GetComponent<Text>().text = message;

        RectTransform recL = refPtL.GetComponent<RectTransform>();
        RectTransform recR = refPtR.GetComponent<RectTransform>();

        Vector2 archP = new Vector2(Random.Range(recL.anchoredPosition.x, recR.anchoredPosition.x), Random.Range(recL.anchoredPosition.y, recR.anchoredPosition.x));

        obj.GetComponent<RectTransform>().anchoredPosition = archP;
    }
    public void SpawnHintWordPrefab(string message, RectTransform rect)
    {
        GameObject obj = Instantiate(hintWordPrefab);
        obj.transform.parent = hintWordPrefabCanvas.transform;

        obj.transform.GetChild(0).gameObject.GetComponent<Text>().text = message;
        obj.GetComponent<RectTransform>().anchoredPosition = rect.anchoredPosition;
    }
}
