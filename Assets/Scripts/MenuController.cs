using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    public static MenuController menu;
    public Button storyButton, quickPlayButton, multiplayerButton, 
        settingsButton, helpButton, backButton, easyButton, hardButton,
        joinRoom, createRoom;
    public GameObject mainPanel, multiplayerPanel, settingsPanel,
        helpPanel, storyPanel, quickplayPanel, lobbyPanel,
        connectingPanel, continuePanel, nameEntryPanel, roomListingPrefab,
        storyHelpPanel, multiPlayerHelpPanel, quickPlayHelpPanel, tutorialHelpPanel,
        settingsHelpPanel, helpHelpPanel, quitHelpPanel, mainHelpPanel; 
    public InputField createRoomField, joinRoomField, nameEntryField;
    public Text lobbyText, connectingText, nameErrorText;
    public ScrollRect roomScrollView;

    private List<string> roomList = new List<string>();
    private List<GameObject> helpPanels;
    // Start is called before the first frame update
    public void Start()
    {
        menu = this;
        storyButton.onClick.AddListener(StoryMode);
        quickPlayButton.onClick.AddListener(QuickPlay);
        multiplayerButton.onClick.AddListener(MultiPlayerConnect);
        settingsButton.onClick.AddListener(Settings);
        helpButton.onClick.AddListener(Help);
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            continuePanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        helpPanels = new List<GameObject>{ storyHelpPanel, multiPlayerHelpPanel, quickPlayHelpPanel, tutorialHelpPanel,
                                           settingsHelpPanel, helpHelpPanel, quitHelpPanel, mainHelpPanel };


    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        Application.Quit();
    }
    public void ClickToContinue()
    {
        continuePanel.SetActive(false);
        nameEntryPanel.SetActive(true);
        nameEntryField.Select();
        nameEntryField.ActivateInputField();

    }
    public void NameEntered()
    {
        if (nameEntryField.text != "")
        {
            PlayerPrefs.SetString("PlayerName", nameEntryField.text);
            nameEntryPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        else
        {
            nameErrorText.gameObject.SetActive(true);
        }
    }

    public void Back()
    {
        if (lobbyPanel.activeSelf)
        {
            lobbyPanel.SetActive(false);
            multiplayerPanel.SetActive(true);
        }
        else
        {
            mainPanel.SetActive(true);
            multiplayerPanel.SetActive(false);
            storyPanel.SetActive(false);
            settingsPanel.SetActive(false);
            helpPanel.SetActive(false);
            quickplayPanel.SetActive(false);
           
        }
    }

    void StoryMode()
    {
        PlayerPrefs.SetString("PLAY_MODE", "STORY");
        mainPanel.SetActive(false);
        storyPanel.SetActive(true);
        
    }

    void QuickPlay()
    {
        mainPanel.SetActive(false);
        quickplayPanel.SetActive(true);
        easyButton.onClick.AddListener(StartEasy);
        hardButton.onClick.AddListener(StartHard);
    }

    private void StartEasy()
    {
        StartAIGame("AI_EASY");
    }

    private void StartHard()
    {
        StartAIGame("AI_HARD");
    }

    private void StartAIGame(string aiDifficulty)
    {
        PlayerPrefs.SetString("PLAY_MODE", aiDifficulty);
        switch (aiDifficulty)
        {
            case "AI_EASY":
                GameData.AIDifficulty = "easy";
                Debug.Log(GameData.AIDifficulty);
                SceneManager.LoadScene("GameScene");
                break;
            case "AI_HARD":
                GameData.AIDifficulty = "hard";
                Debug.Log(GameData.AIDifficulty);
                SceneManager.LoadScene("GameScene");
                break;
            default:
                break;
        }
    }

    void Settings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
       
    }

    void Help()
    {
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
        
    }

    #region Help
    
    private void HideAllHelp()
    {
        foreach (var panel in helpPanels)
        {
            panel.SetActive(false);
        }
    }

    public void ShowHelpPanel(GameObject panel)
    {
        HideAllHelp();
        panel.SetActive(true);
    }

    #endregion

    #region Networking

    public void changeLoadingText(string loadText)
    {
        connectingText.text = loadText;
    }

    void MultiPlayerConnect()
    {
        PlayerPrefs.SetString("PLAY_MODE", "NETWORK");
        mainPanel.SetActive(false);
        connectingPanel.SetActive(true);
       
    }

    public void MultiPlayer()
    {
        mainPanel.SetActive(false);
        connectingPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        createRoom.onClick.AddListener(CreatingRoom);
    }

    public void SetLobbyName(string name)
    {
        lobbyText.text = name;
    }

    public void CreatingRoom()
    {
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(true);
        changeLoadingText("Creating Room...");
    }

    public void MultiplayerToLobby()
    {
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void CreateRoom()
    {
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void AddRoomsToList(List<string> roomNames)
    {
        foreach (var room in roomNames)
        {
            if (!roomList.Contains(room))
            {
                roomList.Add(room);
                GameObject newRoomListing = Instantiate(roomListingPrefab, roomScrollView.content);
                newRoomListing.name = room;
                newRoomListing.GetComponentInChildren<Text>().text = room;
                newRoomListing.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    Debug.Log("In Delegate");
                    JoinRoom(newRoomListing.name);
                });
            }
        }
    }

    void JoinRoom(string name)
    {
        Debug.Log("Join room called");
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(true);
        changeLoadingText("Joining Room...");
        GameData.NetworkController.onClickJoinRoom(name);
    }
    #endregion
}