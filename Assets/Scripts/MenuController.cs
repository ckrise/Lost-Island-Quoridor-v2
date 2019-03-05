using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    #region public variables
    public static MenuController menu;
    public Button storyButton, quickPlayButton, multiplayerButton, 
        settingsButton, helpButton, backButton, easyButton, hardButton,
        joinRoom, createRoom;
    public GameObject mainPanel, multiplayerPanel, settingsPanel,
        helpPanel, storyPanel, quickplayPanel, lobbyPanel,
        connectingPanel, continuePanel, nameEntryPanel, roomListingPrefab,
        storyHelpPanel, multiPlayerHelpPanel, quickPlayHelpPanel, tutorialHelpPanel,
        settingsHelpPanel, helpHelpPanel, quitHelpPanel, mainHelpPanel, loadingPanel,
        failMultiplayerConnectionPanel, failJoinRoomPanel, failCreateRoomPanel, 
        disconnectedFromMultiplayerPanel, levelPanel; 
    public InputField createRoomField, joinRoomField, nameEntryField;
    public Text lobbyText, connectingText, nameErrorText, nameUpdateText;
    public ScrollRect roomScrollView;
    public Slider musicVolumeSlider, sfxVolumeSlider;
    public Toggle fullscreenToggle;
    #endregion
    #region private variables
    private GameObject levelLoader;
    private List<GameObject> roomListings = new List<GameObject>();
    private List<GameObject> helpPanels;
    #endregion
    #region unity
    // Start is called before the first frame update
    public void Start()
    {
        menu = this;
        
        storyButton.onClick.AddListener(StoryMode);
        quickPlayButton.onClick.AddListener(QuickPlay);
        multiplayerButton.onClick.AddListener(MultiPlayerConnect);
        settingsButton.onClick.AddListener(Settings);
        helpButton.onClick.AddListener(Help);
        if (PlayerData.PlayerName != "")
        {
            continuePanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        helpPanels = new List<GameObject>{ storyHelpPanel, multiPlayerHelpPanel, quickPlayHelpPanel, tutorialHelpPanel,
                                           settingsHelpPanel, helpHelpPanel, quitHelpPanel, mainHelpPanel };

        //initialize fullscreen toggle
        fullscreenToggle.isOn = Screen.fullScreen;
        
        //set music and sfx volume
        musicVolumeSlider.value = PlayerData.MusicVolume;
        sfxVolumeSlider.value = PlayerData.SfxVolume;

        //Not a tutorial unless I say it is!
        GameData.IsTutorial = false;
    }
    #endregion

    #region before main
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
            PlayerData.PlayerName = nameEntryField.text;
            Debug.Log(PlayerData.PlayerName);

            //PlayerPrefs.SetString("PlayerName", nameEntryField.text);
            nameEntryPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        else
        {
            nameErrorText.gameObject.SetActive(true);
        }
    }
    #endregion 


    #region settings
    void Settings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void UpdatePlayerMusicVolume(float vol)
    {
        PlayerData.MusicVolume = vol;
    }

    public void UpdatePlayerSFXVolume(float vol)
    {
        PlayerData.SfxVolume = vol;
    }

    public void UpdateNameFromSettings(InputField name)
    {
        if (name.text != "")
        {
            nameUpdateText.text = $"Name changed\n" +
                                  $"from {PlayerData.PlayerName}\n" +
                                  $"to {name.text}";
            nameUpdateText.gameObject.SetActive(true);
            PlayerData.PlayerName = name.text;
            name.text = "";
        }
    }

    public void ToggleFullscreen(Toggle isFullscreen)
    {
        Screen.fullScreen = isFullscreen.isOn;
    }
    #endregion


    #region quickplay
    void QuickPlay()
    {
        mainPanel.SetActive(false);
        quickplayPanel.SetActive(true);
    }

    public void AItoLevelSelect()
    {
        quickplayPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void ToggleGoFirst()
    {
        
        GameData.PlayerGoesFirst = !GameData.PlayerGoesFirst;
        Debug.Log(GameData.PlayerGoesFirst);
    }

    public void SetAIDifficulty(string difficulty)
    {
        GameData.AIDifficulty = difficulty;
    }
    #endregion

    void StoryMode()
    {
        PlayerPrefs.SetString("PLAY_MODE", "STORY");
        mainPanel.SetActive(false);
        storyPanel.SetActive(true);
    }
    
    public void Back()
    {
        if (lobbyPanel.activeSelf)
        {
            lobbyPanel.SetActive(false);
            multiplayerPanel.SetActive(true);
        }
        else if (levelPanel.activeSelf)
        {
            levelPanel.SetActive(false);
            quickplayPanel.SetActive(true);
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

    public void QuitGame()
    {
        Application.Quit();
    }


    #region Help
    void Help()
    {
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
    }
    
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

    #region Networking Multiplayer
    public void MultiPlayer()
    {
        mainPanel.SetActive(false);
        connectingPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        createRoom.onClick.AddListener(CreatingRoom);
    }

    void MultiPlayerConnect()
    {
        PlayerPrefs.SetString("PLAY_MODE", "NETWORK");
        mainPanel.SetActive(false);
        connectingPanel.SetActive(true);
       
    }
    public void changeLoadingText(string loadText)
    {
        connectingText.text = loadText;
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

    public void UpdateRoomList(List<string> roomNames)
    {
        //remove all rooms
        foreach (var room in roomListings)
        {
            Destroy(room);
        }
        roomListings.Clear();
        foreach (var room in roomNames)
        {
            GameObject newRoomListing = Instantiate(roomListingPrefab, roomScrollView.content);
            newRoomListing.name = room;
            newRoomListing.GetComponentInChildren<Text>().text = room;
            newRoomListing.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                Debug.Log("In Delegate");
                JoinRoom(newRoomListing.name);
            });
            roomListings.Add(newRoomListing);
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

    public void OpenFailMultiplayerPanel()
    {
        if (connectingPanel.activeSelf)
        {
            connectingPanel.SetActive(false);
        }
        failMultiplayerConnectionPanel.SetActive(true);
    }

    public void OpenFailJoinRoomPanel()
    {
        if (connectingPanel.activeSelf)
        {
            connectingPanel.SetActive(false);
        }
        failJoinRoomPanel.SetActive(true);
    }

    public void OpenFailCreateRoomPanel()
    {
        if (connectingPanel.activeSelf)
        {
            connectingPanel.SetActive(false);
        }
        failCreateRoomPanel.SetActive(true);
    }

    public void OpenDisconnectedFromMultiPlayerPanel()
    {
        disconnectedFromMultiplayerPanel.SetActive(true);
    }

    public void CloseFailMultiplayerPanel()
    {
        failMultiplayerConnectionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    public void CloseFailJoinRoomPanel()
    {
        failJoinRoomPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
    }

    public void CloseFailCreateRoomPanel()
    {
        failCreateRoomPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
    }

    public void CloseDisconnectedFromMultiPlayerPanel()
    {
        disconnectedFromMultiplayerPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    #endregion
}