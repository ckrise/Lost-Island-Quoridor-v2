using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore4.GameCore
{
    public class Wall
    {
        public List<GameBoard.Coords> Coords { get; set; }
        public Wall()
        {

        }
        public Wall(GameBoard.Coords coord1, GameBoard.Coords coord2, GameBoard.Coords coord3)
        {
            Coords = new List<GameBoard.Coords>()
            {
                coord1,
                coord2,
                coord3
            };
        }
    }
}
