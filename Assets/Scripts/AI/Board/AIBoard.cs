using System.Collections.Generic;
using Board.Util;
namespace Board
{
    public class AIBoard
    {
        private string PlayerOneLocation { get; set; }
        private int PlayerOneNumWalls { get; set; }
        private string PlayerTwoLocation { get; set; }
        private int PlayerTwoNumWalls { get; set; }
        private bool IsPlayerOneTurn { get; set; }

        private List<string> WallsPlaced { get; set; }

        private HashSet<string> ValidWallPlacements { get; set; }
        private HashSet<Move> InvalidPawnMoves { get; set; }

        //Defaults to player one making the first move.
        public AIBoard()
        {
            InvalidPawnMoves = new HashSet<Move>(new MoveEqualityComparer());
            WallsPlaced = new List<string>();
            PlayerOneLocation = "e1";
            PlayerTwoLocation = "e9";
            PlayerOneNumWalls = 10;
            PlayerTwoNumWalls = 10;
            IsPlayerOneTurn = true;
            InitializeValidWallPlacements();
        }

        //Does a deep copy of the board passed to the constructor.
        public AIBoard(AIBoard copy)
        {
            InvalidPawnMoves = new HashSet<Move>(copy.InvalidPawnMoves, new MoveEqualityComparer());
            WallsPlaced = new List<string>(copy.WallsPlaced);
            PlayerOneLocation = copy.PlayerOneLocation;
            PlayerTwoLocation = copy.PlayerTwoLocation;
            PlayerOneNumWalls = copy.PlayerOneNumWalls;
            PlayerTwoNumWalls = copy.PlayerTwoNumWalls; ;
            IsPlayerOneTurn = copy.IsPlayerOneTurn;
            ValidWallPlacements = new HashSet<string>(copy.ValidWallPlacements);
        }

        //Basic get functions.
        public string GetPlayerOnePos() { return PlayerOneLocation; }
        public string GetPlayerTwoPos() { return PlayerTwoLocation; }
        public int GetPlayerOneNumWalls() { return PlayerOneNumWalls; }
        public int GetPlayerTwoNumWalls() { return PlayerTwoNumWalls; }
        public bool GetIsPlayerOneTurn() { return IsPlayerOneTurn; }
        public List<string> GetWallsPlaced() { return WallsPlaced; }
        public HashSet<string> GetAllValidWalls() { return ValidWallPlacements; }
        public HashSet<Move> GetInvalidPawnMoves() { return InvalidPawnMoves; }

        //Accepts any move, updates board state to be as it should after the move is made.
        public void MakeMove(string move)
        {
            if (move.EndsWith("v") || move.EndsWith("h"))
            {
                PlaceWall(move);
            }
            else
            {
                MovePawn(move);
            }
            IsPlayerOneTurn = !IsPlayerOneTurn;
        }

        //Helper function that adds pawn moves blocked and wall placements blocked based on wall given.
        private void PlaceWall(string move)
        {
            WallsPlaced.Add(move);
            if (IsPlayerOneTurn)
            {
                PlayerOneNumWalls--;
            }
            else
            {
                PlayerTwoNumWalls--;
            }

            //When wall is placed need to lookup what pawn moves become invalid.
            List<Move> movesBlocked = DictionaryLookup.PerformPawnBlockLookup(move);
            foreach (Move blockedMove in movesBlocked)
            {
                InvalidPawnMoves.Add(blockedMove);
            }

            List<string> wallsBlocked = DictionaryLookup.PerformWallBlockLookup(move);
            foreach (string blockedWall in wallsBlocked)
            {
                ValidWallPlacements.Remove(blockedWall);
            }
        }

        //Helper function to move pawn location.
        private void MovePawn(string move)
        {
            if (IsPlayerOneTurn)
            {
                PlayerOneLocation = move;
            }
            else
            {
                PlayerTwoLocation = move;
            }
        }

        /**
         * Returns a list of all possible pawn moves for whichever players turn it is.
         **/
        public List<string> GetPawnMoves()
        {
            List<string> possibleMoves;
            if (IsPlayerOneTurn)
            {
                possibleMoves = GetAdjacentMoves(PlayerOneLocation);
                foreach (string move in possibleMoves.ToArray())
                {
                    if (PlayerTwoLocation == move)
                    {
                        possibleMoves.Remove(move);

                        string jumpDirection = BoardAnalysis.GetMoveDirection(PlayerOneLocation, PlayerTwoLocation);
                        List<string> possibleJumps = GetAdjacentMoves(PlayerTwoLocation);
                        List<string> perpendicularJumps = new List<string>();
                        string straightJump = "n";
                        possibleJumps.Remove(PlayerOneLocation);
                        foreach (string jump in possibleJumps)
                        {
                            string possibleDirection = BoardAnalysis.GetMoveDirection(PlayerOneLocation, jump);
                            if (possibleDirection == jumpDirection)
                            {
                                straightJump = jump;
                            }
                            else
                            {
                                perpendicularJumps.Add(jump);
                            }
                        }
                        if (straightJump != "n")
                        {
                            possibleMoves.Add(straightJump);
                        }
                        else
                        {
                            possibleMoves.AddRange(perpendicularJumps);
                        }
                        break;
                    }
                }
            }
            else
            {
                possibleMoves = GetAdjacentMoves(PlayerTwoLocation);
                foreach (string move in possibleMoves.ToArray())
                {
                    if (PlayerOneLocation == move)
                    {
                        possibleMoves.Remove(move);

                        string jumpDirection = BoardAnalysis.GetMoveDirection(PlayerTwoLocation, PlayerOneLocation);
                        List<string> possibleJumps = GetAdjacentMoves(PlayerOneLocation);
                        List<string> perpendicularJumps = new List<string>();
                        string straightJump = "n";
                        possibleJumps.Remove(PlayerTwoLocation);
                        foreach (string jump in possibleJumps)
                        {
                            string possibleDirection = BoardAnalysis.GetMoveDirection(PlayerOneLocation, jump);
                            if (possibleDirection == jumpDirection)
                            {
                                straightJump = jump;
                            }
                            else
                            {
                                perpendicularJumps.Add(jump);
                            }
                        }
                        if (straightJump != "n")
                        {
                            possibleMoves.Add(straightJump);
                        }
                        else
                        {
                            possibleMoves.AddRange(perpendicularJumps);
                        }
                        break;
                    }
                }
            }
            return possibleMoves;
        }

        public List<string> GetWallMoves()
        {
            List<string> possibleMoves = new List<string>();

            if ((IsPlayerOneTurn && PlayerOneNumWalls == 0) || (!IsPlayerOneTurn && PlayerTwoNumWalls == 0)) { }
            else
            {
                foreach (string wall in ValidWallPlacements)
                {
                    //This checks to make sure walls are valid
                    AIBoard tempBoard = new AIBoard(this);
                    tempBoard.MakeMove(wall);

                    if (BoardAnalysis.CheckPathExists(tempBoard, true) && BoardAnalysis.CheckPathExists(tempBoard, false))
                    {
                        possibleMoves.Add(wall);
                    }
                }
            }
            return possibleMoves;
        }

        //Returns a list of moves from the space specified.
        //This is used as a helper function in traversal functions.
        public List<string> GetAdjacentMoves(string space)
        {
            List<string> possibleMoves = new List<string>();
            foreach (string move in DictionaryLookup.PerformAdjacentSpaceLookup(space))
            {
                if (!InvalidPawnMoves.Contains(new Move(space, move)))
                {
                    possibleMoves.Add(move);
                }
            }
            return possibleMoves;
        }

        //Helper function only called when created a board from nothing.
        //Builds the dictionary of walls and sets all to true(placeable).
        private void InitializeValidWallPlacements()
        {
            ValidWallPlacements = new HashSet<string>();
            for (int i = 1; i <= 8; ++i)
            {
                ValidWallPlacements.Add("a" + i.ToString() + "v");
                ValidWallPlacements.Add("a" + i.ToString() + "h");
                ValidWallPlacements.Add("b" + i.ToString() + "v");
                ValidWallPlacements.Add("b" + i.ToString() + "h");
                ValidWallPlacements.Add("c" + i.ToString() + "v");
                ValidWallPlacements.Add("c" + i.ToString() + "h");
                ValidWallPlacements.Add("d" + i.ToString() + "v");
                ValidWallPlacements.Add("d" + i.ToString() + "h");
                ValidWallPlacements.Add("e" + i.ToString() + "v");
                ValidWallPlacements.Add("e" + i.ToString() + "h");
                ValidWallPlacements.Add("f" + i.ToString() + "v");
                ValidWallPlacements.Add("f" + i.ToString() + "h");
                ValidWallPlacements.Add("g" + i.ToString() + "v");
                ValidWallPlacements.Add("g" + i.ToString() + "h");
                ValidWallPlacements.Add("h" + i.ToString() + "v");
                ValidWallPlacements.Add("h" + i.ToString() + "h");
            }
        }

        //This is called to make the first move passed to the board effect Player2.
        public void PlayerTwoGoesFirst()
        {
            IsPlayerOneTurn = false;
        }

        //Returns true if there is a winner.
        //Easily modifiable to return the string of which player won.
        public int GetWinner()
        {
            int winner = 0;
            if (PlayerOneLocation[1] == '9')
            {
                winner = 1;
            }
            else if (PlayerTwoLocation[1] == '1')
            {
                winner = 2;
            }
            return winner;
        }

        public bool IsWinner()
        {
            bool winner = false;
            if (PlayerOneLocation[1] == '9')
            {
                winner = true;
            }
            else if (PlayerTwoLocation[1] == '1')
            {
                winner = true;
            }
            return winner;
        }

        public override bool Equals(object obj)
        {
            var board = obj as AIBoard;
            return board != null &&
                   PlayerOneLocation == board.PlayerOneLocation &&
                   PlayerOneNumWalls == board.PlayerOneNumWalls &&
                   PlayerTwoLocation == board.PlayerTwoLocation &&
                   PlayerTwoNumWalls == board.PlayerTwoNumWalls &&
                   EqualityComparer<List<string>>.Default.Equals(WallsPlaced, board.WallsPlaced);
        }

        public override int GetHashCode()
        {
            var hashCode = 537122267;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerOneLocation);
            hashCode = hashCode * -1521134295 + PlayerOneNumWalls.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerTwoLocation);
            hashCode = hashCode * -1521134295 + PlayerTwoNumWalls.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(WallsPlaced);
            return hashCode;
        }
    }
}