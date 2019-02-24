using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class BoardAnalysis
{
    public static int EstimateShortestPath(AIBoard board, bool isPlayerOne, int maxDepth)
    {
        Queue<SearchNode> spaces = new Queue<SearchNode>();
        List<string> movesToBeVisited = new List<string>();
        int result = -1;
        if (isPlayerOne)
        {
            spaces.Enqueue(new SearchNode(board.GetPlayerOnePos()));
        }
        else
        {
            spaces.Enqueue(new SearchNode(board.GetPlayerTwoPos()));
        }
        movesToBeVisited.Add(spaces.Peek().GetSpace());

        //Will exit after all paths return without finding end or 
        //break out of loop when first path to finish is found.
        while (spaces.Count != 0 && result == -1)
        {
            SearchNode currentNode = spaces.Dequeue();


            if (HasDirectPath(board, isPlayerOne))
            {
                result = FindDirectDistance(currentNode.GetSpace(), isPlayerOne);
                break;
            }

            //If it reaches distance of 8 estimate remaining distance with direct distance.
            if (currentNode.GetDepth() == maxDepth)
            {
                return maxDepth + FindDirectDistance(currentNode.GetSpace(), isPlayerOne);
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
            List<string> movesFromSpace = board.GetAdjacentMoves(currentNode.GetSpace());
            foreach (string move in movesFromSpace)
            {
                if (!movesToBeVisited.Contains(move))
                {
                    spaces.Enqueue(new SearchNode(currentNode, move));
                    movesToBeVisited.Add(move);
                }
            }

        }
        return result;
    }

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
                List<string> movesFromCurrent = board.GetAdjacentMoves(currentPoint);
                foreach (string move in movesFromCurrent)
                {
                    if ((int)move[1] == (int)currentPoint[1] + 1)
                    {
                        nextMove.Enqueue(move);
                        break;
                    }
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
                List<string> movesFromCurrent = board.GetAdjacentMoves(currentPoint);
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
}
