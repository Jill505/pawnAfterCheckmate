using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Level", menuName = "Scriptable Objects/SO_Level")]
public class SO_Level : ScriptableObject
{
    public string levelName = "Def name";
    public int gridSizeX  =10;
    public int gridSizeY = 10;
    public List<GameBoardCell> levelContext = new List<GameBoardCell>();
}

[System.Serializable]
public class GameBoardCell
{
    public int locationX = 0;
    public int locationY = 0;

    public string containObjectName;
    
    ///containObjectName Library
    ///Torch => ¤õ§â
    ///Enemy_Pawn => ¼Ä¤H§L
    ///Enemy_Archer => ¼Ä¤H¤}§L

}