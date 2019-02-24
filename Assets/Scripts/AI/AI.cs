using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        List<TreeNode> possibleMoves = rootNode.GetChildren();
        
        string moveSelected = "error";
        float max = -10000000;
        foreach (TreeNode node in possibleMoves) {
            if (node.GetValue() > max) {
                max = node.GetValue();
                moveSelected = node.GetMoveMade();
            }
        }
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
    public string GetHardMove(string playerMove) {
        HandlePlayerMove(playerMove);
        TreeNode rootNode = new TreeNode(CurrentBoard);
        List<TreeNode> possibleMoves = rootNode.GetChildren();

        string moveSelected = "error";
        float max = -10000000;
        foreach (TreeNode node in possibleMoves)
        {
            int min = 10000000;
            List<TreeNode> nextMoves = node.GetChildren();
            foreach (TreeNode nextNode in nextMoves)
            {
                int value = nextNode.GetValue();
                if (value < min)
                {
                    min = value;
                }
            }
            node.SetValue(min);
            if (node.GetValue() > max)
            {
                max = node.GetValue();
                moveSelected = node.GetMoveMade();
            }
        }
        CurrentBoard.MakeMove(moveSelected);

        return moveSelected;
    }


}
