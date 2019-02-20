using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore4.GameCore;

namespace GameCore4.HelperClasses
{
    class Conversions
    {
        public static List<GameBoard.Coords> MoveToArray(string move)
        {
            List<GameBoard.Coords> MoveCoords = new List<GameBoard.Coords>();
            string letter = move[0].ToString();
            letter = letter.ToLower();
            int column = (letter[0] - 'a') * 2;
            int row;

            switch (move[1])
            {
                case '1':
                    row = 16;
                    break;
                case '2':
                    row = 14;
                    break;
                case '3':
                    row = 12;
                    break;
                case '4':
                    row = 10;
                    break;
                case '5':
                    row = 8;
                    break;
                case '6':
                    row = 6;
                    break;
                case '7':
                    row = 4;
                    break;
                case '8':
                    row = 2;
                    break;
                case '9':
                    row = 0;
                    break;
                default:
                    row = 0;
                    break;
            }

            GameBoard.Coords coords1 = new GameBoard.Coords();
            coords1.Row = row;
            coords1.Column = column;

            //wall move
            if (move.Length == 3)
            {
                GameBoard.Coords coords2 = new GameBoard.Coords();
                GameBoard.Coords coords3 = new GameBoard.Coords();
                //vertical
                if (move[2] == 'v')
                {
                    coords1.Column++;
                    coords2.Row = coords1.Row - 1;
                    coords2.Column = coords1.Column;
                    coords3.Row = coords1.Row - 2;
                    coords3.Column = coords1.Column;
                }
                //horizontal
                else
                {
                    coords1.Row--;
                    coords2.Row = coords1.Row;
                    coords2.Column = coords1.Column + 1;
                    coords3.Row = coords1.Row;
                    coords3.Column = coords1.Column + 2;

                }
                MoveCoords.Add(coords3);
                MoveCoords.Add(coords2);
            }
            MoveCoords.Add(coords1);

            //reverse to get the elements in the correct order
            MoveCoords.Reverse();
            return MoveCoords;
        }

        public static string ArrayToMove(List<GameBoard.Coords> MoveCoords)
        {
            string move = "";
            string row = "", column = "";
            //player move
            if (MoveCoords.Count == 1)
            {
                GameBoard.Coords playerMove = MoveCoords[0];
                switch (playerMove.Row)
                {
                    case 16:
                        row = "1";
                        break;
                    case 14:
                        row = "2";
                        break;
                    case 12:
                        row = "3";
                        break;
                    case 10:
                        row = "4";
                        break;
                    case 8:
                        row = "5";
                        break;
                    case 6:
                        row = "6";
                        break;
                    case 4:
                        row = "7";
                        break;
                    case 2:
                        row = "8";
                        break;
                    case 0:
                        row = "9";
                        break;
                    default:
                        row = "0";
                        break;
                }

                switch (playerMove.Column)
                {
                    case 16:
                        column = "i";
                        break;
                    case 14:
                        column = "h";
                        break;
                    case 12:
                        column = "g";
                        break;
                    case 10:
                        column = "f";
                        break;
                    case 8:
                        column = "e";
                        break;
                    case 6:
                        column = "d";
                        break;
                    case 4:
                        column = "c";
                        break;
                    case 2:
                        column = "b";
                        break;
                    case 0:
                        column = "a";
                        break;
                    default:
                        column = "0";
                        break;
                }
                move = column + row;
            }
            //wall move
            else
            {
                GameBoard.Coords coord1 = MoveCoords[0];
                GameBoard.Coords coord2 = MoveCoords[1];
                GameBoard.Coords coord3 = MoveCoords[2];
                //horizontal
                if (coord1.Row == coord2.Row)
                {
                    column = Convert.ToChar((coord1.Column / 2) + 'a').ToString(); 
                    switch (coord1.Row + 1)
                    {
                        case 16:
                            row = "1";
                            break;
                        case 14:
                            row = "2";
                            break;
                        case 12:
                            row = "3";
                            break;
                        case 10:
                            row = "4";
                            break;
                        case 8:
                            row = "5";
                            break;
                        case 6:
                            row = "6";
                            break;
                        case 4:
                            row = "7";
                            break;
                        case 2:
                            row = "8";
                            break;
                        case 0:
                            row = "9";
                            break;
                        default:
                            row = "0";
                            break;
                    }
                    move = column + row + "h";
                }
                //veritcal
                else
                {
                    column = Convert.ToChar(((coord1.Column - 1) / 2) + 'a').ToString();
                    switch (coord1.Row)
                    {
                        case 16:
                            row = "1";
                            break;
                        case 14:
                            row = "2";
                            break;
                        case 12:
                            row = "3";
                            break;
                        case 10:
                            row = "4";
                            break;
                        case 8:
                            row = "5";
                            break;
                        case 6:
                            row = "6";
                            break;
                        case 4:
                            row = "7";
                            break;
                        case 2:
                            row = "8";
                            break;
                        case 0:
                            row = "9";
                            break;
                        default:
                            row = "0";
                            break;
                    }
                    move = column + row + "v";
                }
            }


            return move;
        }
    }
}
