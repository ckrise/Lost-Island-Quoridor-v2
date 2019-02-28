using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArtificialInteligence;
using Board;
using GameCore4.GameCore;
using GameCore4.HelperClasses;

public class GameController : MonoBehaviour
{
    public static GameController GCInstance;
    private AIBoard Board { get; set; }
    private AIController AIController { get; set; }
    private bool playerTurn {get; set;}
    private bool aiGame {get; set;}
    
    // Start is called before the first frame update
    void Start()
    {
        
        DontDestroyOnLoad(gameObject);  
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
            //set AI Difficulty here
            string aiMove = AIController.GetMove("gamestart");
            Board.MakeMove(aiMove);
            playerTurn = true;
            GUIController.GUIReference.StartPlayerTurn(aiMove, Board.GetWallMoves(), Board.GetPawnMoves());
        }
        else
        {
            Board.PlayerTwoGoesFirst();
        }
        
    }
    //called by the GUIController when the player has finished making a move
    public void RecieveMoveFromPlayer(string move)
    {
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
                string aiMove = AIController.GetMove(move);
                Board.MakeMove(aiMove);
                if(Board.IsWinner())
                {
                    GUIController.GUIReference.GameOver(playerTurn, aiMove);
                    
                }
                else
                {
                    playerTurn = true;
                    GUIController.GUIReference.StartPlayerTurn(aiMove, Board.GetWallMoves(), Board.GetPawnMoves());
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
   
 


   

   
}
