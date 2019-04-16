using System.Collections.Generic;
using Board;
using Board.Util;
using System;

namespace ArtificialInteligence
{
    public class HardAI
    {
        private AIBoard CurrentBoard { get; set; }
        private int MoveNum { get; set; }

        public HardAI()
        {
            CurrentBoard = new AIBoard();
        }
        
        //Initiates a minimax search 2 layers deep.
        public string GetHardMove(string playerMove)
        {
            TreeNode.weights = new List<float> { 1f, 1f, 1f, 0f };

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

            //Analyzing the results of tree evaluation starts here.
            bool moveFound = false;
            string moveSelected = "none";
            float moveSelectedValue = float.NegativeInfinity;

            //Check for level 2 single wall placement.
            float requiredChangedInEval = 5;
            foreach (KeyValuePair<string, MoveInfo> move in moveValues) {
                if (move.Value.min > requiredChangedInEval)
                {
                    moveFound = true;
                    if (moveSelectedValue < move.Value.min)
                    {
                        moveSelected = move.Key;
                    }
                }
            }
            //Check for level 2 double wall placement.
            if (!moveFound) {

            }
            //Check for pawn moves.
            if (!moveFound) {
                List<string> possiblePawnMoves = CurrentBoard.GetPawnMoves();
                float currentMax = float.NegativeInfinity;
                foreach (string move in possiblePawnMoves) {
                    TreeNode n = new TreeNode(CurrentBoard, move);
                    MoveInfo mi = moveValues[move];
                    if (!(mi.singleLevelValue - mi.min == 2)) {
                        if (mi.min > currentMax) {
                            currentMax = mi.min;
                            moveSelected = move;
                            moveFound = true;
                        }
                    }
                }
            }

            if (!moveFound) {
                float max = float.NegativeInfinity;
                List<string> maxMoves = new List<string>();
                foreach (KeyValuePair<string, MoveInfo> move in moveValues)
                {
                    if (move.Value.min > max)
                    {
                        max = move.Value.min;
                        maxMoves.Clear();
                        maxMoves.Add(move.Key);

                    }
                    else if (move.Value.min == max)
                    {
                        maxMoves.Add(move.Key);
                    }
                }

                moveSelected = maxMoves[new Random().Next(0, maxMoves.Count)];
            }

            CurrentBoard.MakeMove(moveSelected);
            return moveSelected;
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
    }
}