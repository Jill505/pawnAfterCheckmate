using UnityEngine;

public class AllGameManager : MonoBehaviour
{
    static Language languageSelect = Language.T_Mandarin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum Language
{
    T_Mandarin,
    C_Mandarin,
    En,
    Jp
}
