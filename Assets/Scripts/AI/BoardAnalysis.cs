using System.Collections.Generic;

static class BoardAnalysis
{
    struct SearchNode {
        public string space;
        public int depth;

        public SearchNode(string s) {
            space = s;
            depth = 0;
        }

        public SearchNode(string s, int d) {
            space = s;
            depth = d + 1;
        }
    }

    //Finds shortest path for the chosen player to the end of the board and returns it.
    public static int GetShortestPath(AIBoard board, bool isPlayerOne)
    {
        Queue<SearchNode> spaces = new Queue<SearchNode>();
        HashSet<string> movesToBeVisited = new HashSet<string>();
        int result = -1;
        if (isPlayerOne)
        {
            spaces.Enqueue(new SearchNode(board.GetPlayerOnePos()));
        }
        else
        {
            spaces.Enqueue(new SearchNode(board.GetPlayerTwoPos()));
        }
        movesToBeVisited.Add(spaces.Peek().space);

        //Will exit after all paths return without finding end or 
        //break out of loop when first path to finish is found.
        while (spaces.Count != 0 && result == -1)
        {
            SearchNode currentNode = spaces.Dequeue();

            //Checks for easy direct path
            if (HasDirectPath(board, isPlayerOne))
            {
                result = FindDirectDistance(currentNode.space, isPlayerOne);
                break;
            }

            //Check the different end conditions for respective players.
            //If they succeed set the result to the node's depth and break the loop.
            if (isPlayerOne)
            {
                if (currentNode.space.EndsWith("9"))
                {
                    result = currentNode.depth;
                }
            }
            else
            {
                if (currentNode.space.EndsWith("1"))
                {
                    result = currentNode.depth;
                }
            }

            //Get a list of moves from the current node location.
            //If the node has not already been visited by this branch it is added to the queue.
            List<string> movesFromSpace = board.GetAdjacentMoves(currentNode.space);
            foreach (string move in movesFromSpace)
            {
                if (!movesToBeVisited.Contains(move))
                {
                    spaces.Enqueue(new SearchNode(move, currentNode.depth + 1));
                    movesToBeVisited.Add(move);
                }
            }

        }
        return result;
    }

    //Uses a depth first search to find any path that reaches the end goal
    public static bool CheckPathExists(AIBoard board, bool isPlayerOne)
    {
        //Moves to be visited is used to prevent the revisiting of nodes by another branch.
        List<string> movesToBeVisited = new List<string>();
        Stack<SearchNode> spaces = new Stack<SearchNode>();
        bool result = false;

        //Adds the appropriate starting node depending on specified player.
        if (isPlayerOne)
        {
            spaces.Push(new SearchNode(board.GetPlayerOnePos()));
        }
        else
        {
            spaces.Push(new SearchNode(board.GetPlayerTwoPos()));
        }


        while (spaces.Count != 0 && !result)
        {
            SearchNode currentNode = spaces.Pop();

            //Check the win conditions of the appropriate player.
            if (isPlayerOne)
            {
                if (currentNode.space.EndsWith("9"))
                {
                    result = true;
                }
            }
            else
            {
                if (currentNode.space.EndsWith("1"))
                {
                    result = true;
                }
            }

            //Get the possible moves from the space of the current node.
            List<string> movesFromSpace = board.GetAdjacentMoves(currentNode.space);
            //Adds the appropriate nodes to the stack to be searched.
            foreach (string move in movesFromSpace)
            {
                if (!movesToBeVisited.Contains(move))
                {
                    spaces.Push(new SearchNode(move, currentNode.depth + 1));
                    movesToBeVisited.Add(move);
                }
            }

        }
        return result;
    }

    //Heuristic utility used to check if a direct path to the end of the board exists.
    public static bool HasDirectPath(AIBoard board, bool isPlayerOne)
    {
        bool pathExists = false;
        Queue<string> nextMove = new Queue<string>();
        string currentPoint;

        if (isPlayerOne)
        {
            nextMove.Enqueue(board.GetPlayerOnePos());
            while (nextMove.Count != 0)
            {
                currentPoint = nextMove.Dequeue();
                if (currentPoint.EndsWith("9"))
                {
                    pathExists = true;
                    break;
                }

                string nextSpace = new string(new char[] {currentPoint[0], (char)(currentPoint[1] + 1) });
                if (!board.GetInvalidPawnMoves().Contains(new Move(currentPoint, nextSpace)))
                {
                    nextMove.Enqueue(nextSpace);
                    break;
                }
            }
        }
        else
        {
            nextMove.Enqueue(board.GetPlayerTwoPos());
            while (nextMove.Count != 0)
            {
                currentPoint = nextMove.Dequeue();
                if (currentPoint.EndsWith("1"))
                {
                    pathExists = true;
                    break;
                }

                string nextSpace = new string(new char[] { currentPoint[0], (char)(currentPoint[1] - 1) });
                if (!board.GetInvalidPawnMoves().Contains(new Move(currentPoint, nextSpace)))
                {
                    nextMove.Enqueue(nextSpace);
                    break;
                }
            }
        }
        return pathExists;
    }

    //Utility that finds manhattan distance to the objective for the respective player.
    public static int FindDirectDistance(string space, bool isPlayerOne)
    {
        int result = 0;
        if (isPlayerOne)
        {
            result = 9 - (space[1] - 48);
        }
        else
        {
            result = (space[1] - 48) - 1;
        }
        return result;
    }

    //Pawn Jumping Mechanic Functions
    //Returns string of right, left, up, or down.
    //Throws exception if passed a move in the wrong format.
    public static string GetMoveDirection(string startSpace, string endSpace)
    {
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
}
