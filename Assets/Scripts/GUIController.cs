using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    
    const string WIN_TEXT = "You Win!";
    const string LOSE_TEXT = "You Lost!";
    //an instance of the GUIController used to call public methods
    public static GUIController GUIReference;
    //Instances of gameboard objects that the controller must manipulate
    public GameObject playerPawn, opponentPawn, ghostSpace,
        ghostWall, wall, hoverpadMaster, winPanel, chatPanel,
        settingsPanel, helpPanel;
    public Text winText, messageText, inputText;
    public Button winButton, chatButton;
    public ScrollRect chatScrollRect;
    public bool animationFinished = false;

    //tile objects that are invisible until pawn is clicked
    private List<GameObject> ghostPlayerMoves;   
    private string playerNumber = "Player1";
    private NetworkController networkController;
    private bool pawnClicked;
    private bool playerTurn;
    private readonly static Dictionary<char, float> COLUMN_MIDPOINT = new Dictionary<char, float>()
    {
        { 'a', .75f },
        { 'b', 2.75f },
        { 'c', 4.75f },
        { 'd', 6.75f },
        { 'e', 8.75f },
        { 'f', 10.75f },
        { 'g', 12.75f },
        { 'h', 14.75f },
        { 'i', 16.75f },
    };
    private readonly static Dictionary<int, float> ROW_MIDPOINT = new Dictionary<int, float>()
    {
        { 1, .75f },
        { 2, 2.75f },
        { 3, 4.75f },
        { 4, 6.75f },
        { 5, 8.75f },
        { 6, 10.75f },
        { 7, 12.75f },
        { 8, 14.75f },
        { 9, 16.75f },
    };
    private Stack<GameObject> opponentWallPoolStack, playerWallPoolStack;
    private string playerMove;


    private void Start()
    {
        if (GameData.IsAIGame)
        {
            chatButton.gameObject.SetActive(false);
        }
        FindInactiveObject(GameData.Scene).SetActive(true);

        //initialize wall pool stacks
        playerWallPoolStack = new Stack<GameObject>(GameObject.FindGameObjectsWithTag("PlayerWallPool"));
        opponentWallPoolStack = new Stack<GameObject>(GameObject.FindGameObjectsWithTag("OpponentWallPool"));
    }

    ////Calculate what is being clicked
    ////Returns Z0 if not in range
    private string FindCoordinate(float x, float y)
    {
        string coordinate = "Z0";
        int xInt = Mathf.FloorToInt(x * 2);
        int yInt = Mathf.FloorToInt(y * 2);

        bool isRowSpace = !(y * 2 % 4 >= 3);
        bool isColSpace = !(x * 2 % 4 >= 3);

        int row = yInt / 4;
        int col = xInt / 4;

        if (!isRowSpace || !isColSpace)
        {
            string dir;
            if (isRowSpace)
            {
                dir = "v";
                row--;
            }
            else
            {
                dir = "h";
            }
            coordinate = FindWallCoordinate(row, col, dir);
        }
        else
        {
            coordinate = $"{System.Convert.ToChar('a' + col)}{row + 1}";
        }

        return coordinate;
    }
    private string FindWallCoordinate(int row, int col, string dir)
    {
        string coordinate = "Z0";
        if (!(row < 0 || col == 8 || dir != "v" && dir != "h"))
        { 
            coordinate = $"{System.Convert.ToChar('a' + col)}{row + 1}{dir}";
        }
        return coordinate;
    }

    void Awake()
    {
        GUIReference = this;
        pawnClicked = false;
        playerTurn = false;
        ghostPlayerMoves = new List<GameObject>();
    }

    public void StartPlayerTurn(string move, List<string> validWalls, List<string> validMoves)
    {
        if (move.Length == 3)
        {
            PlaceOpponentWall(move);
            animationFinished = false;
        }
        else if (move.Length == 2)
        {
            MoveOpponentPawn(move);
            animationFinished = false;
        }

        ActivateGhostMoves(validMoves);
        ActivateHoverPads(validWalls);
        playerTurn = true;
    }

    private void PlaceOpponentWall(string coordinate)
    {
        //should make the mage do a little move before placing a wall
        MageBehavior.Reference.Summon();
        Debug.Log("summon should have been called");

        TakeFromWallPool(false);
        GameObject newWall = Instantiate(wall);
        Vector3 position = FindInactiveObject(coordinate).transform.position;
        float rotation;
        if (coordinate[2] == 'v')
        {
            rotation = 90;
            position.z--;
        }
        else
        {
            rotation = 0;
            position.x++;
        }
        newWall.transform.rotation = Quaternion.Euler(0, rotation, 0);
        newWall.GetComponent<WallAnimation>().SetDestination(position, false);
        newWall.SetActive(true);
    }

    public void PlacePlayerWall(string move)
    {
        if (playerTurn && animationFinished)
        {
            playerTurn = false;
            TakeFromWallPool(true);
            DeactivateGhostWall();
            GameObject newWall = Instantiate(wall);
            newWall.transform.rotation = ghostWall.transform.rotation;
            newWall.SetActive(true);
            newWall.GetComponent<WallAnimation>().SetDestination(ghostWall.transform.position, true);
            playerMove = move;
        }
    }

    private void MoveOpponentPawn(string coordinate)
    {
        //should make the mage play the attack animation
        MageBehavior.Reference.Attack();
        Vector3 newPosition = GetPositionFromCoordinate(coordinate);
        opponentPawn.GetComponent<PawnAnimation>().SetDestination(newPosition, false);
    }

    public void MovePlayerPawn(GameObject ghost)
    {
        playerTurn = false;
        DestroyGhostMoves();
        Vector3 position = ghost.transform.position;
        playerPawn.GetComponent<PawnAnimation>().SetDestination(position, true);
        playerMove = FindCoordinate(position.x, position.z);
    }

    private void ActivateHoverPads(List<string> coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            FindInactiveObject(coordinate).SetActive(true);
        }
    }    

    public void ActivateGhostWall(Vector3 position, char orientation)
    {
        if (playerTurn && !pawnClicked && animationFinished)
        {
            float rotation;
            if (orientation == 'v')
            {
                rotation = 90;
                position.z--;
            }
            else
            {
                rotation = 0;
                position.x++;
            }
            ghostWall.transform.SetPositionAndRotation(position, Quaternion.Euler(0, rotation, 0));
            ghostWall.SetActive(true);
        }
    }

    public void DeactivateGhostWall()
    {
        ghostWall.SetActive(false);
    }

    private void ActivateGhostMoves(List<string> moves)
    {
        foreach (var move in moves)
        {
            CreateGhostMove(move);
        }
    }

    //Create phyisical plane
    //add to list
    private void CreateGhostMove(string move)
    {
        GameObject ghost = Instantiate(ghostSpace);
        ghost.SetActive(false);
        ghost.transform.position = GetPositionFromCoordinate(move);
        ghostPlayerMoves.Add(ghost);
    }

    private Vector3 GetPositionFromCoordinate(string coordinate)
    {
        if (coordinate != "")
        {
            int row = (int)char.GetNumericValue(coordinate[1]);
            float x = COLUMN_MIDPOINT[coordinate[0]];
            float y = ROW_MIDPOINT[row];
            return new Vector3(x, 0, y);
        }
        else
        {
            return new Vector3(); ;
        }
    }

    public void AnimationCompleted(bool isPlayer)
    {
        animationFinished = true;
        if (isPlayer)
        {
            EndTurn(playerMove);
        }
    }

    //Destroys ghost walls and moves
    //changes player bool
    private void EndTurn(string move)
    {
        DestroyGhostMoves();
        DeactivateHoverPads();
        playerTurn = false;
        pawnClicked = false;
        Debug.Log("Player Move: " + move);
        GameController.GCInstance.RecieveMoveFromPlayer(move);
    }

    private void DeactivateHoverPads()
    {
        foreach (var hoverpad in GameObject.FindGameObjectsWithTag("HoverPad"))
        {
            hoverpad.SetActive(false);
        }
    }

    private void DestroyGhostMoves()
    {
        foreach (var ghost in ghostPlayerMoves)
        {
            Destroy(ghost);
        }
        ghostPlayerMoves.Clear();
    }
    
    public void ShowGhostMoves()
    {
        if (playerTurn && animationFinished)
        {
            pawnClicked = !pawnClicked;
            foreach (var space in ghostPlayerMoves)
            {
                space.SetActive(pawnClicked);
            }
        }
    }

    private GameObject FindInactiveObject(string name)
    {
        var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in gameObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    public void GameOver(bool isWinner, string move = "")
    {
        if (move != "")
        {
            MoveOpponentPawn(move);
        }
        winPanel.SetActive(true);
        winText.text = isWinner ? WIN_TEXT : LOSE_TEXT;
        winButton.onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void TakeFromWallPool(bool isPlayer)
    {
        if (isPlayer)
        {
            playerWallPoolStack.Pop().GetComponent<WallAnimation>().RemoveWall();
        }
        else
        {
            opponentWallPoolStack.Pop().GetComponent<WallAnimation>().RemoveWall();
        }
    }

    public void ReceiveMessage(string message)
    {
        //update message window
        UpdateChat("Them:" + message);     
    }

    public void SendChat()
    {
        if (inputText.text != "")
        {
            string message = "You: " + inputText.text;
            string messageToSend = inputText.text;
            inputText.text = "";
            UpdateChat(message);
            //NEW CHANGE
            GameData.NetworkController.onMessageToSend(messageToSend);
        }
    }

    private void UpdateChat(string message)
    {
        Text newChat = Instantiate(messageText, chatScrollRect.content);
        newChat.text = message;
    }

    public void ShowChatMenu()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }

    public void ShowHelpMenu()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }

    public void ShowSettingsMenu()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    
}