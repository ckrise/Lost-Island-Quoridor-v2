using System.Collections;
using System.Collections.Generic;

public class AIBoard
{
    private string PlayerOneLocation { get; set; }
    private int PlayerOneNumWalls { get; set; }
    private string PlayerTwoLocation { get; set; }
    private int PlayerTwoNumWalls { get; set; }
    private bool IsPlayerOneTurn { get; set; }

    //Wall as a string : true if wall can be placed false otherwise
    private Dictionary<string, bool> ValidWallPlacements { get; set; }
    private List<Move> InvalidPawnMoves { get; set; }

    //Defaults to player one making the first move.
    public AIBoard()
    {
        InvalidPawnMoves = new List<Move>();
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
        InvalidPawnMoves = new List<Move>();
        foreach (Move move in copy.InvalidPawnMoves)
        {
            InvalidPawnMoves.Add(move);
        }
        PlayerOneLocation = copy.PlayerOneLocation;
        PlayerTwoLocation = copy.PlayerTwoLocation;
        PlayerOneNumWalls = copy.PlayerOneNumWalls;
        PlayerTwoNumWalls = copy.PlayerTwoNumWalls; ;
        IsPlayerOneTurn = copy.IsPlayerOneTurn;
        ValidWallPlacements = new Dictionary<string, bool>();
        foreach (KeyValuePair<string, bool> wall in copy.ValidWallPlacements)
        {
            ValidWallPlacements.Add(wall.Key, wall.Value);
        }
    }

    public string GetPlayerOnePos()
    {
        return PlayerOneLocation;
    }

    public string GetPlayerTwoPos()
    {
        return PlayerTwoLocation;
    }

    public int GetPlayerOneNumWalls()
    {
        return PlayerOneNumWalls;
    }

    public int GetPlayerTwoNumWalls()
    {
        return PlayerTwoNumWalls;
    }

    public bool GetIsPlayerOneTurn() {
        return IsPlayerOneTurn;
    }

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
            ValidWallPlacements[blockedWall] = false;
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
        List<string> possibleMoves = new List<string>();
        if (IsPlayerOneTurn)
        {
            foreach (string move in DictionaryLookup.PerformAdjacentSpaceLookup(PlayerOneLocation))
            {
                bool moveGood = true;
                foreach (Move invalidMove in InvalidPawnMoves)
                {
                    if (invalidMove.Equals(new Move(PlayerOneLocation, move)))
                    {
                        moveGood = false;
                    }
                }
                if (moveGood)
                {
                    //Possible jump handling done here.
                    if (PlayerTwoLocation == move)
                    {
                        string jumpDirection = GetMoveDirection(PlayerOneLocation, PlayerTwoLocation);
                        List<string> possibleJumps = GetAdjacentMoves(PlayerTwoLocation);
                        List<string> perpendicularJumps = new List<string>();
                        string straightJump = "none";
                        possibleJumps.Remove(PlayerOneLocation);
                        foreach (string jump in possibleJumps) {
                            string possibleDirection = GetMoveDirection(PlayerTwoLocation, jump);
                            if (possibleDirection == jumpDirection)
                            {
                                straightJump = jump;
                            }
                            else {
                                perpendicularJumps.Add(jump);
                            }
                        }
                        if (straightJump != "none")
                        {
                            possibleMoves.Add(straightJump);
                        }
                        else {
                            possibleMoves.AddRange(perpendicularJumps);
                        }
                    }
                    else
                    {
                        possibleMoves.Add(move);
                    }
                }
            }
        }
        else
        {
            foreach (string move in DictionaryLookup.PerformAdjacentSpaceLookup(PlayerTwoLocation))
            {
                bool moveGood = true;
                foreach (Move invalidMove in InvalidPawnMoves)
                {
                    if (invalidMove.Equals(new Move(PlayerTwoLocation, move)))
                    {
                        moveGood = false;
                    }
                }
                if (moveGood)
                {
                    //Possible jump handling done here.
                    if (PlayerOneLocation == move)
                    {
                        string jumpDirection = GetMoveDirection(PlayerTwoLocation, PlayerOneLocation);
                        List<string> possibleJumps = GetAdjacentMoves(PlayerOneLocation);
                        List<string> perpendicularJumps = new List<string>();
                        string straightJump = "none";
                        possibleJumps.Remove(PlayerTwoLocation);
                        foreach (string jump in possibleJumps)
                        {
                            string possibleDirection = GetMoveDirection(PlayerOneLocation, jump);
                            if (possibleDirection == jumpDirection)
                            {
                                straightJump = jump;
                            }
                            else
                            {
                                perpendicularJumps.Add(jump);
                            }
                        }
                        if (straightJump != "none")
                        {
                            possibleMoves.Add(straightJump);
                        }
                        else
                        {
                            possibleMoves.AddRange(perpendicularJumps);
                        }
                    }
                    else
                    {
                        possibleMoves.Add(move);
                    }
                }
            }
        }
        return possibleMoves;
    }

    public List<string> GetWallMoves()
    {
        List<string> possibleMoves = new List<string>();
        foreach (KeyValuePair<string, bool> move in ValidWallPlacements)
        {
            if (IsPlayerOneTurn && PlayerOneNumWalls == 0)
            {
                break;
            }
            else if (!IsPlayerOneTurn && PlayerTwoNumWalls == 0)
            {
                break;
            }
            if (move.Value)
            {
                //This checks to make sure walls are valid
                AIBoard tempBoard = new AIBoard(this);
                tempBoard.MakeMove(move.Key);

                if (tempBoard.CheckPathExists(true) && tempBoard.CheckPathExists(false))
                {
                    possibleMoves.Add(move.Key);
                }

            }
        }
        return possibleMoves;
    }

    //Pawn Jumping Mechanic Functions
    //Returns string of right, left, up, or down.
    //Throws exception if passed a move in the wrong format.
    private string GetMoveDirection(string startSpace, string endSpace) {
        string result;
        int startRow = startSpace[0];
        int startNumber = startSpace[1];
        int endRow = endSpace[0];
        int endNumber = endSpace[1];

        if (startRow > endRow)
        {
            result = "left";
        }
        else if (endRow > startRow)
        {
            result = "right";
        }
        else if (startNumber > endNumber)
        {
            result = "down";
        }
        else if (endNumber > startNumber)
        {
            result = "up";
        }
        else
        {
            result = "error";
        }

        return result;
    }


    //Shortest Path functions start here.
    //If result returns as -1: no path exists.
    //Otherwise it will return the number of moves it would take to reach the end goal
    //for each respective player.
    //TODO: optimize by checking each square found for the direct path(would save tons of time early game).
    public int EstimateShortestPath(bool isPlayerOne)
    {
        Queue<SearchNode> spaces = new Queue<SearchNode>();
        int result = -1;
        if (isPlayerOne)
        {
            spaces.Enqueue(new SearchNode(PlayerOneLocation));
        }
        else
        {
            spaces.Enqueue(new SearchNode(PlayerTwoLocation));
        }

        //Will exit after all paths return without finding end or 
        //break out of loop when first path to finish is found.
        while (spaces.Count != 0 && result == -1)
        {
            SearchNode currentNode = spaces.Dequeue();

            //Checks for easy direct path
            if (IsDirectPath(isPlayerOne)) {
                result = FindDirectDistance(currentNode.GetSpace(), isPlayerOne);
                break;
            }

            //If it reaches distance of 8 estimate remaining distance with direct distance.
            if (currentNode.GetDepth() == 6) {
                return 6 + FindDirectDistance(currentNode.GetSpace(), isPlayerOne);
            }

            //Check the different end conditions for respective players.
            //If they succeed set the result to the node's depth and break the loop.
            if (isPlayerOne)
            {
                if (currentNode.GetSpace().EndsWith("9"))
                {
                    result = currentNode.GetDepth();
                }
            }
            else
            {
                if (currentNode.GetSpace().EndsWith("1"))
                {
                    result = currentNode.GetDepth();
                }
            }

            //Get a list of moves from the current node location.
            //If the node has not already been visited by this branch it is added to the queue.
            List<string> movesFromSpace = GetAdjacentMoves(currentNode.GetSpace());
            foreach (string move in movesFromSpace)
            {
                if (!currentNode.GetVisited().Contains(move))
                {
                    spaces.Enqueue(new SearchNode(currentNode, move));
                }
            }

        }
        return result;
    }

    public bool IsDirectPath(bool isPlayerOne) {
        bool pathExists = false;
        Queue<string> nextMove = new Queue<string>();
        string currentPoint;

        if (isPlayerOne)
        {
            nextMove.Enqueue(GetPlayerOnePos());
            while (nextMove.Count != 0) {
                currentPoint = nextMove.Dequeue();
                if (currentPoint.EndsWith("9")) {
                    pathExists = true;
                    break;
                }
                List<string> movesFromCurrent = GetAdjacentMoves(currentPoint);
                foreach (string move in movesFromCurrent) {
                    if ((int)move[1] == (int)currentPoint[1] + 1) {
                        nextMove.Enqueue(move);
                        break;
                    }
                }
            }
        }
        else {
            nextMove.Enqueue(GetPlayerTwoPos());
            while (nextMove.Count != 0)
            {
                currentPoint = nextMove.Dequeue();
                if (currentPoint.EndsWith("1"))
                {
                    pathExists = true;
                    break;
                }
                List<string> movesFromCurrent = GetAdjacentMoves(currentPoint);
                foreach (string move in movesFromCurrent)
                {
                    if ((int)move[1] == (int)currentPoint[1] - 1)
                    {
                        nextMove.Enqueue(move);
                        break;
                    }
                }
            }
        }
        return pathExists;
    }

    public int FindDirectDistance(string space, bool isPlayerOne) {
        int result = 0;
        if (isPlayerOne)
        {
            result = 9 - (space[1] - 48);
        }
        else {
            result = (space[1] - 48) - 1;
        }
        return result;
    }

    //Uses a depth first search to find any path that reaches the end goal
    private bool CheckPathExists(bool isPlayerOne)
    {
        //Moves to be visited is used to prevent the revisiting of nodes by another branch.
        List<string> movesToBeVisited = new List<string>();
        Stack<SearchNode> spaces = new Stack<SearchNode>();
        bool result = false;

        //Adds the appropriate starting node depending on specified player.
        if (isPlayerOne)
        {
            spaces.Push(new SearchNode(PlayerOneLocation));
        }
        else
        {
            spaces.Push(new SearchNode(PlayerTwoLocation));
        }


        while (spaces.Count != 0 && !result)
        {
            SearchNode currentNode = spaces.Pop();

            //Check the win conditions of the appropriate player.
            if (isPlayerOne)
            {
                if (currentNode.GetSpace().EndsWith("9"))
                {
                    result = true;
                }
            }
            else
            {
                if (currentNode.GetSpace().EndsWith("1"))
                {
                    result = true;
                }
            }

            //Get the possible moves from the space of the current node.
            List<string> movesFromSpace = GetAdjacentMoves(currentNode.GetSpace());
            //Adds the appropriate nodes to the stack to be searched.
            foreach (string move in movesFromSpace)
            {
                SearchNode sc = new SearchNode(currentNode, move);
                if (!currentNode.GetVisited().Contains(move) && !movesToBeVisited.Contains(move))
                {
                    spaces.Push(sc);
                    movesToBeVisited.Add(move);
                }
            }

        }
        return result;
    }

    //Returns a list of moves from the space specified.
    //This is used as a helper function in traversal functions.
    private List<string> GetAdjacentMoves(string space)
    {
        List<string> possibleMoves = new List<string>();
        foreach (string move in DictionaryLookup.PerformAdjacentSpaceLookup(space))
        {
            bool moveGood = true;
            foreach (Move invalidMove in InvalidPawnMoves)
            {
                if (invalidMove.Equals(new Move(space, move)))
                {
                    moveGood = false;
                    break;
                }
            }
            if (moveGood)
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
        ValidWallPlacements = new Dictionary<string, bool>();
        for (int i = 1; i <= 8; ++i)
        {
            ValidWallPlacements.Add("a" + i.ToString() + "v", true);
            ValidWallPlacements.Add("a" + i.ToString() + "h", true);
            ValidWallPlacements.Add("b" + i.ToString() + "v", true);
            ValidWallPlacements.Add("b" + i.ToString() + "h", true);
            ValidWallPlacements.Add("c" + i.ToString() + "v", true);
            ValidWallPlacements.Add("c" + i.ToString() + "h", true);
            ValidWallPlacements.Add("d" + i.ToString() + "v", true);
            ValidWallPlacements.Add("d" + i.ToString() + "h", true);
            ValidWallPlacements.Add("e" + i.ToString() + "v", true);
            ValidWallPlacements.Add("e" + i.ToString() + "h", true);
            ValidWallPlacements.Add("f" + i.ToString() + "v", true);
            ValidWallPlacements.Add("f" + i.ToString() + "h", true);
            ValidWallPlacements.Add("g" + i.ToString() + "v", true);
            ValidWallPlacements.Add("g" + i.ToString() + "h", true);
            ValidWallPlacements.Add("h" + i.ToString() + "v", true);
            ValidWallPlacements.Add("h" + i.ToString() + "h", true);
        }
    }

    //This is called to make the first move passed to the board effect Player2.
    public void PlayerTwoGoesFirst()
    {
        IsPlayerOneTurn = false;
    }

    //Returns true if there is a winner.
    //Easily modifiable to return the string of which player won.
    public bool IsWinner()
    {
        bool result = false;
        if (PlayerOneLocation.EndsWith("9"))
        {
            result = true;
        }
        else if (PlayerTwoLocation.EndsWith("1"))
        {
            result = true;
        }
        return result;
    }

}
