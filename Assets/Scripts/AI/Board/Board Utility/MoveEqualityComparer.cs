﻿using System.Collections.Generic;

namespace Board.Util
{
    public class MoveEqualityComparer : IEqualityComparer<Move>
    {
        public bool Equals(Move x, Move y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Move obj)
        {
            return obj.GetHashCode();
        }
    }
}



