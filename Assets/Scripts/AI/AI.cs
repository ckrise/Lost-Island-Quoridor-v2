using System.Collections;
using System.Collections.Generic;
using System;

public class AI {

    public AIBoard CurrentBoard { get; set; }
    private List<string> PossibleWallMoves { get; set; }
    private List<string> PossiblePlayerMoves { get; set; }

    public AI()
    {
        CurrentBoard = new AIBoard();
        PossibleWallMoves = new List<string>();
        PossiblePlayerMoves = new List<string>();
    }

    //Currently gets a random move of all possible moves.
    //Takes playermove, if first move of the game then pass "gamestart", all lower case.
    public string GetEasyMove(string playerMove) {
        HandlePlayerMove(playerMove);
        TreeNode rootNode = new TreeNode(CurrentBoard);
        AlphaBeta ab = new AlphaBeta();
        
        string moveSelected = "error";

        ab.Iterate(rootNode, 1, -100, 100, true);
        moveSelected = ab.GetMoveSelected();
        CurrentBoard.MakeMove(moveSelected);

        return moveSelected;
    }

    private void HandlePlayerMove(string playerMove)
    {
        if (playerMove == "gamestart")
        {
            CurrentBoard.PlayerTwoGoesFirst();
        }
        else if (playerMove == "")
        {

        }
        else
        {
            CurrentBoard.MakeMove(playerMove);
        }
    }


    //TODO:
    public string GetHardMove(string move) {
        return "hardmove";
    }


}
