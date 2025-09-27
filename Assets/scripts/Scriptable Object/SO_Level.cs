using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Level", menuName = "Scriptable Objects/SO_Level")]
public class SO_Level : ScriptableObject
{
    public string levelName = "Def name";
    public string levelID = "Def ID";

    [TextArea(3, 10)]
    public string levelDesc = "Def Desc";

    [Header("Level setting")]
    public int allowDeployTroopNumber = 3;


    public bool hasPerform;
    public Perform myPerform;
    public int gridSizeX  =10;
    public int gridSizeY = 10;
    public List<GameBoardCell> levelContext = new List<GameBoardCell>();
    public List<GameBoardInsChess> chessInsData = new List<GameBoardInsChess>();

#if UNITY_EDITOR
    private void OnValidate()
    {/*
        if (gridSizeX < 1) gridSizeX = 1;
        if (gridSizeY < 1) gridSizeY = 1;
        //自動更新
        
        //bigger
        if (levelContext.Count > (gridSizeX * gridSizeY))
        {
            for (int i = levelContext.Count; i < gridSizeX * gridSizeY; i++)
            {
                levelContext.Add(new GameBoardCell());
            }
        }

        //smaller
        if (levelContext.Count < (gridSizeX * gridSizeY))
        {
            for (int i = levelContext.Count - 1; i > gridSizeX * gridSizeY; i--)
            {
                levelContext.RemoveAt(i);
            }
        }*/
        if (gridSizeX < 1) gridSizeX = 1;
        if (gridSizeY < 1) gridSizeY = 1;

        int targetCount = gridSizeX * gridSizeY;

        // 如果目前比目標少 → 補齊
        if (levelContext.Count < targetCount)
        {
            for (int i = levelContext.Count; i < targetCount; i++)
            {
                GameBoardCell cell = new GameBoardCell();
                cell.locationX = i / gridSizeX;
                cell.locationY = i % gridSizeX;
                levelContext.Add(cell);
            }
        }

        // 如果目前比目標多 → 刪掉多餘
        if (levelContext.Count > targetCount)
        {
            for (int i = levelContext.Count - 1; i >= targetCount; i--)
            {
                levelContext.RemoveAt(i);
            }
        }
    }
#endif
}

[System.Serializable]
public class GameBoardCell
{
    public int locationX = 0;
    public int locationY = 0;

    public bool isDeployed = false; //已被占用部署地塊
    public bool isAbleToDeploy= false; //是起始允許部署地塊
    public bool passable = true; //可通過地塊

    public string performID;

    ///containObjectName Library
    ///Torch => 火把
    ///Enemy_Pawn => 敵人兵
    ///Enemy_Archer => 敵人弓兵

}
public class GameBoardInsChess
{
    public SO_Chess[] chessFile;
    public int locationX = 0;
    public int locationY = 0;
}

[System.Serializable] 
public class Perform //場地外觀
{
    public performStyle myPerform;

    public void applyPerform()
    {

    }
}
public enum performStyle
{

}