﻿using System.Collections.Generic;
using Board;
using Board.Util;

namespace ArtificialInteligence
{
    public class TreeNode
    {
        //Will use static weights in real implementation to avoid overhead of copying to all nodes.
        private AIBoard Board { get; set; }
        private string MoveMade { get; set; }
        private bool IsPlayerOne { get; set; }
        private float value;

        //Current weights of the form 
        //0 - P1 shortest path
        //1 = P2 shortest path
        //2 = p1 number of walls
        //3 = p2 number of walls
        //4 = p1 manhattan distance
        //4 = p2 manhattan distance
        private readonly List<float> weights;

        //Used for creating a rootnode.
        //This will determine who the max player is for this node and all future children.
        //Max is equal to whose turn it is at this point.
        public TreeNode(AIBoard copy)
        {
            Board = new AIBoard(copy);
            MoveMade = "rootnode";
            weights = new List<float> { 1f, -1f, 1f, -1f, 0f, 0f };
            IsPlayerOne = copy.GetIsPlayerOneTurn();
        }

        //Used in the creating of children of a treenode.
        public TreeNode(AIBoard copy, string move,  List<float> w, bool isp1)
        {
            Board = new AIBoard(copy);
            MoveMade = move;
            weights = w;
            IsPlayerOne = isp1;
        }

        public TreeNode(AIBoard copy, List<float> w, bool isp1) {
            Board = new AIBoard(copy);
            MoveMade = "rootnode";
            weights = w;
            IsPlayerOne = isp1;
        }

        //Constructs a list of treenodes that result from every move made that is possible.
        //Pruning function (SetNodesOfInterest) will cut of many nodes that are not of interest.
        public List<TreeNode> GetChildren()
        {
            List<TreeNode> children = new List<TreeNode>();
            foreach (string move in Board.GetPawnMoves())
            {
                //This checks to make sure walls are valid
                AIBoard tempBoard = new AIBoard(Board);
                tempBoard.MakeMove(move);
                children.Add(new TreeNode(tempBoard, move, weights, IsPlayerOne));
            }

            //This gets only valid walls but is done here so that it will only check walls of interest.
            //This avoids checking if a ton of walls are valid that we don't care about.
            //This is why we do not just call GetWallMoves()
            HashSet<string> wallMoves = Board.GetAllValidWalls();
            SetNodesOfInterest(ref wallMoves);
            foreach (string wall in wallMoves)
            {
                //This checks to make sure walls are valid
                AIBoard tempBoard = new AIBoard(Board);
                tempBoard.MakeMove(wall);
                if ((Board.GetIsPlayerOneTurn() && Board.GetPlayerOneNumWalls() == 0) ||
                    (!Board.GetIsPlayerOneTurn() && Board.GetPlayerTwoNumWalls() == 0)) { }
                else
                {
                    if (BoardAnalysis.CheckPathExists(tempBoard, true) && BoardAnalysis.CheckPathExists(tempBoard, false))
                    {
                        children.Add(new TreeNode(tempBoard, wall, weights, IsPlayerOne));
                    }
                }
            }
            //end of wall selection

            return children;
        }

        //Pruning function used select only walls adjacent to walls or adjacent to pawns.
        private void SetNodesOfInterest(ref HashSet<string> moves)
        {
            int p1Column = Board.GetPlayerOnePos()[0]; //Ascii column Value of a-i
            int p1Row = Board.GetPlayerOnePos()[1]; //Ascii row Value of 1-9

            int p2Column = Board.GetPlayerTwoPos()[0]; //Ascii column Value of a-i
            int p2Row = Board.GetPlayerOnePos()[1]; //Ascii row Value of 1-9
            List<string> wallsOfInterest = new List<string>();

            List<int> columnsOfInterest = new List<int> { p1Column - 1, p1Column, p1Column + 1, p2Column - 1, p2Column, p2Column + 1 };
            List<int> rowsOfInterest = new List<int> { p1Row - 1, p1Row, p1Row + 1, p2Row - 1, p2Row, p2Row + 1 };
            List<string> toBeRemoved = new List<string>();

            foreach (string wall in Board.GetWallsPlaced())
            {
                wallsOfInterest.AddRange(DictionaryLookup.PerformWallsOfInterestLookup(wall));
            }

            foreach (string move in moves)
            {
                if (!columnsOfInterest.Contains(move[0]) || !rowsOfInterest.Contains(move[1]))
                {
                    if (!wallsOfInterest.Contains(move))
                    {
                        toBeRemoved.Add(move);
                    }
                }
            }

            foreach (string move in toBeRemoved)
            {
                moves.Remove(move);
            }
        }

        //Returns the move made to get this board.
        public string GetMoveMade()
        {
            return MoveMade;
        }

        //Determines if this node is in a end game state.
        public bool IsTerminalNode()
        {
            return Board.IsWinner();
        }

        //Gets the Value of the node, calculates it if it has not been done.
        public float GetValue()
        {
            return value;
        }

        public void SetValue(float val)
        {
            value = val;
        }

        //Gets the Value of the node, calculates it if it has not been done.
        public float CalcValue()
        {
            EvaluateNode();
            return value;
        }

        //Static evaluation function of the gameboard.
        private void EvaluateNode()
        {
            //If a player won, assigns node proper value and then returns.
            if (EvaluateWinPossibility(ref value)) { }
            //Calculates factors of interest and calculates the value.
            else
            {
                int P1SP = BoardAnalysis.EstimateShortestPath(Board, true);
                int P2SP = BoardAnalysis.EstimateShortestPath(Board, false);
                int P1NumWalls = Board.GetPlayerOneNumWalls();
                int P2NumWalls = Board.GetPlayerTwoNumWalls();
                int P1MD = BoardAnalysis.FindDirectDistance(Board.GetPlayerOnePos(), true);
                int P2MD = BoardAnalysis.FindDirectDistance(Board.GetPlayerTwoPos(), false);

                if (!IsPlayerOne)
                {
                    value = weights[0] * P1SP + weights[1] * P2SP
                          + weights[2] * P2NumWalls + weights[3] * P1NumWalls
                          + weights[4] * P1MD + weights[5] * P2MD;
                }
                else {
                    value = -weights[0] * P1SP + -weights[1] * P2SP
                         + -weights[2] * P2NumWalls + -weights[3] * P1NumWalls
                         + -weights[4] * P1MD + -weights[5] * P2MD;
                }
            }
        }

        /**
         * Returns true if val is ready to return.
         * Tests for upcoming win conditions to 
         **/
        private bool EvaluateWinPossibility(ref float val)
        {
            int winner = Board.GetWinner();
            bool result = false;
            if (winner == 1 && !IsPlayerOne)
            {
                val = -10000;
                result = true;
            }
            else if (winner == 2 && !IsPlayerOne)
            {
                val = 10000;
                result = true;
            }
            else if (winner == 1 && IsPlayerOne)
            {
                val = 10000;
                result = true;
            }
            else if (winner == 2 && IsPlayerOne)
            {
                val = -10000;
                result = true;
            }
            return result;
        }


        //Override functions used for storage equality.
        public override bool Equals(object obj)
        {
            var node = obj as TreeNode;
            return node != null &&
                   EqualityComparer<AIBoard>.Default.Equals(Board, node.Board);
        }

        public override int GetHashCode()
        {
            return 1211497443 + EqualityComparer<AIBoard>.Default.GetHashCode(Board);
        }
    }
}
