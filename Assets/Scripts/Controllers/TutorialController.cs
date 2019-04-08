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
        ghostWall, wall, hoverpadMaster, winPanel, gameControlPanel,
        settingsPanel, helpPanel, playerTurnPanel, opponentTurnPanel,
        clickReceiverPanel, adventureWinPanel, storyOpening, storyTutorial;
    //panels in the help panel tab view
    public GameObject rulesPanel, gameplayPanel;
    public GameObject levelLoader;
    public Text messageText;
    public Button winButton;
    public Slider musicVolumeSlider, sfxVolumeSlider;
    public bool animationFinished = false;
    public bool gameOver = false;
    //tutorial panels
    public GameObject tutorialPanel;
    public GameObject gameObjectivePanel, movingPawnPanel,  wallPlacementPanel, 
        miscRulesPanel, helpHelpPanel, skipButton;
    
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
            gameControlPanel.SetActive(false);
            tutorialPanel.SetActive(false);
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
        tutorialPanelQueue.Add(gameObjectivePanel);
        tutorialPanelQueue.Add(movingPawnPanel);
        tutorialPanelQueue.Add(wallPlacementPanel);
        tutorialPanelQueue.Add(miscRulesPanel);  


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
            //GameData.AdventureProgress++;
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
                Debug.Log(tutorialProgress);
                advancePanels();
                StartPlayerTurn("", returnAllWallMoves(), new List<string>());
                break;
            case 3:
                Debug.Log(tutorialProgress);
                advancePanels();
                activateClickToContinue();
                break;
            case 4:
                tutorialPanel.SetActive(false);
                skipButton.SetActive(false);
                if(GameData.InAdventureMode)
                {
                    adventureWinPanel.SetActive(true);
                }
                else
                {
                    winPanel.SetActive(true);
                }
                //end turorial panel pops up
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
            
            
        }
    }
    public void MovePlayerPawn(GameObject ghost)
    {
        
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
        if(tutorialProgress == 1)
        {
            MoveOpponentPawn("e8");
        }
        ProgressController();
        //else if(tutorialProgress == 8)
        //{
        //    Debug.Log("Opponent Moves");
        //    MoveOpponentPawn("f7");
        //    List<string>allowedPawnMoves = new List<string>();
        //    allowedPawnMoves.Add("f8");
        //    Debug.Log("Starting player turn");
        //    StartPlayerTurn("", new List<string>(), allowedPawnMoves);
        //}
        //else if(tutorialProgress == 10)
        //{
        //    Debug.Log("Opponent Moves");
        //    MoveOpponentPawn("e7");
        //}
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
        GameData.AdventureProgress++;
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
        gameControlPanel.SetActive(true);
        tutorialPanel.SetActive(true);
        CameraBehavior.reference.AnimateCamera();
    }

    #endregion

    #region menu
 

    public void ShowHelpMenu()
    {
        if (!gameOver)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowSettingsMenu()
    {
        if (!gameOver)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            helpPanel.SetActive(false);
        }
    }
    #endregion

    #region settings

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

    public void ToggleFullscreen(Toggle isFullscreen)
    {
        var rezs = Screen.resolutions;
        foreach (var rez in rezs)
        {
            Debug.Log(rez);
        }
        int width = rezs[rezs.Length - 1].width;
        int height = rezs[rezs.Length - 1].height;
        Screen.SetResolution(width, height, isFullscreen.isOn);
    }

    #endregion

 
}