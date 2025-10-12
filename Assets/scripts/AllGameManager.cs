using UnityEngine;

public class AllGameManager : MonoBehaviour
{
    static public AllGameManager MyAGM;

    static public Language languageSelect = Language.T_Mandarin;

    static public string path = "SaveFile" + "00";
    static public SaveFileType SFT = SaveFileType.SaveFile0;
    public void SaveBasicSetting()
    {
        
    }

    public void LoadBasicSetting()
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

public enum gear
{
    noGear, //無道具
    bow, //弓箭
    car, //戰車
    horse //馬
}

public enum ability
{
    thePawn,       //兵卒
    undeadWill, //不死意志
    bowElt,    //弓箭擅長
    carElt,    //車擅長
    canonElt, //炮擅長
}

public enum SaveFileType
{
    SaveFile0,
    SaveFile1,
    SaveFile2,
}

public enum Camp
{
    Player,
    Enemy,
    Bucket,
    Item
}

public enum BucketType
{
    noType,
    firePowderBucket
}

public enum MissionType
{
    Survive,
    KillTarget
}