using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TreeNode
{
    private AIBoard board;
    private int Value { get; set; }
    private bool ValueSet { get; set; }
    private string moveMade { get; set; }

    public TreeNode(AIBoard copy) {
        board = new AIBoard(copy);
        ValueSet = false;
        moveMade = "rootnode";
    }

    public TreeNode(AIBoard copy, string move) {
        board = new AIBoard(copy);
        board.MakeMove(move);
        moveMade = move;
        ValueSet = false;
    }

    //Returns a list of treeNodes that contains of the children of this node.
    public List<TreeNode> GetChildren() {
        List<TreeNode> children = new List<TreeNode>();
        List<string> moves = board.GetWallMoves();
        moves.AddRange(board.GetPawnMoves());
        foreach (string move in moves) {
            children.Add(new TreeNode(new AIBoard(board), move));
        }     
        return children;
    }

    public string GetMoveMade() {
        return moveMade;
    }

    //Determines if this node is in a end game state.
    public bool IsTerminalNode() {
        return board.IsWinner();
    }

    //Gets the value of the node, calculates it if it has not been done.
    public int GetValue() {
        if (!ValueSet) {
            EvaluateNode();
        }
        return Value;
    }

    //Static evaluation function of the gameboard.
    private void EvaluateNode() {
        int playerOneShortestPath;
        int playerTwoShortestPath;
        if (moveMade.EndsWith("h") || moveMade.EndsWith("v"))
        {
            playerOneShortestPath = board.EstimateShortestPath(true, 6);
            playerTwoShortestPath = board.EstimateShortestPath(false, 6);
        }
        else
        {
            playerOneShortestPath = board.EstimateShortestPath(true, 10);
            playerTwoShortestPath = board.EstimateShortestPath(false, 20);
        }
        
        //Difference is the number of moves P2 path is shorter than P1.
        int difference = playerOneShortestPath - playerTwoShortestPath;
        int wallDifference = board.GetPlayerTwoNumWalls() - board.GetPlayerOneNumWalls();
        Value = difference + board.GetPlayerTwoNumWalls() + wallDifference * 2;
    }

    

}
