using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore4.GameCore
{
    public class Player
    {
        public int WallsLeft { get; set; }
        public GameBoard.Coords CurrentPosition { get; set; }
        private string PlayerType { get; set; }

        public Player(string player)
        {
            PlayerType = player;
            if(player == "Player1")
            {
                CurrentPosition = new GameBoard.Coords(16, 8);
            }
            else
            {
                CurrentPosition = new GameBoard.Coords(0, 8);
            }
            WallsLeft = 10;
        }

        //Written By: Jackson
        public Player(Player player) {
            WallsLeft = player.WallsLeft;
            CurrentPosition = player.CurrentPosition;
            PlayerType = player.PlayerType;
        }
    }
}
