using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryManager : MonoBehaviour
{
    public SO_ItemLibData[] ItemLibDataArr; //物品說明
    public SO_EnemyLibData[] EnemyLibDataArr; //統計
    public SO_StoryLibData[] StoryLibDaraArr; //回顧劇情
    public SO_CharacterLibData[] CharacterLibData_Arr; //角色資訊

    public GameObject ButtonContextGameObject;

    public List<GameObject> LoadingUIGameObjectArr = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject ItemButtonPrefab;
    public GameObject EnemyButtonPrefab;
    public GameObject StoryButtonPrefab;
    public GameObject CharacterInfoButtonPrefab;


    [Header("UI")]
    public TextMeshProUGUI CategoryName;

    [Header("Information Interface")]
    public TextMeshProUGUI InformationInterface_Name;
    public TextMeshProUGUI InformationInterface_Description;
    public Image InformationInterface_Image;

    [Header("Information Interface Special Load Function Button")]
    public GameObject TrickApplyButton;
    public TrickType special_loadingTrickType;

    public bool ShowAllInfo = false;

    void Start()
    {
        LoadItemCategory(); //Load Default
        InformationInterfaceReset();
    }

    void Update()
    {
        
    }

    public void LoadItemCategory()
    {
        InformationInterfaceReset();
        for (int i = ButtonContextGameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ButtonContextGameObject.transform.GetChild(i).gameObject);
        }

        CategoryName.text = "物品";

        for (int i = 0; i < ItemLibDataArr.Length; i++)
        {
            if (SaveSystem.SF.ItemKnowledgeLevelData[ItemLibDataArr[i].ItemUID] > 0 || ShowAllInfo)
            {
                //Player know this item, add it.
                GameObject IBP = Instantiate(ItemButtonPrefab);
                IBP.transform.SetParent(ButtonContextGameObject.transform);
                IBP.transform.localScale = new Vector3(1, 1, 1);

                LibraryButtonContainer LBC = IBP.GetComponent<LibraryButtonContainer>();
                LBC.itemLibData = ItemLibDataArr[i];
                LBC.myShowText.text = LBC.itemLibData.GetName();
            }
        }
    }

    public void LoadEnemyCategory()
    {
        InformationInterfaceReset();
        for (int i = ButtonContextGameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ButtonContextGameObject.transform.GetChild(i).gameObject);
        }

        CategoryName.text = "敵人";


        for (int i = 0; i < EnemyLibDataArr.Length; i++)
        {
            if (SaveSystem.SF.EnemyHistoryKillData[EnemyLibDataArr[i].EnemyUID] > 0 || ShowAllInfo)
            {
                //Player know this item, add it.
                GameObject IBP = Instantiate(EnemyButtonPrefab);
                IBP.transform.SetParent(ButtonContextGameObject.transform);
                IBP.transform.localScale = new Vector3(1, 1, 1);

                LibraryButtonContainer LBC = IBP.GetComponent<LibraryButtonContainer>();
                LBC.enemyLibData = EnemyLibDataArr[i];
                LBC.myShowText.text = LBC.enemyLibData.GetName();
            }
        }
    }

    public void LoadStoryCategory()
    {
        InformationInterfaceReset();
        for (int i = ButtonContextGameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ButtonContextGameObject.transform.GetChild(i).gameObject);
        }

        CategoryName.text = "故事";

        for (int i = 0; i < StoryLibDaraArr.Length; i++)
        {
            if (SaveSystem.SF.StoryKnowledgeLevelData[StoryLibDaraArr[i].StoryUID] > 0 || ShowAllInfo)
            {
                //Player know this item, add it.
                GameObject IBP = Instantiate(StoryButtonPrefab);
                IBP.transform.SetParent(ButtonContextGameObject.transform);
                IBP.transform.localScale = new Vector3(1, 1, 1);

                LibraryButtonContainer LBC = IBP.GetComponent<LibraryButtonContainer>();
                LBC.storyLibData = StoryLibDaraArr[i];
                LBC.myShowText.text = LBC.storyLibData.GetName();
            }
        }
    }

    public void LoadCharacterCategory()
    {
        InformationInterfaceReset();
        for (int i = ButtonContextGameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ButtonContextGameObject.transform.GetChild(i).gameObject);
        }

        CategoryName.text = "角色";

        for (int i = 0; i < CharacterLibData_Arr.Length; i++)
        {
            if (SaveSystem.SF.CharacterKnowledgeLevelData[CharacterLibData_Arr[i].CharacterUID] > 0 || ShowAllInfo)
            {
                //Player know this item, add it.
                GameObject IBP = Instantiate(CharacterInfoButtonPrefab);
                IBP.transform.SetParent(ButtonContextGameObject.transform);
                IBP.transform.localScale = new Vector3(1, 1, 1);

                LibraryButtonContainer LBC = IBP.GetComponent<LibraryButtonContainer>();
                LBC.characterLibData = CharacterLibData_Arr[i];
                LBC.myShowText.text = LBC.characterLibData.GetName();
            }
        }
    }


    public void InformationInterfaceReset()
    {
        //init all the extra button
        InformationInterface_Image.sprite = Resources.Load<Sprite>("MISC/alphaPicutre_withDot");
        InformationInterface_Name.text = string.Empty;
        InformationInterface_Description.text = string.Empty;
    }

}

public enum LibraryCategory
{
    Item,
    Enemy,
    Story,
    Character
}

public class LibraryEntry
{
    public TextAsset langData;
    public string[] strs_lang;

    public int knowledgeLevel;

    public void loadKnowledgeLevel()
    {
        //載入認知等級
        //0 = 條目未解鎖
    }

    public void LoadLangData()
    {

    }
}