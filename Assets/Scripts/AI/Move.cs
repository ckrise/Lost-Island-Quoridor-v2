using System.Collections.Generic;

public class Move
{
    private string Pos1 { set; get; }
    private string Pos2 { set; get; }


    public Move(string space1, string space2)
    {
        Pos1 = space1;
        Pos2 = space2;
    }

    //If pos1/pos2 are in the move passed it is equal.
    public bool Equals(Move move)
    {
        bool result = false;
        if ((Pos1 == move.Pos1 && Pos2 == move.Pos2) || (Pos2 == move.Pos1 && Pos1 == move.Pos2))
        {
            result = true;
        }
        return result;
    }

    public override int GetHashCode()
    {
        var hashCode = 1867150729;
        if (Pos1.CompareTo(Pos2) > 0)
        {
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pos1);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pos2);
        }
        else {
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pos2);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pos1);
           
        }
        return hashCode;
    }
}
