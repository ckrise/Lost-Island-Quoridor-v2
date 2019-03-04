using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    private List<string> roomList = new List<string>();
    public PhotonView photonView;
    #endregion

    #region references
    private void Start()
    {
        networkController = this;
        gameController = GameController.GCInstance;
        guiController = GUIController.GUIReference;
        
    }
    #endregion

    #region Multiplayer Connect
    public void connectToPhoton()
    {
        string connectString = "Connecting To Multiplayer...";
        menuController = MenuController.menu;
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("connecting");
        menuController.changeLoadingText(connectString);
    }

    private void OnFailedToConnectToPhoton()
    {
        menuController.OpenFailMultiplayerPanel();
        Debug.Log("Failed To Connect");
        //TODO notify user they have not connected
    }

    private void OnJoinedLobby()
    {
        menuController.MultiPlayer();
        GameData.NetworkController = this;
        Debug.Log("joined"); 
    }

    private void OnReceivedRoomListUpdate()
    {
        Debug.Log("This was called");
        listRooms();
    }
    #endregion

    #region Room Functions
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
    public void onClickCreateRoom()
    {
        string creatingRoom = "Creating Room...";
        string playerName = PlayerData.PlayerName;  //PlayerPrefs.GetString("PlayerName");
        string newPlayerName = playerName;
        bool goodRoomName = false;
        int count = 1;
        
        while(!goodRoomName)
        {
            if (!roomList.Contains(newPlayerName))
            {
                goodRoomName = true;
                count = 1;
            }
            else if(roomList.Contains(playerName))
            {
                newPlayerName = playerName + '(' + count + ')';
                count++;
            }
            else
            {
                //Not even sure how it would get to this
            }
        }
        playerName = newPlayerName;
        
        Debug.Log(playerName);
        PhotonNetwork.CreateRoom(playerName, new RoomOptions() { MaxPlayers = 2 }, null);
        menuController.changeLoadingText(creatingRoom);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("CreatedRoom");
        GameData.PlayerGoesFirst = true;
        myRoom = PhotonNetwork.room.Name;
        menuController.SetLobbyName(myRoom);
        menuController.CreateRoom();
    }

    private void OnPhotonCreateRoomFailed()
    {
        menuController.OpenFailCreateRoomPanel();
        Debug.Log("Could not create room");
    }
    #endregion

    #region Join Rooms
    public void onClickJoinRoom(string name)
    {
        Debug.Log(name);
        PhotonNetwork.JoinRoom(name);
        //PhotonNetwork.JoinRoom(roomName);
        
    }

    private void OnJoinedRoom()
    {
        myRoom = PhotonNetwork.room.Name;
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            GameData.PlayerGoesFirst = false;
            networkGame();
            PhotonNetwork.LoadLevel("GameScene");
        }
        Debug.Log("JOINED ROOM!");
        Debug.Log(PhotonNetwork.room.Name);
    }

    private void OnPhotonJoinRoomFailed()
    {
        menuController.OpenFailJoinRoomPanel();
        Debug.Log("Failed To Join Room");
    }
    #endregion

    #region Back Functions
    public void onClickLeaveRoom()
    {
       PhotonNetwork.room.IsVisible = false;
       PhotonNetwork.room.IsOpen = false;
       PhotonNetwork.LeaveRoom();
       Debug.Log("Left Room");
        
    }

    public void onClickBack()
    {
        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        else if(PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

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
        //isConnectedServer = false;
        Debug.Log("Disconnected from photon");
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Call this when anything disconnection happens"); 
    }

    public void onClickLeaveMultiplayer()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    private void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
    }
    #endregion

    #region Network Game
    private void OnPhotonPlayerConnected()
    {
        if(PhotonNetwork.room.PlayerCount == 2)
        {
            networkGame();
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            Debug.Log("Player count not at 2");
        }
    }

    public void networkGame()
    {
        Debug.Log("Start network game");
        //Setting some things up before we start the game
        photonView = PhotonView.Get(this);
        guiController = GUIController.GUIReference;
        GameData.IsAIGame = false;

        if (photonView == null)
        {
            Debug.Log("null object");
        }
    }
    #endregion

    #region Quitting/Finishing Game
    public void gameOver()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public void OnPhotonPlayerDisconnected()
    {
        Debug.Log("Player disconnected!");
        if(PhotonNetwork.room.PlayerCount == 1)
        {
            guiController.OpponentLeft();           
        }
    }

    #endregion

    #region Send Moves

    public void onMoveToSend(string moveToSend)
    {
        photonView.RPC("sendMove", PhotonTargets.Others, moveToSend);
    }

    public void onMessageToSend(string message)
    {
        Debug.Log(message);
        photonView.RPC("chatMessage", PhotonTargets.Others, message);
    }
    #endregion

    #region Receive Move
    [PunRPC]
    public void chatMessage(string message)
    {
        string messageToDisplay = message;
        //NEW CHANGE
        guiController.ReceiveMessage(message);
        Debug.Log(messageToDisplay);
    }

    [PunRPC]
    public void sendMove(string move)
    {
        string newMove = move;
        string moveToSend;
        moveToSend = changeOrientation(newMove);
        gameController.RecieveMoveFromNetwork(moveToSend);
        Debug.Log(move);
    }

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
}
