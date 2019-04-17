using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    #region public variables
    //an instance of the GUIController used to call public methods
    public static GUIController Instance;
    //Instances of gameboard objects that the controller must manipulate
    public GameObject playerPawn, opponentPawn, ghostSpace,
        ghostWall, wall, hoverpadMaster, winComputerPanel, winOnlinePanel, loseComputerPanel, loseOnlinePanel, chatPanel,
        settingsPanel, helpPanel, confirmForfeitPanel, opponentDisconnectedPanel, opponentFofeitedPanel,
        disconnectedFromNetworkPanel, playerTurnPanel, opponentTurnPanel,
        adventureWinPanel, adventureLosePanel, moveTimerPanel, gameControlPanel,
        storyBefore, storyAfter, chatHelpPanel, helpHelpPanel, helpHelpWithoutChatPanel, endStoryPanel;
    //panels in the help panel tab view
    public GameObject rulesPanel, gameplayPanel;
    public GameObject levelLoader;
    public GameObject RestartGameButton;
    public Text messageText;
    public InputField chatInputField;
    public Button winButton, chatButton;
    public ScrollRect chatScrollRect;
    public Slider musicVolumeSlider, sfxVolumeSlider;
    public bool animationFinished = false;
    public bool gameOver = false;
    public bool playerTurn;
    public Toggle fullscreenToggle;
    public AudioSource clickSound;
  
    #endregion
    #region private variables
    //tile objects that are invisible until pawn is clicked
    private string scene;
    private bool pauseGame;
    private bool pawnClicked;
    private string playerMove;
    private musicScript musicReference;
    private List<GameObject> ghostPlayerMoves;
    private Stack<GameObject> opponentWallPoolStack, playerWallPoolStack;
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
    #endregion
    #region unity
    void Awake()
    {
        Instance = this;
        pawnClicked = false;
        playerTurn = false;
        pauseGame = false;
        ghostPlayerMoves = new List<GameObject>();
        scene = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {        
        if (GameData.IsAIGame)
        {
            chatButton.gameObject.SetActive(false);
            chatHelpPanel.SetActive(false);
            helpHelpPanel.SetActive(false);
            helpHelpWithoutChatPanel.SetActive(true);
           
            chatHelpPanel.SetActive(false);            
        }
        else
        {
            RestartGameButton.SetActive(false);
        }

        if (GameData.InAdventureMode)
        {
            //display panels
            storyBefore.SetActive(true);
            gameControlPanel.SetActive(false);
            playerTurnPanel.SetActive(false);
            opponentTurnPanel.SetActive(false);
            pauseGame = true;

            //Start Beach scene camera on the board
            if (GameData.AdventureProgress == 1)
            {
                CameraBehavior.reference.Idle();
            }
        }
        else
        {
            CameraBehavior.reference.AnimateCamera();
        }

        //initialize wall pool stacks
        playerWallPoolStack = new Stack<GameObject>(GameObject.FindGameObjectsWithTag("PlayerWallPool"));
        opponentWallPoolStack = new Stack<GameObject>(GameObject.FindGameObjectsWithTag("OpponentWallPool"));

        //set music and sfx volume
        musicVolumeSlider.value = PlayerData.MusicVolume;
        sfxVolumeSlider.value = PlayerData.SfxVolume;
        fullscreenToggle.isOn = Screen.fullScreen;
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
        else if (!pauseGame)
        {
            opponentTurnPanel.SetActive(false);
            playerTurnPanel.SetActive(true);
        }

        ActivateGhostMoves(validMoves);
        ActivateHoverPads(validWalls);
        playerTurn = true;
    }
    public void PlacePlayerWall(Vector3 position, string move)
    {
        if (!pawnClicked && playerTurn && animationFinished && !pauseGame)
        {
            playerTurn = false;
            TakeFromWallPool(true);
            DeactivateGhostWall();
            GameObject newWall = Instantiate(wall);
            var transformation = GetPositionAndRotationFromHoverPad(position, move[2]);
            newWall.transform.rotation = transformation.Item2;
            newWall.SetActive(true);
            newWall.GetComponent<WallAnimation>().Animate(transformation.Item1, true);
            playerMove = move;
        }
    }
    public void MovePlayerPawn(GameObject ghost)
    {
        if (playerTurn && !pauseGame)
        {
            playerTurn = false;
            DestroyGhostMoves();
            playerPawn.GetComponent<PawnBehavior>().SetOpaque();
            opponentPawn.GetComponent<PawnBehavior>().SetOpaque();
            Vector3 position = ghost.transform.position;
            playerPawn.GetComponent<PawnAnimation>().Animate(position, true);
            playerMove = FindCoordinate(position.x, position.z);
        }
    }
    public bool IsPlayerTurn()
    {
        return playerTurn && animationFinished && !pauseGame;
    }
    public bool IsCameraFinished()
    {
        return FindObjectOfType<Camera>().GetComponent<CameraBehavior>().IsFinishedAnimating;
    }
    public void AnimationCompleted(bool isPlayer)
    {
        animationFinished = true;
        if (isPlayer)
        {
            EndTurn(playerMove);
            if (!GameData.IsAIGame)
            {
               //GameData.NetworkController.resetTimer();
            }
        }
        else
        {
            opponentTurnPanel.SetActive(false);
            playerTurnPanel.SetActive(true);
        }
    }

    private void EndTurn(string move)
    {
        playerTurnPanel.SetActive(false);
        opponentTurnPanel.SetActive(true);
        DestroyGhostMoves();
        DeactivateHoverPads();
        playerPawn.GetComponent<MeshCollider>().enabled = true;
        playerTurn = false;
        pawnClicked = false;
        Debug.Log("Player Move: " + move);
        GameController.GCInstance.RecieveMoveFromPlayer(move);
    }
    #endregion

    #region opponent
    private void PlaceOpponentWall(string coordinate)
    {
        //should make the mage do a little move before placing a wall
        PlayRaiseWall();
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
    }
    private void MoveOpponentPawn(string coordinate)
    {
        //should make the mage play the attack animation
        PlayMovePawn();
        StartCoroutine("MoveOpponentPawnOnTime", coordinate);
    }

    private IEnumerator MoveOpponentPawnOnTime(string coordinate)
    {
        yield return new WaitForSeconds(.5f);
        Vector3 newPosition = GetPositionFromCoordinate(coordinate);
        opponentPawn.GetComponent<PawnAnimation>().Animate(newPosition, false);
    }

    

    #endregion
    #region opponent animations
    private void PlayLose()
    {
        Debug.Log("Scene: " + scene);
        if (scene == "JungleScene")
        {
            MageBehavior.Reference.Lose();
        }
        else if (scene == "BeachScene")
        {
            GruntBehavior.Reference.Lose();
        }

    }
    private void PlayRaiseWall()
    {
        Debug.Log("Scene: " + scene);
        if (scene == "TempleScene")
        {
            KingBehavior.Reference.RaiseWall();
        }
        else if (scene == "JungleScene")
        {
            MageBehavior.Reference.RaiseWall();
        }
        else if (scene == "BeachScene")
        {
            GruntBehavior.Reference.RaiseWall();
        }

    }

    private void PlayMovePawn()
    {
        Debug.Log("Scene: " + scene);
        if (scene == "TempleScene")
        {
            KingBehavior.Reference.MovePawn();
        }
        else if (scene == "JungleScene")
        {
            MageBehavior.Reference.MovePawn();
        }
        else if (scene == "BeachScene")
        {
            GruntBehavior.Reference.MovePawn();
            //TODO:
            //Add skeleton grunt animation
        }

    }
    //private void PlayWin()
    //{
    //    Debug.Log("Scene: " + scene);
    //    if (scene == "TempleScene")
    //    {
    //        KingBehavior.Reference.MovePawn();
    //    }
    //    else if (scene == "JungleScene")
    //    {
    //        MageBehavior.Reference.Lose();
    //    }
    //    else if (scene == "BeachScene")
    //    {
    //        GruntBehavior.Reference.Lose();

    //    }

    //}

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
        if (playerTurn && !pawnClicked && animationFinished && !pauseGame)
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
            playerWallPoolStack.Pop().GetComponent<WallAnimation>().RemoveWallFromPool();
        }
        else
        {
            opponentWallPoolStack.Pop().GetComponent<WallAnimation>().RemoveWallFromPool();
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
        if (playerTurn && animationFinished && !pauseGame)
        {
            clickSound.Play();
            playerPawn.GetComponent<MeshCollider>().enabled = pawnClicked;
            pawnClicked = !pawnClicked;
            foreach (var space in ghostPlayerMoves)
            {
                space.SetActive(pawnClicked);
            }
            if (pawnClicked)
            {
                playerPawn.GetComponent<PawnBehavior>().SetTransparent();
                opponentPawn.GetComponent<PawnBehavior>().SetTransparent();
            }
            else
            {
                playerPawn.GetComponent<PawnBehavior>().SetOpaque();
                opponentPawn.GetComponent<PawnBehavior>().SetOpaque();
            }
        }
    }
    #endregion

    #region game end
    public void GameOver(bool isWinner, string move = "")
    {
        musicReference = musicScript.musicScriptReference;
        gameOver = true;
        if (move != "")
        {
            MoveOpponentPawn(move);
        }
        gameControlPanel.SetActive(false);
        opponentTurnPanel.SetActive(false);
        playerTurnPanel.SetActive(false);
        if (isWinner)
        {
            PlayLose();
            musicReference.playWin();
            if (GameData.InAdventureMode)
            {
                GameData.AdventureProgress++;
                adventureWinPanel.SetActive(true);
            }
            else if(GameData.IsAIGame)
            {
                winComputerPanel.SetActive(true);
            }
            else
            {
                winOnlinePanel.SetActive(true);
            }
        }
        else
        {
            musicReference.playLose();
            if (GameData.InAdventureMode)
            {
                adventureLosePanel.SetActive(true);
            }
            else if(GameData.IsAIGame)
            {
                loseComputerPanel.SetActive(true);
            }
            else
            {
                loseOnlinePanel.SetActive(true);
            }
        }
    }

    public void openConfirmForfeit()
    {
        pauseGame = true;
        confirmForfeitPanel.SetActive(true);
        if (!GameData.IsAIGame)
        {
            GameData.NetworkController.onSendForfeitMessage();
        }
    }

    public void closeConfirmForfeit()
    {
        pauseGame = false;
        confirmForfeitPanel.SetActive(false);
        if (!GameData.IsAIGame)
        {
            GameData.NetworkController.onSendNoQuitMessage();
        }
    }

    public void LeaveGame()
    {
        if (!GameData.IsAIGame)
        {
            Debug.Log("NetworkGame");
            //GameData.NetworkController.onSendForfeitMessage();
            GameData.NetworkController.gameOver();
        }
        else if(GameData.InAdventureMode == true)
        {
            GameData.InAdventureMode = false;
        }
        //SceneManager.LoadScene("MainMenu");
    }
    public void OpponentLeft(bool opponentForfeit)
    {
        //TODO:
        //Lock the MenuButtons as well
        
        //lock UI
        playerTurn = false;
     
        gameOver = true;
        if(moveTimerPanel.activeSelf)
        {
            moveTimerPanel.SetActive(false);
        }
        if (!opponentForfeit)
        {
            opponentDisconnectedPanel.SetActive(true);
            loseOnlinePanel.SetActive(false);
            winOnlinePanel.SetActive(false);
        }
        else
        {
            opponentFofeitedPanel.SetActive(true);
        }
        
    }

    public void openLostConnectionPanel()
    {
        if (moveTimerPanel.activeSelf)
        {
            moveTimerPanel.SetActive(false);
        }
        if(opponentDisconnectedPanel.activeSelf)
        {
            opponentDisconnectedPanel.SetActive(false);
        }
        //if()
        disconnectedFromNetworkPanel.SetActive(true);
        
    }

    public void closeLostConnectionPanel()
    {
        disconnectedFromNetworkPanel.SetActive(false);
    }

    public void onClickPlayAgain()
    {
        GameData.NetworkController.onSendReplayMessage();
    }
    #endregion

    #region menu
    public void ShowChatMenu()
    {
   
        if(!gameOver)
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
        foreach (var wall in opponentWallPoolStack)
        {
            wall.GetComponent<AudioSource>().volume = vol;
        }
        foreach (var wall in playerWallPoolStack)
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


    #region chat
    public void ReceiveMessage(string message)
    {
        //update message window
        UpdateChat(message);
        helpPanel.SetActive(false);
        settingsPanel.SetActive(false);
        chatPanel.SetActive(true);
    }

    public void SendChat()
    {
        if (chatInputField.text != "")
        {
            string messageToSend = "[" + PlayerData.PlayerName + "]: " + chatInputField.text;
            string messageToDisplay = "[You]: " + chatInputField.text;
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
    //we should be able to delete this but I'm leaving for now just in case we want it back -Brad
    //public void showRulesPanel()
    //{
    //    gameplayPanel.SetActive(false);
    //    rulesPanel.SetActive(true);
    //}
    //public void showGameplayPanel()
    //{
    //    rulesPanel.SetActive(false);
    //    gameplayPanel.SetActive(true);
    //}
    #endregion
    #region Move Timer panel
    //public void displayMoveTimerPanel()
    //{
    //    if (!disconnectedFromNetworkPanel.activeSelf && !opponentDisconnectedPanel.activeSelf)
    //    {
    //        moveTimerPanel.SetActive(true);
    //    }
    //}

    //public void closeMoveTimerPanel()
    //{
    //    moveTimerPanel.SetActive(false);
    //}
    #endregion
    #region story
    public void ContinueStory()
    {
        storyAfter.SetActive(true);
    }
    public void AdvanceLevel()
    {
        if (GameData.AdventureProgress == 2)
        {
            
            GameData.AIDifficulty = "intermediate";
            levelLoader.GetComponent<LevelLoader>().LoadLevel("JungleScene");
        }
        else if (GameData.AdventureProgress == 3)
        {
            
            GameData.AIDifficulty = "hard";
            levelLoader.GetComponent<LevelLoader>().LoadLevel("TempleScene");
        }
    }
    public void EndStory()
    {
        GameData.AdventureProgress = 0;
        GameData.InAdventureMode = false;
        levelLoader.GetComponent<LevelLoader>().LoadLevel("MainMenu");
    }
    public void LeaveStory()
    {
        GameData.InAdventureMode = false;
        levelLoader.GetComponent<LevelLoader>().LoadLevel("MainMenu");
    }

    public void ClickDismissStoryBefore()
    {
        storyBefore.SetActive(false);
        gameControlPanel.SetActive(true);
        playerTurnPanel.SetActive(true);
        pauseGame = false;
        if (GameData.AdventureProgress != 1)
        {
            CameraBehavior.reference.AnimateCamera();
        }
    }
    public void ClickDismissStoryAfter()
    {
        storyAfter.SetActive(false);
        AdvanceLevel();
    }
    public void ShowEndStoryPanel()
    {
        storyAfter.SetActive(false);
        endStoryPanel.SetActive(true);
    }
    #endregion
}