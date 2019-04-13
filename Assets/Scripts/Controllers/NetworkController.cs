using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Timers;


public class NetworkController : MonoBehaviour
{
    #region Variables
    public const string versionName = "0.1";
    public static NetworkController networkController;
    public string myRoom;
    public string connectingString;
    public InputField createRoomInput, joinRoomInput;
    public static MenuController menuController;
    public GameController gameController = null;
    public GUIController guiController = null;
    public PhotonView photonView;
    private List<string> roomList = new List<string>();
    private static float timer = 0.0f;
    private static bool isNetworkingGame = false;
    private static bool opponentForfeit = false;
    private static string sceneToLoad;
    private static bool opponentWantsReplay = false;
    private static bool playerWantsReplay = true;

    #endregion

    #region references
    //set photon to master client loading scene for both players
    private void Start()
    {
        networkController = this;
        gameController = GameController.GCInstance;
        guiController = GUIController.Instance;
       
    }
    #endregion
    //this is only used for timer
    private void Update()
    {
        //timerCheck();
    }

    #region Multiplayer Connect
    //When player hits multiplayer button
    public void connectToPhoton()
    {
        string connectString = "Connecting To Multiplayer...";
        menuController = MenuController.menu;
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("connecting");
        menuController.changeLoadingText(connectString);
    }

    //Callback when player cannot conenct
    private void OnFailedToConnectToPhoton()
    {
        menuController.OpenFailMultiplayerPanel();
        Debug.Log("Failed To Connect");
    }

    //When we have sucessfully joined multiplayer and can look for rooms to join
    private void OnJoinedLobby()
    {
        menuController.MultiPlayer();
        GameData.NetworkController = this;
        Debug.Log("joined"); 
    }

    //Whenever the room list changes
    private void OnReceivedRoomListUpdate()
    {
        Debug.Log("This was called");
        listRooms();
    }
    #endregion

    #region Room Functions
    //Get the list of rooms from photon, parse the name and build our list
    private void buildRoomList()
    {
        roomList.Clear();
        foreach (var room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(room.PlayerCount);
            if (room.PlayerCount < 2)
            {
                string roomToAdd = room.ToString();
                string[] nameList = roomToAdd.Split('\'');
                Debug.Log(nameList[1]);
                roomList.Add(nameList[1]);
            }
        }
    }

    //Called whenever room list updated, tells GUI to fix the list on screen
    public void listRooms()
    {
        buildRoomList();
        if(roomList.Count == 0)
        {
            Debug.Log("not built");
        }
        menuController.UpdateRoomList(roomList);
    }
    #endregion

    #region Create Rooms
    //This function does a couple things before it actually creates a room
    public void onClickCreateRoom()
    {
        string creatingRoom = "Creating Room...";
        //Get the player name from PlayerData so we aren't using the actual name
        string playerName = PlayerData.PlayerName;  //PlayerPrefs.GetString("PlayerName");
        //Tells us when we can use this name
        bool goodRoomName = false;
        int count = 1;
        string sceneToAdd = "";
        string roomName;
        if (GameData.Scene == "BeachScene")
        {
            sceneToAdd = "-BEA";
        }
        else if (GameData.Scene == "JungleScene")
        {
            sceneToAdd = "-JUN";
        }
        else
        {
            sceneToAdd = "-TEM";
        }
        string newPlayerName = playerName + sceneToAdd;
        //This while loop just puts a number on the end of the name
        //EX player1, player2
        while (!goodRoomName)
        {
            if (!roomList.Contains(newPlayerName))
            {
                goodRoomName = true;
                count = 1;
            }
            else if(roomList.Contains(newPlayerName))
            {
                playerName = playerName + '(' + count + ')';
                newPlayerName = playerName + sceneToAdd;
                count++;
            }
            else
            {
                //Not even sure how it would get to this
            }
        }
        roomName = newPlayerName;
        
        //Create room, only want 2 in room
        Debug.Log(playerName);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 }, null);
        menuController.changeLoadingText(creatingRoom);
    }

    //We only want to change GUI when Photon tells us we have done what we needed
    //This is why all the On functions change menuController, don't want to jump the gun
    private void OnCreatedRoom()
    {
        Debug.Log("CreatedRoom");
        myRoom = PhotonNetwork.room.Name;
        menuController.SetRoomName(myRoom);
        menuController.CreateRoom();
        //If you make room you are now master client, congrats
        PhotonNetwork.SetMasterClient(PhotonNetwork.player);
    }

    //Dido
    private void OnPhotonCreateRoomFailed()
    {
        menuController.OpenFailCreateRoomPanel();
        Debug.Log("Could not create room");
    }
    #endregion

    #region Join Rooms
    //Join room with the name of what we clicked on
    public void onClickJoinRoom(string name)
    {
        Debug.Log(name);
        sceneToLoad = getScene(name);
        PhotonNetwork.JoinRoom(name);    
    }

    //Both players will call OnJoinedRoom if they create/join room
    private void OnJoinedRoom()
    {
        myRoom = PhotonNetwork.room.Name;

        //Quick way to see who is going first
        if(PhotonNetwork.player.IsMasterClient)
        {
            GameData.PlayerGoesFirst = true;
            menuController.SetRoomName(myRoom);
            Debug.Log("GOING FIRST");
        }
        //You joined so you go second
        else
        {
            GameData.PlayerGoesFirst = false;
            Debug.Log("GOING SECOND");
            networkGame();
            menuController.levelLoader.GetComponent<LevelLoader>().LoadLevel(sceneToLoad);
            //load level
        }
        //This is what the person who joined sends to the "host"

    }

    //Dido
    private void OnPhotonJoinRoomFailed()
    {
        menuController.OpenFailJoinRoomPanel();
        Debug.Log("Failed To Join Room");
    }

    private string getScene(string name)
    {
        string sceneToGet;
        string scene = name.Substring(name.Length - 3);
        if(scene == "BEA")
        {
            sceneToGet = "BeachScene";
        }
        else if (scene == "JUN")
        {
            sceneToGet = "JungleScene";
        }
        else
        {
            sceneToGet = "TempleScene";
        }
        

        return sceneToGet;
    }
    #endregion

    #region Back Functions
    //When we leave the room we want it turned invisible until it is destroyed
    //Only those who create it can call this function
    public void onClickLeaveRoom()
    {
       PhotonNetwork.room.IsVisible = false;
       PhotonNetwork.room.IsOpen = false;
       PhotonNetwork.LeaveRoom();  
    }

    //This is either called in the lobby or in an actually room
    public void onClickBack()
    {
        //We are in a room so we must call leave room
        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        //We are leaving multiplayer
        else if(PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    //We either lost connection in a game or at the menus
    private void OnConnectionFail()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu")
        {
            menuController.OpenDisconnectedFromMultiPlayerPanel();
        }
        else
        {
            guiController.openLostConnectionPanel();
        }
    }

    //Dido
    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Call this when anything disconnection happens"); 
    }

    //Pretty sure this isn't called
    public void onClickLeaveMultiplayer()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    //Dido
    private void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
    }
    #endregion

    #region Network Game
    //Dido
    private void OnPhotonPlayerConnected()
    {
       // Debug.Log("player connected");
       if(PhotonNetwork.room.PlayerCount == 2)
        {
            networkGame();
            PhotonNetwork.room.IsVisible = false;
            menuController.levelLoader.GetComponent<LevelLoader>().LoadLevel(GameData.Scene);
            //load level
        }
    }

    //This does some very important things for the online games to work
    public void networkGame()
    {
        //this allows us to send things
        photonView = PhotonView.Get(this);
        guiController = GUIController.Instance;
        GameData.IsAIGame = false;
        if (photonView == null)
        {
            Debug.Log("null object");
        }
        isNetworkingGame = true;
    }
    #endregion

    #region Quitting/Finishing Game
    //TODO change to just going to multiplayer panel not main menu
    public void gameOver()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    //Dido
    public void OnPhotonPlayerDisconnected()
    {
        if(PhotonNetwork.room.PlayerCount == 1)
        {
            guiController.OpponentLeft(opponentForfeit);
        }
    }
    #endregion

    #region Send Stuff
    //We are going to tell
    //a) what function to call
    //b) who to send it to
    //c) what parameters the function needs
    public void onMoveToSend(string moveToSend)
    {
        Debug.Log(PhotonNetwork.player.ID + "made a move");
        photonView.RPC("sendMove", PhotonTargets.Others, moveToSend);
       
    }

    //send chat message to player
    public void onMessageToSend(string message)
    {
        //Debug.Log(message);
        photonView.RPC("chatMessage", PhotonTargets.Others, message);
      
    }

    public void onSendForfeitMessage()
    {
        string forfeitMessage = "forfeit";
        if (isNetworkingGame)
        {
            photonView.RPC("forfeitMessage", PhotonTargets.Others, forfeitMessage);
        }
    }

    public void onSendNoQuitMessage()
    {
        string message = "no forfeit";
        if (isNetworkingGame)
        {
            photonView.RPC("noQuitMessage", PhotonTargets.Others, message);
        }
    }

    public void onSendReplayMessage()
    {
        playerWantsReplay = true;
        string message = "Replay";
        photonView.RPC("sendReplayMessage", PhotonTargets.Others, message);
        if(checkForReplay())
        {

        }
    }

    //this function is for people joining the game
    //It sets stuff up for sending messages and tells the 
    //"host" to start the game

   
    #endregion

    #region Receive Stuff
    //this is the receiving side
    //get chat message display it and reset move timer
    [PunRPC]
    public void chatMessage(string message)
    {
        timer -= timer;
        Debug.Log(timer);
        string messageToDisplay = message;
        //NEW CHANGE
        guiController.ReceiveMessage(message);
        Debug.Log(messageToDisplay);
    }
    
    //get move sent to us and send it to gameController
    [PunRPC]
    public void sendMove(string move)
    {
        string newMove = move;
        string moveToSend;
        moveToSend = changeOrientation(newMove);
        gameController.RecieveMoveFromNetwork(moveToSend);
        //Debug.Log(move);
    }

 
    [PunRPC]
    public void forfeitMessage(string forfeitMessage)
    {
        //TODO gui part
        opponentForfeit = true;
        //gameOver();
    }

    [PunRPC]
    public void noQuitMessage(string message)
    {
        opponentForfeit = false;
    }

    [PunRPC]
    public void sendReplayMessage(string message)
    {
        opponentWantsReplay = true;
        if(checkForReplay())
        {

        }
    }

    //"host" gets startgame message

    //we get the move from the player as if they were player1 so we
    //change as if they were player2
    public string changeOrientation(string move)
    {
        string orientation = "";
        int x = move[0] - 96;
        x = 10 - x;
        char letter = (char)(x + 96);
        x = move[1] - 48;
        x = 10 - x;
        char number = (char)(x + 48);
        if(move.Length == 3)
        {
            letter--;
            number--;
            orientation = move[2].ToString();
        }
        return $"{letter}{number}{orientation}";  
    }
    #endregion

    #region Timer Stuff
    //update timer if we are in a game
    //private void timerCheck()
    //{
    //    if (SceneManager.GetActiveScene().name != "MainMenu" && guiController != null && isNetworkingGame)
    //    {
    //        if (!guiController.playerTurn)
    //        {
    //            timer += Time.deltaTime;
    //            //Debug.Log(timer);
    //            if (timer >= moveTime)
    //            {
    //                DisplayMessage();
    //                //TODO might need to get rid of this
    //                //timer = timer - moveTime;
    //            }
    //        }
    //    }
    //}

    //easy to understand, right?
    //public void continueWaiting()
    //{
    //    timer -= timer;
    //}

    ////yup
    //public void resetTimer()
    //{
    //    timer -= timer;
    //}

    ////decisions, decisions
    //private void DisplayMessage()
    //{
    //    guiController.displayMoveTimerPanel();
    //}
    #endregion

    #region Reconnect and Replay
    public void tryReconnect()
    {
        if(PhotonNetwork.ReconnectAndRejoin())
        {
            //Just close panel and continue
            //maybe send rpc to opponent saying they are here
        }
        else
        {
            //Tell them not able to and leave
            if(PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {

            }
        }
    }

    private bool checkForReplay()
    {
        bool result = false;
        if(opponentWantsReplay && playerWantsReplay)
        {
            result = true;
        }
        return result;
    }
    
    #endregion
}
