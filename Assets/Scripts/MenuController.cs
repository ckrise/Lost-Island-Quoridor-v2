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
        joinRoom, createRoom, roomListingPrefab;
    public GameObject mainPanel, multiplayerPanel, settingsPanel,
        helpPanel, storyPanel, quickplayPanel, lobbyPanel, 
        connectingPanel, continuePanel;
    public InputField createRoomField, joinRoomField;
    public Text lobbyText, connectingText;
    public ScrollRect roomScrollView;
    // Start is called before the first frame update
    public void Start()
    {
        menu = this;
        storyButton.onClick.AddListener(StoryMode);
        quickPlayButton.onClick.AddListener(QuickPlay);
        multiplayerButton.onClick.AddListener(MultiPlayerConnect);
        settingsButton.onClick.AddListener(Settings);
        helpButton.onClick.AddListener(Help);
        backButton.onClick.AddListener(Back);
    }
    public void ClickToContinue()
    {
        continuePanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    void Back()
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
            backButton.gameObject.SetActive(false);
        }
    }

    void StoryMode()
    {
        PlayerPrefs.SetString("PLAY_MODE", "STORY");
        mainPanel.SetActive(false);
        storyPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
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
                SceneManager.LoadScene("GameScene");
                break;
            case "AI_HARD":
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
        backButton.gameObject.SetActive(true);
    }

    void Help()
    {
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

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
        backButton.gameObject.SetActive(true);
    }

    public void MultiPlayer()
    {
        mainPanel.SetActive(false);
        connectingPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        joinRoom.onClick.AddListener(JoiningRoom);
        createRoom.onClick.AddListener(CreatingRoom);
    }

    public void SetLobbyName(string name)
    {
        lobbyText.text = name;
    }
    
    public void JoiningRoom()
    {
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(true);
        changeLoadingText("Joining Room...");
    }

    public void CreatingRoom()
    {
        multiplayerPanel.SetActive(false);
        connectingPanel.SetActive(true);
        changeLoadingText("Creating Room...");
    }

    public void JoinRoom()
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
        if(roomNames.Count == 0)
        {
            Debug.Log("empty");
        }
        else
        {
            Debug.Log("not empty");
        }
        Debug.Log(roomNames[0]);
        foreach (var room in roomNames)
        {
            Button newRoomListing = Instantiate<Button>(roomListingPrefab, roomScrollView.content);
            newRoomListing.name = room;
            newRoomListing.GetComponentInChildren<Text>().text = room;
        }
    }
    #endregion
}