using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore4.GameCore;
using GameCore4.HelperClasses;

public class GameController : MonoBehaviour
{
    public static GameController GCInstance;
    private GameBoard Board { get; set; }
    private AIController AIController { get; set; }
    private bool playerTurn {get; set;}
    private bool aiGame {get; set;}
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);  
        GCInstance = this;
        Board = new GameBoard();
        AIController = new AIController();
        playerTurn = GameData.PlayerGoesFirst;
        aiGame = GameData.IsAIGame;

        if(playerTurn)
        {
            GUIController.GUIReference.StartPlayerTurn("", Board.ValidWallMoves, Board.ValidPlayer1Moves);
        }
        else if(aiGame)
        {
            string aiMove = AIController.GetMove("gamestart");
            Board.MakeMove(aiMove, playerTurn);
            playerTurn = true;
            GUIController.GUIReference.StartPlayerTurn(aiMove, Board.ValidWallMoves, Board.ValidPlayer1Moves);
        }
        
    }
    //called by the GUIController when the player has finished making a move
    public void RecieveMoveFromPlayer(string move)
    {
        Board.MakeMove(move, playerTurn);
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
                string aiMove = AIController.GetMove(move);
                Board.MakeMove(aiMove, playerTurn);
                if(Board.IsWinner())
                {
                    GUIController.GUIReference.GameOver(playerTurn, aiMove);
                    
                }
                else
                {
                    playerTurn = true;
                    GUIController.GUIReference.StartPlayerTurn(aiMove, Board.ValidWallMoves, Board.ValidPlayer1Moves);
                }
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
        Board.MakeMove(move, playerTurn);
        if(Board.IsWinner())
        {
            GameData.NetworkController.gameOver();
            GUIController.GUIReference.GameOver(false, move);
        }
        else
        {
            playerTurn = true;
            GUIController.GUIReference.StartPlayerTurn(move, Board.ValidWallMoves, Board.ValidPlayer1Moves);
        }
    }
   
 


   

   
}
