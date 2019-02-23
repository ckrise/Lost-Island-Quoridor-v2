using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TreeNode
{
    private AIBoard board;
    private float Value { get; set; }
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
        //SetNodesOfInterest(ref moves);
        foreach (string move in moves) {
            children.Add(new TreeNode(new AIBoard(board), move));
        }     
        return children;
    }

    private void SetNodesOfInterest(ref List<string> moves) {
        int p1 = board.GetPlayerOnePos()[0];
        int p2 = board.GetPlayerTwoPos()[0];
        List<int> columnsOfInterest = new List<int> { p1 - 1, p1, p1 + 1, p2 - 1, p2, p2 + 1 };
        List<string> toBeRemoved = new List<string>();

        foreach (string move in moves) {
            if (!columnsOfInterest.Contains(move[0])) {
                toBeRemoved.Add(move);
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
        return !(board.GetWinner() == "none");
    }

    //Gets the value of the node, calculates it if it has not been done.
    public float GetValue() {
        if (!ValueSet) {
            EvaluateNode();
        }
        return Value;
    }

    //Static evaluation function of the gameboard.
    private void EvaluateNode() {
        //Currently assumes max player is player 2.
        string winner = board.GetWinner();
        if (winner == "player1")
        {
            Value = -1000;
            return;
        }
        else if(winner == "player2")
        {
            Value = 1000;
            return;
        }

        int PlayerOneDistance = board.FindShortestPath(true);
        int PlayerTwoDistance = board.FindShortestPath(false);

        Value = PlayerOneDistance - PlayerTwoDistance;
    }
}
