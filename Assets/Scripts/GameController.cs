using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArtificialInteligence;
using Board;
using GameCore4.GameCore;
using GameCore4.HelperClasses;
using System.Threading;


public class GameController : MonoBehaviour
{
    public static GameController GCInstance;
    private AIBoard Board { get; set; }
    private AIController AIController { get; set; }
    private bool playerTurn {get; set;}
    private bool aiGame {get; set;}
    private string aiMove = "";
    private string playerMove = "";
    private bool aiHasNewMove = false;
    // Start is called before the first frame update
    void Start()
    {
        //do we still need this????
        DontDestroyOnLoad(gameObject);  
        //???????????????????????????

        GCInstance = this;
        Board = new AIBoard();
        AIController = new AIController();
        playerTurn = GameData.PlayerGoesFirst;
        aiGame = GameData.IsAIGame;
        if(aiGame)
        {
            if (GameData.AIDifficulty == "easy")
            {
                AIController.SetAIEasy();
            }
            else if (GameData.AIDifficulty == "hard")
            {
                AIController.SetAIHard();
            }
        }

        if(playerTurn)
        {
            GUIController.GUIReference.StartPlayerTurn("", Board.GetWallMoves(), Board.GetPawnMoves());
        }
        else if(aiGame)
        {
            Board.PlayerTwoGoesFirst();
            StartAIThread();
        }
        else
        {
            Board.PlayerTwoGoesFirst();
        }
        
    }
    //called by the GUIController when the player has finished making a move
    public void RecieveMoveFromPlayer(string move)
    {
        playerMove = move;
        Board.MakeMove(move);
        if (Board.IsWinner())
        {
            GUIController.GUIReference.GameOver(true, "");
            if(!aiGame)
            {
                GameData.NetworkController.onMoveToSend(move);
            }
        }
        else
        {
            playerTurn = false;
            if (aiGame)
            {
                StartAIThread();
            }
            else
            {
                GameData.NetworkController.onMoveToSend(move);
            }
        }
    }

    //called by the Networking Controller when the Network has
    //gotten a move from the opponent
    public void RecieveMoveFromNetwork(string move)
    {
        Board.MakeMove(move);
        if(Board.IsWinner())
        {
            GameData.NetworkController.gameOver();
            GUIController.GUIReference.GameOver(false, move);
        }
        else
        {
            playerTurn = true;
            GUIController.GUIReference.StartPlayerTurn(move, Board.GetWallMoves(), Board.GetPawnMoves());
        }
    }
   
    //creates a new thread that calls GetAIMove
    private void StartAIThread()
    {
        Thread thread = new Thread(GetAIMove) { IsBackground = true };
        thread.Start();
        InvokeRepeating("ReceiveAIMove", 0.5f, 0.25f);
    }

    //gets a move from the AI and adds it to the board
    private void GetAIMove()
    {
        if(playerMove == "")
        {
            aiMove = AIController.GetMove("gamestart");
        }
        else
        {
           aiMove = AIController.GetMove(playerMove);
        }
        Board.MakeMove(aiMove);
        aiHasNewMove = true;
    }
    //gets called on an interval until the AI is done. then gives the results of the move to the user
    private void ReceiveAIMove()
    { 
        if(aiHasNewMove)
        {
            aiHasNewMove = false;
            if (Board.IsWinner())
            {
                GUIController.GUIReference.GameOver(playerTurn, aiMove);
            }
            else
            {
                playerTurn = true;
                GUIController.GUIReference.StartPlayerTurn(aiMove, Board.GetWallMoves(), Board.GetPawnMoves());
            }
            CancelInvoke();
        }
    }
}
