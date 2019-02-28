using System.Collections.Generic;
using Board;

namespace ArtificialInteligence.GeneticLearning
{
    class AIForTraining
    {
        private AIBoard CurrentBoard { get; set; }
        private List<HashSet<TreeNode>> NodeEvalHistory { get; set; }
        private int CurrentHistoryList { get; set; }
        public List<float> EvalWeights { get; set; }

        public AIForTraining()
        {
            CurrentBoard = new AIBoard();
            NodeEvalHistory = new List<HashSet<TreeNode>>();
            CurrentHistoryList = -1;
            EvalWeights = new List<float> { 1, -1, 1, -1, 1, -1 };
        }

        public AIForTraining(AIForTraining ai)
        {
            CurrentBoard = new AIBoard();
            NodeEvalHistory = new List<HashSet<TreeNode>>();
            CurrentHistoryList = -1;
            EvalWeights = new List<float>(ai.EvalWeights);
        }

        public AIForTraining(List<float> weights)
        {
            CurrentBoard = new AIBoard();
            NodeEvalHistory = new List<HashSet<TreeNode>>();
            CurrentHistoryList = -1;
            EvalWeights = new List<float>(weights);
        }

        public void ResetBoard() {
            CurrentBoard = new AIBoard();
            NodeEvalHistory = new List<HashSet<TreeNode>>();
            CurrentHistoryList = -1;
        }

        //Does a game tree search 1 layer deep.
        public string GetEasyMove(string playerMove)
        {
            HandlePlayerMove(playerMove);

            TreeNode rootNode = new TreeNode(CurrentBoard);

            string moveSelected = IterateStart(rootNode, 1);

            CurrentBoard.MakeMove(moveSelected);

            return moveSelected;
        }

        //Initiates a minimax search 2 layers deep.
        public string GetHardMove(string playerMove)
        {
            CurrentHistoryList++;
            NodeEvalHistory.Add(new HashSet<TreeNode>(new TreeNodeEqualityComparer()));

            HandlePlayerMove(playerMove);

            TreeNode rootNode = new TreeNode(CurrentBoard, EvalWeights, CurrentBoard.GetIsPlayerOneTurn());

            string moveSelected = IterateStart(rootNode, 2);

            CurrentBoard.MakeMove(moveSelected);

            return moveSelected;
        }

        //This handles game start logic and updating the board during the game.
        private void HandlePlayerMove(string playerMove)
        {
            //AI starts on e9 and makes the first move of the game.
            if (playerMove == "gamestart") { CurrentBoard.PlayerTwoGoesFirst(); }
            //AI starts on e1 and makes the first move of the game.
            else if (playerMove == "") { }
            //Updates the board with the player's move.
            else
            {
                CurrentBoard.MakeMove(playerMove);
            }
        }

        //Function able to return the actual move from the first level.
        //This function is what initiates the alpha beta minimax search.
        //Currently selects a random move from a list of moves evaluated to be equal to one another.
        private string IterateStart(TreeNode node, int depth)
        {
            List<TreeNode> rootChildren = node.GetChildren();
            float alpha = float.NegativeInfinity;
            float beta = float.PositiveInfinity;

            List<string> movesSelected = new List<string>();
            foreach (TreeNode child in rootChildren)
            {
                float result = Iterate(child, depth - 1, alpha, beta, false);
                if (result > alpha)
                {
                    alpha = result;
                    movesSelected.Clear();
                    movesSelected.Add(child.GetMoveMade());
                }
                else if (result == alpha)
                {
                    movesSelected.Add(child.GetMoveMade());
                }
            }
            return movesSelected[new System.Random().Next(movesSelected.Count)];
        }


        //Function that performs alpha beta pruning in a minimax tree search.
        private float Iterate(TreeNode node, int depth, float alpha, float beta, bool isMaxPlayer)
        {
            if (depth == 0 || node.IsTerminalNode())
            {
                float result;
                if (TryGetNodeValue(ref node))
                {
                    result = node.GetValue();
                }
                else
                {
                    result = node.CalcValue();
                    NodeEvalHistory[CurrentHistoryList].Add(node);
                }
                return result;
            }
            if (isMaxPlayer)
            {
                foreach (TreeNode child in node.GetChildren())
                {
                    alpha = System.Math.Max(alpha, Iterate(child, depth - 1, alpha, beta, !isMaxPlayer));
                    if (beta > alpha)
                    {
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

        //Function uses history to find the value already calculated
        private bool TryGetNodeValue(ref TreeNode node)
        {
            bool resultFound = false;
            foreach (HashSet<TreeNode> set in NodeEvalHistory)
            {
                if (set.Contains(node))
                {
                    foreach (TreeNode calculatedNode in set)
                    {
                        if (calculatedNode.Equals(node))
                        {
                            node.SetValue(calculatedNode.GetValue());
                            resultFound = true;
                        }
                    }
                }
            }
            return resultFound;
        }

        public AIForTraining GetMutatedAI() {
            System.Random rnd = new System.Random();
            List<float> mutatedWeights = new List<float>(EvalWeights);

            for (int i = 0; i < mutatedWeights.Count; i++) {
                int random = rnd.Next(0, 4);
                if (random == 0)
                { 
                  //flip sign of weight
                    mutatedWeights[i] += 1;
                }
                else if (random == 1)
                { //if 2
                  //pick random weight between -1 and 1
                    mutatedWeights[i] -= 1;
                }
            }

            AIForTraining newAI = new AIForTraining(mutatedWeights);
            return newAI;
        }
    }
}


