using System.Collections.Generic;
using Board;
using System;

namespace ArtificialInteligence
{
    public class AI
    {
        private AIBoard CurrentBoard { get; set; }

        public AI()
        {
            CurrentBoard = new AIBoard();
        }

        //Does a game tree search 1 layer deep.
        public string GetEasyMove(string playerMove)
        {
            TreeNode.weights = new List<float> { .1f, 1f, .05f, 0f };
            HandlePlayerMove(playerMove);

            TreeNode rootNode = new TreeNode(CurrentBoard);

            Dictionary<string, float> depthOneValues = IterateStart(rootNode, 1);
            Dictionary<string, float> depthTwoValues = IterateStart(rootNode, 2);

            string moveSelected = "error";
            float max = float.NegativeInfinity;
            foreach (KeyValuePair<string, float> move in depthOneValues)
            {
                float moveValue;
                if (depthTwoValues[move.Key] > 1000 || depthTwoValues[move.Key] < -1000) {
                    moveValue = depthTwoValues[move.Key];
                }
                else {
                    moveValue = move.Value;
                }
                
                if (max < moveValue)
                {
                    max = moveValue;
                    moveSelected = move.Key;
                }
            }

            CurrentBoard.MakeMove(moveSelected);

            return moveSelected;
        }
        
        public string GetIntermediateMove(string playerMove) {
            TreeNode.weights = new List<float> { 1f, 1f, .25f, 0f };
            HandlePlayerMove(playerMove);

            TreeNode rootNode = new TreeNode(CurrentBoard);

            Dictionary<string, float> depthOneValues = IterateStart(rootNode, 1);
            Dictionary<string, float> depthTwoValues = IterateStart(rootNode, 2);

            string moveSelected = "error";
            float max = float.NegativeInfinity;
            foreach (KeyValuePair<string, float> move in depthOneValues)
            {
                float moveValue;
                if (depthTwoValues[move.Key] > 1000 || depthTwoValues[move.Key] < -1000)
                {
                    moveValue = depthTwoValues[move.Key];
                }
                else
                {
                    moveValue = move.Value;
                }

                if (max < moveValue)
                {
                    max = moveValue;
                    moveSelected = move.Key;
                }
            }

            CurrentBoard.MakeMove(moveSelected);

            return moveSelected;
        }

        //Initiates a minimax search 2 layers deep.
        public string GetHardMove(string playerMove)
        {
            TreeNode.weights = new List<float> { 1f, 1f, .25f, 0f };
            HandlePlayerMove(playerMove);

            TreeNode rootNode = new TreeNode(CurrentBoard);

            int numLevelsSearched;
            if (CurrentBoard.GetPlayerTwoNumWalls() == 0)
            {
                numLevelsSearched = 1;
            }
            else
            {
                numLevelsSearched = 2;
            }

            Dictionary<string, float> moveValues = IterateStart(rootNode, numLevelsSearched);

            string moveSelected = "error";
            float max = float.NegativeInfinity;
            foreach (KeyValuePair<string, float> move in moveValues) {
                if (max < move.Value) {
                    max = move.Value;
                    moveSelected = move.Key;
                }
            }

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
        private Dictionary<string, float> IterateStart(TreeNode node, int depth)
        {
            List<TreeNode> rootChildren = node.GetChildren();
            float alpha = float.NegativeInfinity;
            float beta = float.PositiveInfinity;

            Dictionary<string, float> moveValues = new Dictionary<string, float>();
            foreach (TreeNode child in rootChildren)
            {
                float result = ABIterate(child, depth - 1, alpha, beta, false);
                if (result > alpha)
                {
                    alpha = result;
                }
                moveValues.Add(child.GetMoveMade(), result);
            }

            return moveValues;
        }


        //Function that performs alpha beta pruning in a minimax tree search.
        private float ABIterate(TreeNode node, int depth, float alpha, float beta, bool isMaxPlayer)
        {
            if (depth == 0 || node.IsTerminalNode())
            {
                return node.CalcValue();
            }
            if (isMaxPlayer)
            {
                foreach (TreeNode child in node.GetChildren())
                {
                    alpha = System.Math.Max(alpha, ABIterate(child, depth - 1, alpha, beta, !isMaxPlayer));
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
                    beta = System.Math.Min(beta, ABIterate(child, depth - 1, alpha, beta, !isMaxPlayer));
                    if (beta < alpha)
                    {
                        break;
                    }
                }
                return beta;
            }
        }

        //If risk is very high will return 1, low risk is higher return value.
        private int GetMoveRisk(string move) {
            int riskValue = 100;
            AIBoard tempBoard = new AIBoard(CurrentBoard);
            tempBoard.MakeMove(move);
            WallDiffNode rootNode = new WallDiffNode(tempBoard);

            List<string> wallPlacements = tempBoard.GetWallMoves();
            foreach (string wall in wallPlacements) {
                riskValue = Math.Min(riskValue, RiskIterate(new WallDiffNode(rootNode, wall), 0));
            }

            return riskValue;
        }

        private static int changeThreshhold = 3;
        private int RiskIterate(WallDiffNode node, int depth, int maxDepth = 4) {
            //NumMoves is the number of moves it takes before a significant change in shortest path is found.
            int changeInDiff = node.CalcChangeInDiff();
            int numMoves = 1000;
            if (changeInDiff > changeThreshhold)
            {
                numMoves = depth;
            }
            else if (depth > maxDepth) {
                numMoves = depth;
            }
            else {
                List<WallDiffNode> children = node.GetChildren();
                foreach (WallDiffNode child in children) {
                    numMoves = Math.Min(numMoves, RiskIterate(child, depth + 1));
                }
            }
            return numMoves;   
        }
        
    }
}