using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SearchNode
{
    private string space;
    private int depth;
    private List<string> visitedSpaces;

    //Initializes node with depth of zero as start node and no visited spaces.
    public SearchNode(string space)
    {
        this.space = space;
        depth = 0;
        visitedSpaces = new List<string>();
    }

    //Gets visited spaces from previous node and adds the previous node to visited spaces.
    //Adds one to the depth from the previous node.
    //Sets this node to the new space
    public SearchNode(SearchNode sn, string space)
    {
        this.space = space;
        depth = sn.GetDepth() + 1;
        visitedSpaces = sn.GetVisited();
        visitedSpaces.Add(sn.GetSpace());
    }

    public int GetDepth()
    {
        return depth;
    }

    public List<string> GetVisited()
    {
        return visitedSpaces.ToList();
    }

    public string GetSpace()
    {
        return space;
    }
}

