using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_LobbyLevel", menuName = "Scriptable Objects/SO_LobbyLevel")]
public class SO_LobbyLevel : ScriptableObject
{
    public Sprite backgroundImage;
    public int myStageIndex;
    public int myLevelIndex;

    public SO_Level mySO_Level;

    public SO_LobbyClickableObject[] mySO_LCOs;
}

[Serializable]
public class LobbyGameStage
{
    public SO_LobbyLevel[] levels;
}
