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
        SetNodesOfInterest(ref moves);
        foreach (string move in moves) {
            children.Add(new TreeNode(new AIBoard(board), move));
        }
        foreach (string move in board.GetPawnMoves()) {
            children.Add(new TreeNode(new AIBoard(board), move));
        }
        return children;
    }

    private void SetNodesOfInterest(ref List<string> moves) {
        int p1Column = board.GetPlayerOnePos()[0]; //Ascii column value of a-i
        int p1Row = board.GetPlayerOnePos()[1]; //Ascii row value of 1-9

        int p2Column = board.GetPlayerTwoPos()[0]; //Ascii column value of a-i
        int p2Row = board.GetPlayerOnePos()[1]; //Ascii row value of 1-9
        HashSet<string> wallsOfInterest = new HashSet<string>();

        List<int> columnsOfInterest = new List<int> { p1Column - 1, p1Column, p1Column + 1, p2Column - 1, p2Column, p2Column + 1 };
        List<int> rowsOfInterest = new List<int> { p1Row - 1, p1Row, p1Row + 1, p2Row - 1, p2Row, p2Row + 1 };
        List<string> toBeRemoved = new List<string>();

        foreach (string wall in board.GetWallsPlaced()) {
            foreach (string importantWall in DictionaryLookup.PerformWallsOfInterestLookup(wall)) {
                wallsOfInterest.Add(importantWall);
            }
        }

        foreach (string move in moves) {
            if (!columnsOfInterest.Contains(move[0]) || !rowsOfInterest.Contains(move[1]))
            {
                if (!wallsOfInterest.Contains(move)) {
                    toBeRemoved.Add(move);
                }
            }
        }

        foreach (string move in toBeRemoved) {
            moves.Remove(move);
        }
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

    public void SetValue(int value) {
        Value = value;
        ValueSet = true;
    }

    //Static evaluation function of the gameboard.
    private void EvaluateNode() {
        int playerOneShortestPath;
        int playerTwoShortestPath;

        string winner = board.GetWinner();
        if (winner == "player1")
        {
            Value = -10000;
            return;
        }
        else if (winner == "player2") {
            Value = 10000;
            return;
        }
        if (moveMade.EndsWith("h") || moveMade.EndsWith("v"))
        {
            playerOneShortestPath = BoardAnalysis.EstimateShortestPath(board, true, 100);
            playerTwoShortestPath = BoardAnalysis.EstimateShortestPath(board, false, 100);
        }
        else
        {
            playerOneShortestPath = BoardAnalysis.EstimateShortestPath(board, true, 100);
            playerTwoShortestPath = BoardAnalysis.EstimateShortestPath(board, false, 100);
        }

        //Difference is the number of moves P2 path is shorter than P1.
        int difference = playerOneShortestPath - playerTwoShortestPath;
        int wallDifference = board.GetPlayerTwoNumWalls() - board.GetPlayerOneNumWalls();
        Value = difference + wallDifference;
    }

    

}
