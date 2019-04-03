using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    #region public variables
    //Instances of gameboard objects that the controller must manipulate
    public static TutorialController Instance;
    public GameObject playerPawn, opponentPawn, ghostSpace,
        ghostWall, wall, hoverpadMaster, winPanel, chatPanel,
        settingsPanel, helpPanel, playerTurnPanel, opponentTurnPanel,
        clickReceiverPanel, adventureWinPanel, storyOpening, storyTutorial, helpHelpPanel2;
    //panels in the help panel tab view
    public GameObject rulesPanel, gameplayPanel;
    public GameObject levelLoader;
    public Text messageText;
    public InputField chatInputField;
    public Button winButton, chatButton;
    public ScrollRect chatScrollRect;
    public Slider musicVolumeSlider, sfxVolumeSlider;
    public bool animationFinished = false;
    public bool gameOver = false;
    //tutorial panels
    public GameObject tutorialPanel;
    public GameObject introPanel, movingPawn1Panel, movingPawn2panel, wallPlacement1Panel,
                     wallPlacement2Panel, wallplacement3Panel, transitionPanel, turningTablesPanel,
                     jumping1Panel, jumping2Panel, invalidWallPlacementPanel, endGameplayPanel,
                     settingsTutorialPanel, helpTutorialPanel, chat1TutorialPanel, chat2TutorialPanel,
                     endTurorialPanel, chatHelpPanel, helpHelpPanel;
    
    #endregion

    #region private variables
    //tile objects that are invisible until pawn is clicked
    private List<GameObject> ghostPlayerMoves;
    private bool pawnClicked;
    private bool playerTurn;
    private bool allowPawnClick = false;
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
    private List<GameObject> opponentWallPool, playerWallPool;
    private int opponentWallPoolIndex = 0;
    private int playerWallPoolIndex = 0;
    private string playerMove;
    private int tutorialProgress = 0;
    private List<GameObject> tutorialPanelQueue = new List<GameObject>();
    private List<GameObject> wallsPlaced = new List<GameObject>();

    #endregion

    #region unity
    void Awake()
    {
        //don't think we need this thing anymore.
        GameData.IsTutorial = true;
        /////////////////////////////
        Instance = this;
        pawnClicked = false;
        playerTurn = false;
        ghostPlayerMoves = new List<GameObject>();
    }

    private void Start()
    {
        if (GameData.InAdventureMode)
        {
            //display panels
            storyOpening.SetActive(true);
            chatButton.gameObject.SetActive(false);
            helpHelpPanel.SetActive(false);
            helpHelpPanel2.SetActive(true);
            chatHelpPanel.SetActive(false);  
        }
        else
        {
            CameraBehavior.reference.AnimateCamera();
        }
        //initialize wall pool stacks
        playerWallPool = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerWallPool"));
        opponentWallPool = new List<GameObject>(GameObject.FindGameObjectsWithTag("OpponentWallPool"));

        //set music and sfx volume
        musicVolumeSlider.value = PlayerData.MusicVolume;
        sfxVolumeSlider.value = PlayerData.SfxVolume;

        //create tutorial panel queue
        tutorialPanelQueue.Add(introPanel);                 //0
        tutorialPanelQueue.Add(movingPawn1Panel);           //1
        tutorialPanelQueue.Add(movingPawn2panel);           //2
        tutorialPanelQueue.Add(wallPlacement1Panel);        //3
        tutorialPanelQueue.Add(wallPlacement2Panel);        //4
        tutorialPanelQueue.Add(wallplacement3Panel);        //5
        tutorialPanelQueue.Add(transitionPanel);            //6
        tutorialPanelQueue.Add(turningTablesPanel);         //7
        tutorialPanelQueue.Add(jumping1Panel);              //8
        tutorialPanelQueue.Add(jumping2Panel);              //9
        tutorialPanelQueue.Add(invalidWallPlacementPanel);  //10
        tutorialPanelQueue.Add(endGameplayPanel);           //11
        tutorialPanelQueue.Add(settingsTutorialPanel);      //12
        tutorialPanelQueue.Add(helpTutorialPanel);          //13
        tutorialPanelQueue.Add(chat1TutorialPanel);         //14
        tutorialPanelQueue.Add(chat2TutorialPanel);         //15
        tutorialPanelQueue.Add(endTurorialPanel);           //16

    }
    #endregion

    #region conversions
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

    public Tuple<Vector3, Quaternion> GetPositionAndRotationFromHoverPad(Vector3 position, char orientation)
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
        return new Tuple<Vector3, Quaternion>(position, Quaternion.Euler(0, rotation, 0));
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

    #endregion

    #region tutorial controller
    public void SkipTutorial()
    {
        Debug.Log("Clicked: Skip");
        Debug.Log("skipped");
        if (GameData.InAdventureMode)
        {
            GameData.AdventureProgress++;
            ContinueStory();
        }
        else
        {
            LeaveGame();
        }
    }

    private void ProgressController()
    {
        tutorialProgress++;
        switch (tutorialProgress)
        {
            case 1:
                advancePanels();
                Debug.Log(tutorialProgress);
                //deactivate all the hover pads becuase they are all active at the start
                DeactivateHoverPads();
                //activate ability to click on pawn
                List<string> allowedPawnMoves = new List<string>();
                allowedPawnMoves.Add("d1");
                allowedPawnMoves.Add("e2");
                allowedPawnMoves.Add("f1");
                allowPawnClick = true;
                StartPlayerTurn("", new List<string>(), allowedPawnMoves);
                break;
            case 2:
                allowPawnClick = false;
                advancePanels();
                Debug.Log(tutorialProgress);
                //happens when player clicks thier pawn
                break;
            case 3:
                advancePanels();
                //happens when player moves their pawn
                Debug.Log(tutorialProgress);
                //the enemey makes a move during this index
                activateClickToContinue();
                break;
            case 4:
                advancePanels();
                //happens aftert the enemy has finished making a move and 
                //the player clicked to continue
                Debug.Log(tutorialProgress);
                //start the player's turn with only walls as a placement possibility
                StartPlayerTurn("", returnAllWallMoves(), new List<string>());                
                break;
            case 5:
                advancePanels();
                Debug.Log(tutorialProgress);
                activateClickToContinue();
                break;
            case 6:
                advancePanels();
                Debug.Log(tutorialProgress);
                activateClickToContinue();
                break;
            case 7:
                advancePanels();
                CleanBoard();
                //this should be where the board resets
                List<string> playerWallsToPlace = new List<string>();
                playerWallsToPlace.Add("c7v");
                playerWallsToPlace.Add("d8h");
                List<string> opponentWallsToPlace = new List<string>();
                opponentWallsToPlace.Add("e6h");
                opponentWallsToPlace.Add("f7v");
                SetBoard("e7", "d7", playerWallsToPlace, opponentWallsToPlace);
                //activate the player's turn but only allow one wall to be placed
                List<string> placeableWalls = new List<string>();
                placeableWalls.Add("c6h");                
                StartPlayerTurn("", placeableWalls, new List<string>());
                Debug.Log(tutorialProgress);              
                break;
            case 8:
                allowPawnClick = true;
                advancePanels();
                //opponent jumps the player
                Debug.Log(tutorialProgress);
                break;
            case 9:
                allowPawnClick = false;
                advancePanels();
                Debug.Log(tutorialProgress);
                break;
            case 10:
                advancePanels();
                Debug.Log(tutorialProgress);
                activateClickToContinue();
                break;
            case 11:
                advancePanels();
                Debug.Log(tutorialProgress);
                break;
            case 12:
                advancePanels();
                Debug.Log(tutorialProgress);
                break;
            case 13:
                advancePanels();
                Debug.Log(tutorialProgress);
                activateClickToContinue();
                break;
            case 14:
                helpPanel.SetActive(false);
                if (GameData.InAdventureMode)
                {
                    Debug.Log("InAdventureMode triggered");
                    tutorialPanelQueue[13].SetActive(false);
                    //tutorialPanelQueue[16].SetActive(true);
                    tutorialProgress = 15;
                    ProgressController();
                }
                else
                {
                    advancePanels();
                    Debug.Log(tutorialProgress);
                }
                
                break;
            case 15:
                advancePanels();
                Debug.Log(tutorialProgress);
                activateClickToContinue();
                break;
            case 16:
                advancePanels();
                chatPanel.SetActive(false);
                Debug.Log(tutorialProgress);
                List<string> winningMove = new List<string>();
                winningMove.Add("f9");
                allowPawnClick = true;
                StartPlayerTurn("", new List<string>(), winningMove );
                break;
            case 17:
                allowPawnClick = false;
                Debug.Log(tutorialProgress);
                tutorialPanel.SetActive(false);
                //end the game
                GameOver(true);
                break;
            default:
                break;
        }
    }

    public void Continue()
    {
        Debug.Log("Clicked continue");
        clickReceiverPanel.SetActive(false);
        ProgressController();
    }
    private void activateClickToContinue()
    {
        clickReceiverPanel.SetActive(true);
    }
    private void advancePanels()
    {
        tutorialPanelQueue[tutorialProgress - 1].SetActive(false);
        tutorialPanelQueue[tutorialProgress].SetActive(true);
    }
    private List<string> returnAllWallMoves()
    {
        List<string> walls = new List<string>();
        //build all avalible wall moves as strings
        string move;
        for (int i = 1; i < 9; i++)
        {
            move = "a" + i.ToString() + "v";
            walls.Add(move);
            move = "b" + i.ToString() + "v";
            walls.Add(move);
            move = "c" + i.ToString() + "v";
            walls.Add(move);
            move = "d" + i.ToString() + "v";
            walls.Add(move);
            move = "e" + i.ToString() + "v";
            walls.Add(move);
            move = "f" + i.ToString() + "v";
            walls.Add(move);
            move = "g" + i.ToString() + "v";
            walls.Add(move);
            move = "h" + i.ToString() + "v";
            walls.Add(move);
        }
        for (int i = 1; i < 9; i++)
        {
            move = "a" + i.ToString() + "h";
            walls.Add(move);
            move = "b" + i.ToString() + "h";
            walls.Add(move);
            move = "c" + i.ToString() + "h";
            walls.Add(move);
            move = "d" + i.ToString() + "h";
            walls.Add(move);
            move = "e" + i.ToString() + "h";
            walls.Add(move);
            move = "f" + i.ToString() + "h";
            walls.Add(move);
            move = "g" + i.ToString() + "h";
            walls.Add(move);
            move = "h" + i.ToString() + "h";
            walls.Add(move);
        }

        return walls;
    }
    
        #endregion

    #region player
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
        opponentTurnPanel.SetActive(false);
        playerTurnPanel.SetActive(true);
    }
    public void PlacePlayerWall(Vector3 position, string move)
    {
        if (playerTurn && animationFinished)
        {
            playerTurn = false;
            TakeFromWallPool(true);
            DeactivateGhostWall();
            GameObject newWall = Instantiate(wall);
            var transformation = GetPositionAndRotationFromHoverPad(position, move[2]);
            newWall.transform.rotation = transformation.Item2;
            newWall.SetActive(true);
            newWall.GetComponent<WallAnimation>().Animate(transformation.Item1, true);
            wallsPlaced.Add(newWall);
            playerMove = move;
            
            ProgressController();
        }
    }
    public void MovePlayerPawn(GameObject ghost)
    {
        ProgressController();
        if (playerTurn)
        {
            playerTurn = false;
            DestroyGhostMoves();
            playerPawn.GetComponent<PawnBehavior>().SetOpaque();
            Vector3 position = ghost.transform.position;
            playerPawn.GetComponent<PawnAnimation>().Animate(position, true);
            playerMove = FindCoordinate(position.x, position.z);
        }
    }
    public bool IsPlayerTurn()
    {
        return allowPawnClick && playerTurn && animationFinished;
    }
    public void AnimationCompleted(bool isPlayer)
    {
        animationFinished = true;
        if (isPlayer)
        {
            EndTurn(playerMove);
        }
    }

    private void EndTurn(string move)
    {
        playerTurnPanel.SetActive(false);
        opponentTurnPanel.SetActive(true);
        DestroyGhostMoves();
        DeactivateHoverPads();
        playerTurn = false;
        pawnClicked = false;
        //make the opponent move
        if(tutorialProgress == 3)
        {
            MoveOpponentPawn("e8");
        }
        else if(tutorialProgress == 8)
        {
            Debug.Log("Opponent Moves");
            MoveOpponentPawn("f7");
            List<string>allowedPawnMoves = new List<string>();
            allowedPawnMoves.Add("f8");
            Debug.Log("Starting player turn");
            StartPlayerTurn("", new List<string>(), allowedPawnMoves);
        }
        else if(tutorialProgress == 10)
        {
            Debug.Log("Opponent Moves");
            MoveOpponentPawn("e7");
        }
    }
    #endregion

    #region opponent
    private void PlaceOpponentWall(string coordinate)
    {
        //should make the mage do a little move before placing a wall
        CrocBehavior.Reference.RaiseWall();
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
        newWall.GetComponent<WallAnimation>().Animate(position, false);
        newWall.SetActive(true);
        wallsPlaced.Add(newWall);
    }
    private void MoveOpponentPawn(string coordinate)
    {
        //should make the mage play the attack animation
        CrocBehavior.Reference.MovePawn();
        Vector3 newPosition = GetPositionFromCoordinate(coordinate);
        opponentPawn.GetComponent<PawnAnimation>().Animate(newPosition, false);
    }
    #endregion

    #region setboard
    private void CleanBoard()
    {
        //Remove placed walls on board
        foreach (GameObject wall in wallsPlaced)
        {
            Destroy(wall);
        }
        wallsPlaced.Clear();
        //Add walls back to pool
        opponentWallPoolIndex = 0;
        playerWallPoolIndex = 0;
        foreach (GameObject wall in opponentWallPool)
        {
            wall.GetComponent<WallAnimation>().AddWallToPool();
        }
        foreach (GameObject wall in playerWallPool)
        {
            wall.GetComponent<WallAnimation>().AddWallToPool();
        }
    }

    private void SetBoard(string playerCoordinate,
                          string opponentCoordinate,
                          List<string> playerWalls,
                          List<string> opponentWalls)
    {
        PlacePawn(playerCoordinate, true);
        PlacePawn(opponentCoordinate, false);
        foreach (var wall in playerWalls)
        {
            PlaceWall(wall, true);
        }
        foreach (var wall in opponentWalls)
        {
            PlaceWall(wall, false);
        }
    }

    private void PlaceWall(string coordinate, bool isPlayer)
    {
        TakeFromWallPool(isPlayer);
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
        newWall.transform.SetPositionAndRotation(position, Quaternion.Euler(0, rotation, 0));
        newWall.SetActive(true);
        wallsPlaced.Add(newWall);
    }
    private void PlacePawn(string coordinate, bool isPlayer)
    {
        Vector3 newPosition = GetPositionFromCoordinate(coordinate);
        if (isPlayer)
        {
            playerPawn.GetComponent<PawnAnimation>().SetPosition(newPosition);
        }
        else
        {
            opponentPawn.GetComponent<PawnAnimation>().SetPosition(newPosition);
        }
    }
    #endregion

    #region walls

    private void ActivateHoverPads(List<string> coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            FindInactiveObject(coordinate).SetActive(true);
        }
    }

    public void ActivateGhostWall(Vector3 position, char orientation)
    {
        if (playerTurn && animationFinished)
        {
            var transformation = GetPositionAndRotationFromHoverPad(position, orientation);
            ghostWall.transform.SetPositionAndRotation(transformation.Item1, transformation.Item2);
            ghostWall.SetActive(true);
        }
    }

    public void DeactivateGhostWall()
    {
        ghostWall.SetActive(false);
    }
    private void DeactivateHoverPads()
    {
        foreach (var hoverpad in GameObject.FindGameObjectsWithTag("HoverPad"))
        {
            hoverpad.SetActive(false);
        }
    }
    private void TakeFromWallPool(bool isPlayer)
    {
        if (isPlayer)
        {
            playerWallPool[playerWallPoolIndex].GetComponent<WallAnimation>().RemoveWallFromPool();
            playerWallPoolIndex++;
        }
        else
        {
            opponentWallPool[opponentWallPoolIndex].GetComponent<WallAnimation>().RemoveWallFromPool();
            opponentWallPoolIndex++;
        }
    }

    #endregion

    #region moves
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
        if (allowPawnClick && playerTurn && animationFinished)
        {
            Debug.Log("pawn was Clicked");
           
            pawnClicked = !pawnClicked;
            foreach (var space in ghostPlayerMoves)
            {
                space.SetActive(pawnClicked);
            }
            if (pawnClicked)
            {
                playerPawn.GetComponent<PawnBehavior>().SetTransparent();
            }
            else
            {
                playerPawn.GetComponent<PawnBehavior>().SetOpaque();
            }
            if (tutorialProgress == 1 || tutorialProgress == 8)
            {
                ProgressController();
            }
        }
    }
    #endregion

    #region game end
    public void GameOver(bool isWinner, string move = "")
    {
        gameOver = true;
        if (move != "")
        {
            MoveOpponentPawn(move);
        }
        if (isWinner)
        {
            if(GameData.InAdventureMode)
            {
                GameData.AdventureProgress++;
                adventureWinPanel.SetActive(true);
            }
            else
            {
                winPanel.SetActive(true);
            }
        }
    }
    public void LeaveGame()
    {
        if(GameData.InAdventureMode)
        {
            GameData.InAdventureMode = false;
        }
        levelLoader.GetComponent<LevelLoader>().LoadLevel("MainMenu");
    }

    public void ContinueStory()
    {
        GameData.IsTutorial = false;
        GameData.AIDifficulty = "easy";
        Debug.Log("Adventure Progress: " + GameData.AdventureProgress);
        levelLoader.GetComponent<LevelLoader>().LoadLevel("BeachScene");
    }
    public void LeaveStory()
    {
        GameData.InAdventureMode = false;
        levelLoader.GetComponent<LevelLoader>().LoadLevel("MainMenu");
    }
    
    public void ClickOpeningToTutorial()
    {
        storyOpening.SetActive(false);
        storyTutorial.SetActive(true);
    }
    public void ClickDissmissTutorialStory()
    {
        //continue
        storyTutorial.SetActive(false);
        CameraBehavior.reference.AnimateCamera();
    }

    #endregion

    #region menu
    public void ShowChatMenu()
    {
        if (!gameOver)
        {
            chatInputField.Select();
            chatInputField.ActivateInputField();
            chatPanel.SetActive(!chatPanel.activeSelf);
            helpPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowHelpMenu()
    {
        if (!gameOver)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
            settingsPanel.SetActive(false);
            chatPanel.SetActive(false);
            if(GameData.InAdventureMode)
            {
                chatHelpPanel.SetActive(false);
                //todo:
                //Move the help button panel up
            }
        }
    }

    public void ShowSettingsMenu()
    {
        if (!gameOver)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            helpPanel.SetActive(false);
            chatPanel.SetActive(false);
        }
    }
    #endregion

    #region settings
    public void ShowSettings()
    {
        if(tutorialProgress == 11)
        {
            ProgressController();
            settingsPanel.SetActive(true);
        }
    }

    public void UpdatePlayerMusicVolume(float vol)
    {
        PlayerData.MusicVolume = vol;
    }

    public void UpdatePlayerSFXVolume(float vol)
    {
        PlayerData.SfxVolume = vol;
    }

    //set wall volume
    public void SetWallVolume(float vol)
    {
        foreach (var wall in opponentWallPool)
        {
            wall.GetComponent<AudioSource>().volume = vol;
        }
        foreach (var wall in playerWallPool)
        {
            wall.GetComponent<AudioSource>().volume = vol;
        }
    }



    #endregion


    #region chat
    public void ShowChat()
    {
        if(tutorialProgress == 14)
        {
            ProgressController();
            chatPanel.SetActive(true);
            
        }
    }

    public void ReceiveMessage(string message)
    {
        //update message window
        UpdateChat(message);
        ShowChatMenu();
    }

    public void SendChat()
    {
        if (chatInputField.text != "")
        {
            string messageToSend = PlayerPrefs.GetString("PlayerName") + ": " + chatInputField.text;
            string messageToDisplay = "You: " + chatInputField.text;
            chatInputField.text = "";
            UpdateChat(messageToDisplay);
            GameData.NetworkController.onMessageToSend(messageToSend);
        }
    }

    private void UpdateChat(string message)
    {
        Text newChat = Instantiate(messageText, chatScrollRect.content);
        newChat.text = message;
    }
    #endregion

    #region helpPanel
    public void ShowHelp()
    {
        if(tutorialProgress == 12)
        {

            settingsPanel.SetActive(false);
            ProgressController();
            helpPanel.SetActive(true);

        }
    }
    public void showRulesPanel()
    {
        gameplayPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }
    public void showGameplayPanel()
    {
        rulesPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }
        #endregion
}