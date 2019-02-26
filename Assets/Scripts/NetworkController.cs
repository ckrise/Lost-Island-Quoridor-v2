using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class NetworkController : MonoBehaviour
{
    public const string versionName = "0.1";
    public static NetworkController networkController;
    public string myRoom;
    public string connectingString;
    private bool isConnectedServer = false;
    private bool playingNetworkGame = false;
    public InputField createRoomInput, joinRoomInput;
    public static MenuController menuController;
    public GameController gameController = null;
    public GUIController guiController = null;
    private List<string> roomList = new List<string>();
    public InputField messageInputField;
    public PhotonView photonView;

    #region references
    private void Start()
    {
        networkController = this;
        gameController = GameController.GCInstance;
        guiController = GUIController.GUIReference;
        
    }

    public void getGameReference(GameController gameControllerReference)
    {
        //NEW CHANGE
        gameController = gameControllerReference;
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

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        isConnectedServer = true;
        Debug.Log("Connected To Our App");
    }

    private void OnFailedToConnectToPhoton()
    {
        Debug.Log("Failed To Connect");
    }

    private void OnJoinedLobby()
    {
        menuController.MultiPlayer();
        GameData.NetworkController = this;
        //listRooms();
        Debug.Log("joined"); 
    }

    private void OnRecievedRoomListUpdate()
    {
        Debug.Log("This was called");
        listRooms();
    }

    public bool isConnectedToServer()
    {
        return isConnectedServer;
    }
    #endregion

    #region Room Functions
    private void buildRoomList()
    { 
        foreach (var room in PhotonNetwork.GetRoomList())
        {
            string roomToAdd = room.ToString();
            string[] nameList = roomToAdd.Split('\'');
            Debug.Log(nameList[1]);
            roomList.Add(nameList[1]);
        }
    }

    public void listRooms()
    {
        buildRoomList();
        if(roomList.Count == 0)
        {
            Debug.Log("not built");
        }
        menuController.AddRoomsToList(roomList);
    }

    public List<string> getListOfRooms()
    {
        return roomList;
    }

    public string getRoom()
    {
        return myRoom;
    }

    #endregion

    #region Create Rooms
    public void onClickCreateRoom()
    {
        string creatingRoom = "Creating Room...";
        string playerName = PlayerPrefs.GetString("PlayerName");
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
        Debug.Log("Failed To Join Room");
    }

    #endregion

    #region Back Functions
    public void onClickLeaveRoom()
    {
        if (PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount == 2)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("Left Room");
        }
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

    private void OnDisconnectedFromPhoton()
    {
        isConnectedServer = false;
        Debug.Log("Disconnected from photon");
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

    //Outline of how to send info over network
    public void networkGame()
    {
        Debug.Log("Start networkgame");
        //To call RPC function need a PhotonView
        photonView = PhotonView.Get(this);
        //NEW CHANGE
        guiController = GUIController.GUIReference;
        playingNetworkGame = true;
        
        GameData.IsAIGame = false;
        playingNetworkGame = true;

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

    //public void playerQuitNetworkGame()
    //{
    //    gameOver();
    //}

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

    //public void onForfeitToSend()
    //{
    //    Debug.Log("Called onForfeitToSend() this SHOULD be working!");
    //    photonView.RPC("forfeit", PhotonTargets.Others);
    //    playerQuitNetworkGame();
    //}

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

    //[PunRPC]
    //public void forfeit()
    //{
    //    Debug.Log("Player left");
    //    playerQuitNetworkGame();        
    //}

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
