using UnityEngine;

public class SpecialLevelScript : MonoBehaviour
{
    public virtual void DO_GameInit()
    {

    }
    public virtual void Do_LevelAwake()
    {

    }
    public virtual string LevelTargetString()
    {
        return "¹w³]­È";
    }
}
public enum SpecialLevelScriptType
{
    noSLS,
    Karen
}