using UnityEngine;
using UnityEngine.UI;

public class ButtonEnterPlaySound : MonoBehaviour
{
    public SoundManager SM;
    public Button myButton;

    public bool soundPlayClog;

    public void Start()
    {
        SM = FindFirstObjectByType<SoundManager>();
    }
}
