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
}
