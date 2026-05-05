using UnityEngine;
using UnityEngine.UI;

public class LibraryButtonContainer : MonoBehaviour
{
    public Text myShowText;

    public GameLobbyManager gameLobbyManager;
    public LibraryManager libraryManager;

    public SO_ItemLibData itemLibData;
    public SO_EnemyLibData enemyLibData;
    public SO_StoryLibData storyLibData;
    public SO_CharacterLibData characterLibData;
    public SO_MechanicsLibData mechanicsLibData;
    public SO_StatsLibData statsLibData;


    private void Awake()
    {
        gameLobbyManager = FindFirstObjectByType<GameLobbyManager>();
        libraryManager = FindFirstObjectByType<LibraryManager>();
    }

    public void OnItemButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = itemLibData.GetMaxKnowledgeSprite();
        libraryManager.InformationInterface_Name.text = itemLibData.GetName();
        libraryManager.InformationInterface_Description.text = itemLibData.GetMaxKnowledgeStrs();

        libraryManager.TrickApplyButton.SetActive(true);
        libraryManager.special_loadingTrickType = itemLibData.myTrickType;

        SaveSystem.LoadSF();

        libraryManager.TrickApplyButton.GetComponent<Button>().interactable = 
            SaveSystem.SF.holdingTrickType == itemLibData.myTrickType ? false : true;  
    }

    public void OnEnemyButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = enemyLibData.GetMaxKnowledgeSprite();
        libraryManager.InformationInterface_Name.text = enemyLibData.GetName();
        libraryManager.InformationInterface_Description.text = enemyLibData.GetMaxKnowledgeStrs();
    }

    public void OnStoryButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = storyLibData.GetMaxKnowledgeSprite();
        libraryManager.InformationInterface_Name.text = storyLibData.GetName();
        libraryManager.InformationInterface_Description.text = storyLibData.GetMaxKnowledgeStrs();
    }

    public void OnCharacterButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = characterLibData.GetMaxKnowledgeSprite();
        libraryManager.InformationInterface_Name.text = characterLibData.GetName();
        libraryManager.InformationInterface_Description.text = characterLibData.GetMaxKnowledgeStrs();
    }

    public void OnMechanicsButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = mechanicsLibData.GetMaxKnowledgeSprite();
        libraryManager.InformationInterface_Name.text = mechanicsLibData.GetName();
        libraryManager.InformationInterface_Description.text = mechanicsLibData.GetMaxKnowledgeStrs();
    }

    public void OnStatsButtonDown()
    {
        libraryManager.InformationInterfaceReset();

        libraryManager.InformationInterface_Image.sprite = statsLibData.GetSprite();
        libraryManager.InformationInterface_Name.text = statsLibData.GetName();
        libraryManager.InformationInterface_Description.text = statsLibData.GetDesc();
    }
}
