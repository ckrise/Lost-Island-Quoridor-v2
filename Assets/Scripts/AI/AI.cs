using System.Collections.Generic;

public class AI {
    private AIBoard CurrentBoard { get; set; }
    
    public AI()
    {
        CurrentBoard = new AIBoard();
    }

    //Does a game tree search 1 layer deep.
    public string GetEasyMove(string playerMove) {
        HandlePlayerMove(playerMove);

        TreeNode rootNode = new TreeNode(CurrentBoard);

        string moveSelected = IterateStart(rootNode, 1);

        CurrentBoard.MakeMove(moveSelected);

        return moveSelected;
    }

    //Initiates a minimax search 2 layers deep.
    public string GetHardMove(string playerMove)
    {
        HandlePlayerMove(playerMove);

        TreeNode rootNode = new TreeNode(CurrentBoard);

        string moveSelected = IterateStart(rootNode, 2);

        CurrentBoard.MakeMove(moveSelected);

        return moveSelected;
    }

    //This handles game start logic and updating the board during the game.
    private void HandlePlayerMove(string playerMove)
    {
        //AI starts on e9 and makes the first move of the game.
        if (playerMove == "gamestart")
        {
            CurrentBoard.PlayerTwoGoesFirst();
        }
        //AI starts on e1 and makes the first move of the game.
        else if (playerMove == "")
        {

        }
        //Updates the board with the player's move.
        else
        {
            CurrentBoard.MakeMove(playerMove);
        }
    }

    //Function able to return the actual move from the first level.
    //This function is what initiates the alpha beta minimax search.
    //Currently selects a random move from a list of moves evaluated to be equal to one another.
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


    //Function that performs alpha beta pruning in a minimax tree search.
    private int Iterate(TreeNode node, int depth, int alpha, int beta, bool isMaxPlayer) {
        if (depth == 0) {
            return node.GetValue();
        }
        if (isMaxPlayer)
        {
            foreach (TreeNode child in node.GetChildren()) {
                alpha = System.Math.Max(alpha, Iterate(child, depth - 1, alpha, beta, !isMaxPlayer));
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
                beta = System.Math.Min(beta, Iterate(child, depth - 1, alpha, beta, !isMaxPlayer));
                if (beta < alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }
}
