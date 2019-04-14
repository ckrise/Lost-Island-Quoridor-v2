using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Board;
using Board.Util;

namespace ArtificialInteligence
{
    class WallDiffNode
    {
        private AIBoard Board { get; set; }
        private int Depth { get; set; }

        private string MoveMade { get; set; }

        //Diff is equal to P1SP - P2SP
        private int PreviousDiff { get; set; }
        private int Diff { get; set; }
        
        public WallDiffNode(AIBoard copy)
        {
            Board = new AIBoard(copy);
            Depth = 0;
            MoveMade = "rootnode";
            Diff = BoardAnalysis.FindShortestPath(Board, true) - BoardAnalysis.FindShortestPath(Board, false);
        }
        
        //Used for creating children
        public WallDiffNode(WallDiffNode n, string move)
        {
            MoveMade = move;
            Board = new AIBoard(n.Board);
            Board.MakeMove(move);
            Depth = n.Depth + 1;
            PreviousDiff = n.Diff;
        }

        //This returns all walls adjacent to the wall move made in 
        public List<WallDiffNode> GetChildren()
        {
            List<WallDiffNode> children = new List<WallDiffNode>();
            List<string> adjacentWalls = DictionaryLookup.PerformWallsOfInterestLookup(MoveMade);
            foreach (string wall in adjacentWalls) {
                AIBoard tempBoard = new AIBoard(Board);
                tempBoard.MakeMove(wall);
                if (BoardAnalysis.CheckPathExists(tempBoard, true) && BoardAnalysis.CheckPathExists(tempBoard, false)) {
                    children.Add(new WallDiffNode(this, wall));
                }
            }
            return children;
        }

        //If it is positive P2SP's path got that much shorter.
        public int CalcChangeInDiff() {
            Diff = BoardAnalysis.FindShortestPath(Board, true) - BoardAnalysis.FindShortestPath(Board, false);
            return PreviousDiff - Diff;
        }
    }
}
