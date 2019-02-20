using System;

public class AlphaBeta
{
    private string moveSelected{ get; set; }

    public string GetMoveSelected() {
        return moveSelected;
    }

    public int Iterate(TreeNode node, int depth, int alpha, int beta, bool maxPlayer)
    {
        //If depth is 0 search completed, if EndGameReached() game is over in that state.
        if (depth == 0 || node.IsTerminalNode())
        {
            return node.GetValue();
        }
        //Looks for max value of this node.
        else if (maxPlayer)
        {
            foreach (TreeNode child in node.GetChildren())
            {
                int tempAlpha = alpha;
                alpha = Math.Max(alpha, Iterate(child, depth - 1, alpha, beta, !maxPlayer));
                if (node.GetMoveMade() == "rootnode")
                {
                    if (tempAlpha != alpha) {
                        moveSelected = child.GetMoveMade();
                    }
                }
                if (alpha > beta)
                {
                    break;
                }
            }
            return alpha;
        }
        //Looks for min value of this node.
        else
        {
            foreach (TreeNode child in node.GetChildren())
            {
                beta = Math.Min(beta, Iterate(child, depth - 1, alpha, beta, !maxPlayer));
                if (beta < alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }
}
