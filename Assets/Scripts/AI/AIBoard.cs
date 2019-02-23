using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AIBoard
{
    private string PlayerOneLocation { get; set; }
    private int PlayerOneNumWalls { get; set; }
    private string PlayerTwoLocation { get; set; }
    private int PlayerTwoNumWalls { get; set; }
    private bool IsPlayerOneTurn { get; set; }

    //Wall as a string : true if wall can be placed false otherwise
    private bool MapsInitialized { get; set; }
    private Dictionary<string, int> ShortestPathMapP1;
    private Dictionary<string, int> ShortestPathMapP2;
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
        ShortestPathMapP1 = new Dictionary<string, int>();
        ShortestPathMapP2 = new Dictionary<string, int>();
        ResetMaps();
    }

    //Does a deep copy of the board passed to the constructor.
    public AIBoard(AIBoard copy)
    {
        InvalidPawnMoves = new List<Move>(copy.InvalidPawnMoves);
        PlayerOneLocation = copy.PlayerOneLocation;
        PlayerTwoLocation = copy.PlayerTwoLocation;
        PlayerOneNumWalls = copy.PlayerOneNumWalls;
        PlayerTwoNumWalls = copy.PlayerTwoNumWalls; ;
        IsPlayerOneTurn = copy.IsPlayerOneTurn;
        ValidWallPlacements = new Dictionary<string, bool>(copy.ValidWallPlacements);
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
    public int FindShortestPath(bool isPlayerOne)
    {
        int result = -1;
        if (isPlayerOne)
        {
            List<string> endSpaces = new List<string> { "a9", "b9", "c9", "d9", "e9", "f9", "g9", "h9", "i9" };
            foreach (string endSpace in endSpaces)
            {
                List<string> visitedSpaces = new List<string>();
                Queue<string> spacesToVisit = new Queue<string>();
                spacesToVisit.Enqueue(endSpace);
                while (spacesToVisit.Count > 0)
                {
                    string currentSpace = spacesToVisit.Dequeue();

                    List<string> adjacentSpaces = GetAdjacentMoves(currentSpace);

                    int min = ShortestPathMapP1[currentSpace];
                    foreach (string thing in adjacentSpaces)
                    {
                        int thingSP = ShortestPathMapP1[thing];
                        if (thingSP + 1 < min)
                        {
                            ShortestPathMapP1[currentSpace] = thingSP + 1;
                        }
                        if (!visitedSpaces.Contains(thing))
                        {
                            spacesToVisit.Enqueue(thing);
                            visitedSpaces.Add(thing);
                        }
                    }
                }
            }
            result = ShortestPathMapP1[PlayerOneLocation];
        }
        else {
            List<string> endSpaces = new List<string> { "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1", "i1" };
            foreach (string endSpace in endSpaces)
            {
                List<string> visitedSpaces = new List<string>();
                Queue<string> spacesToVisit = new Queue<string>();
                spacesToVisit.Enqueue(endSpace);
                while (spacesToVisit.Count > 0)
                {
                    string currentSpace = spacesToVisit.Dequeue();

                    List<string> adjacentSpaces = GetAdjacentMoves(currentSpace);

                    int min = ShortestPathMapP2[currentSpace];
                    foreach (string thing in adjacentSpaces)
                    {
                        int thingSP = ShortestPathMapP2[thing];
                        if (thingSP + 1 < min)
                        {
                            ShortestPathMapP2[currentSpace] = thingSP + 1;
                        }
                        if (!visitedSpaces.Contains(thing))
                        {
                            spacesToVisit.Enqueue(thing);
                            visitedSpaces.Add(thing);
                        }
                    }
                }
            }
            result = ShortestPathMapP2[PlayerTwoLocation];
        }

        return result;
    }

    public Dictionary<string, int> GetP1SPMap() {
        return ShortestPathMapP1;
    }

    private void ResetMaps() {
        for (int i = 1; i < 10; i++) {
            ShortestPathMapP1.Add("a" + i.ToString(), 100);
            ShortestPathMapP1.Add("b" + i.ToString(), 100);
            ShortestPathMapP1.Add("c" + i.ToString(), 100);
            ShortestPathMapP1.Add("d" + i.ToString(), 100);
            ShortestPathMapP1.Add("e" + i.ToString(), 100);
            ShortestPathMapP1.Add("f" + i.ToString(), 100);
            ShortestPathMapP1.Add("g" + i.ToString(), 100);
            ShortestPathMapP1.Add("h" + i.ToString(), 100);
            ShortestPathMapP1.Add("i" + i.ToString(), 100);

            ShortestPathMapP2.Add("a" + i.ToString(), 100);
            ShortestPathMapP2.Add("b" + i.ToString(), 100);
            ShortestPathMapP2.Add("c" + i.ToString(), 100);
            ShortestPathMapP2.Add("d" + i.ToString(), 100);
            ShortestPathMapP2.Add("e" + i.ToString(), 100);
            ShortestPathMapP2.Add("f" + i.ToString(), 100);
            ShortestPathMapP2.Add("g" + i.ToString(), 100);
            ShortestPathMapP2.Add("h" + i.ToString(), 100);
            ShortestPathMapP2.Add("i" + i.ToString(), 100);
        }
        ShortestPathMapP1["a9"] = 0;
        ShortestPathMapP1["b9"] = 0;
        ShortestPathMapP1["c9"] = 0;
        ShortestPathMapP1["d9"] = 0;
        ShortestPathMapP1["e9"] = 0;
        ShortestPathMapP1["f9"] = 0;
        ShortestPathMapP1["g9"] = 0;
        ShortestPathMapP1["h9"] = 0;
        ShortestPathMapP1["i9"] = 0;

        ShortestPathMapP2["a1"] = 0;
        ShortestPathMapP2["b1"] = 0;
        ShortestPathMapP2["c1"] = 0;
        ShortestPathMapP2["d1"] = 0;
        ShortestPathMapP2["e1"] = 0;
        ShortestPathMapP2["f1"] = 0;
        ShortestPathMapP2["g1"] = 0;
        ShortestPathMapP2["h1"] = 0;
        ShortestPathMapP2["i1"] = 0;
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
    public List<string> GetAdjacentMoves(string space)
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
    public string GetWinner()
    {
        string winner = "none";
        if (PlayerOneLocation.EndsWith("9"))
        {
            winner = "player1";
        }
        else if (PlayerTwoLocation.EndsWith("1"))
        {
            winner = "player2";
        }
        return winner;
    }

}
