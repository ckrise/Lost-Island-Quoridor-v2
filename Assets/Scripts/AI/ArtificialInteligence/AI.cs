using System.Collections.Generic;
using Board;
using Board.Util;
using System;

namespace ArtificialInteligence
{
    public class AI
    {
        private AIBoard CurrentBoard { get; set; }
        private int MoveNum { get; set; }

        public AI()
        {
            CurrentBoard = new AIBoard();
        }

        //Does a game tree search 1 layer deep.
        public string GetEasyMove(string playerMove)
        {
            float weight1;
            if (MoveNum < 4)
            {
                weight1 = 0;
            }
            else if (MoveNum > 3 && MoveNum < 8)
            {
                weight1 = 2;
            }
            else
            {
                weight1 = new Random().Next(0, 1);
            }
            TreeNode.weights = new List<float> { weight1, 1f };
            TreeNode.wallValues = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f }; 

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

            MoveNum++;
            return moveSelected;
        }

        public string GetIntermediateMove(string playerMove)
        {
            TreeNode.weights = new List<float> { new Random().Next(0, 3), 1f };
            TreeNode.wallValues = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
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
            TreeNode.weights = new List<float> { 1f, 1f };
            TreeNode.wallValues = new List<float> { 5f, 3f, 3f, 2f, 2f, 0f, 0f, 0f, 0f, 0f };

            //AI starts on e9 and makes the first move of the game.
            if (playerMove == "gamestart") { CurrentBoard.PlayerTwoGoesFirst(); }
            //AI starts on e1 and makes the first move of the game.
            else if (playerMove == "") { }
            //Updates the board with the player's move.
            else
            {
                CurrentBoard.MakeMove(playerMove);
            }

            TreeNode rootNode = new TreeNode(CurrentBoard);
            float rootNodeValue = rootNode.CalcValue();

            List<TreeNode> possibleMoves = rootNode.GetChildren();

            //Create dictionary of all useful values of first two levels of analysis.
            Dictionary<string, MoveInfo> moveValues = new Dictionary<string, MoveInfo>();
            foreach (TreeNode moveNode in possibleMoves)
            {
                MoveInfo thisMovesInfo = new MoveInfo();
                foreach (TreeNode childNode in moveNode.GetChildren())
                {
                    thisMovesInfo.UpdateValues(childNode.CalcValue(), moveNode.CalcValue());
                }
                moveValues.Add(moveNode.GetMoveMade(), thisMovesInfo);
            }

            string selectedMove = "none1";

            //Minimax of the first two levels.
            List<string> movesSelected = new List<string>();
            float value = float.NegativeInfinity;
            foreach (KeyValuePair<string, MoveInfo> pair in moveValues) {
                if (pair.Value.min > value)
                {
                    value = pair.Value.min;
                    movesSelected.Clear();
                    movesSelected.Add(pair.Key);
                }
                else if (pair.Value.min == value)
                {
                    movesSelected.Add(pair.Key);
                }
            }

            selectedMove = movesSelected[new Random().Next(0, movesSelected.Count)];

            //If selected move isn't a wall then use the pawn moves single level evaluation.
            //And if jump next move isn't a possibility.
            bool isPawnMove = !(selectedMove.EndsWith("h") || selectedMove.EndsWith("v"));
            bool jumpPossible = BoardAnalysis.FindDistanceBetween(CurrentBoard, CurrentBoard.GetPlayerOnePos(), CurrentBoard.GetPlayerTwoPos()) == 2;
            if (isPawnMove && !jumpPossible) {
                List<string> pawnMoves = CurrentBoard.GetPawnMoves();
                value = float.NegativeInfinity;
                selectedMove = "none2";
                foreach (string pm in pawnMoves) {
                    float tempVal = moveValues[pm].singleLevelValue;
                    if (tempVal > value) {
                        selectedMove = pm;
                        value = tempVal;
                    }
                }
            }

            CurrentBoard.MakeMove(selectedMove);
            return selectedMove;
        }

        class MoveInfo
        {
            public float min, max, singleLevelValue;
            List<float> allChildValues;

            public MoveInfo()
            {
                min = float.PositiveInfinity;
                max = float.NegativeInfinity;
                allChildValues = new List<float>();
            }

            public void UpdateValues(float m, float slv)
            {
                singleLevelValue = slv;
                allChildValues.Add(m);
                if (m < min)
                {
                    min = m;
                }
                if (m > max)
                {
                    max = m;
                }
            }
        }

        private List<float> GetHardWeights(AIBoard board, int turnNum)
        {
            float p1spWeight = 1f;
            float p2spWeight = 1f;
            float p1nwWeight = 0f;
            float p2nwWeight = 0f;

            //Variables used in weight calculations.
            int playerOneSP = BoardAnalysis.FindShortestPath(board, true);
            int playerTwoSP = BoardAnalysis.FindShortestPath(board, true);
            int playerTwoNumWalls = CurrentBoard.GetPlayerTwoNumWalls();
            int distanceBetweenPlayers = BoardAnalysis.FindDistanceBetween(CurrentBoard, CurrentBoard.GetPlayerOnePos(), CurrentBoard.GetPlayerTwoPos());

            if (playerOneSP < 4) {
                p1spWeight = 4 - playerOneSP;
            }
            if (playerTwoSP < 3) {
                p2spWeight = 3 - playerTwoSP;
            }
            if (playerTwoNumWalls < 4) {
                p2nwWeight = 6 - playerTwoNumWalls;
            }


            List<float> w = new List<float> { p1spWeight, p2spWeight, p2nwWeight, p1nwWeight };
            return w;
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
        private int GetMoveRisk(string move)
        {
            int riskValue = 100;
            AIBoard tempBoard = new AIBoard(CurrentBoard);
            tempBoard.MakeMove(move);
            WallDiffNode rootNode = new WallDiffNode(tempBoard);

            List<string> wallPlacements = tempBoard.GetWallMoves();
            foreach (string wall in wallPlacements)
            {
                riskValue = Math.Min(riskValue, RiskIterate(new WallDiffNode(rootNode, wall), 0));
            }

            return riskValue;
        }

        private static int changeThreshhold = 3;
        private int RiskIterate(WallDiffNode node, int depth, int maxDepth = 4)
        {
            //NumMoves is the number of moves it takes before a significant change in shortest path is found.
            int changeInDiff = node.CalcChangeInDiff();
            int numMoves = 1000;
            if (changeInDiff > changeThreshhold)
            {
                numMoves = depth;
            }
            else if (depth > maxDepth)
            {
                numMoves = depth;
            }
            else
            {
                List<WallDiffNode> children = node.GetChildren();
                foreach (WallDiffNode child in children)
                {
                    numMoves = Math.Min(numMoves, RiskIterate(child, depth + 1));
                }
            }
            return numMoves;
        }

    }
}