using UnityEngine;

public class DevCheat_Lobby : MonoBehaviour
{
    public GameObject MyPatternObject;

    public void OpenCheatPattern() { MyPatternObject.SetActive(true); }
    public void CloseCheatPattern(){ MyPatternObject.SetActive(false); }
    public void Cheat_GainMerit()
    {
        SaveSystem.SF.merit += 100;
        SaveSystem.SaveSF();
    }

    public void Cheat_GainKey()
    {
        SaveSystem.SF.key += 1;
        SaveSystem.SaveSF();
    }

    public void Cheat_GainSkillPoint()
    {
        SaveSystem.SF.skillPoint += 1;
        SaveSystem.SaveSF();
    }
}
