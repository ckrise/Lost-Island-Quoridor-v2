using System.Collections;
using System.Collections.Generic;

public class Move
{
    private string Pos1 { set; get; }
    private string Pos2 { set; get; }

    public Move(string start, string end)
    {
        Pos1 = start;
        Pos2 = end;
    }

    public bool Equals(Move move)
    {
        bool result = false;
        if (Pos1 == move.GetPos1() && Pos2 == move.GetPos2())
        {
            result = true;
        }
        else if (Pos2 == move.GetPos1() && Pos1 == move.GetPos2())
        {
            result = true;
        }
        return result;
    }

    public string GetPos1()
    {
        return Pos1;
    }

    public string GetPos2()
    {
        return Pos2;
    }
}
