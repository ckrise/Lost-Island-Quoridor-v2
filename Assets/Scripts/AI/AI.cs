using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class AI {

    public AIBoard CurrentBoard { get; set; }

    public AI()
    {
        CurrentBoard = new AIBoard();
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

        string moveSelected = "error";
        if (CurrentBoard.GetPlayerOneNumWalls() == 0 || CurrentBoard.GetPlayerTwoNumWalls() == 0)
        {
            moveSelected = IterateStart(rootNode, 3);
        }
        else {
            moveSelected = IterateStart(rootNode, 2);
        }
        CurrentBoard.MakeMove(moveSelected);

        return moveSelected;
    }

    private string IterateStart(TreeNode node, int depth) {
        List<TreeNode> rootChildren = node.GetChildren();
        int alpha = -10000000;
        int beta = 100000000;

        List<string> movesSelected = new List<string>();
        foreach (TreeNode child in rootChildren) {
            int result = Iterate(child, depth - 1, alpha, beta, false);
            if (result > alpha)
            {
                alpha = result;
                movesSelected.Clear();
                movesSelected.Add(child.GetMoveMade());
            }
            else if (result == alpha) {
                movesSelected.Add(child.GetMoveMade());
            }
        }

        return movesSelected[DictionaryLookup.rnd.Next(movesSelected.Count)];
    }

    private int Iterate(TreeNode node, int depth, int alpha, int beta, bool isMaxPlayer) {
        if (depth == 0) {
            return node.GetValue();
        }
        if (isMaxPlayer)
        {
            foreach (TreeNode child in node.GetChildren()) {
                alpha = Math.Max(alpha, Iterate(child, depth - 1, alpha, beta, !isMaxPlayer));
                if (beta > alpha) {
                    break;
                }
            }

            return alpha;
        }
        else
        {
            foreach (TreeNode child in node.GetChildren())
            {
                beta = Math.Min(beta, Iterate(child, depth - 1, alpha, beta, !isMaxPlayer));
                if (beta < alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }
}
