using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GameCore4.HelperClasses;

namespace GameCore4.GameCore
{
    public class GameBoard
    {

        public List<string> RemovedWalls { get; set; }

        public struct Coords
        {
            public Coords(int row, int column)
            {
                Row = row;
                Column = column;
            }
            public int Row { get; set; }
            public int Column { get; set; }
        }
        private int[,] Board { get; set; }
        public bool Winner { get; set; }
        //assigned 2
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public List<string> ValidWallMoves { get; set; }
        public List<string> ValidPlayer1Moves { get; set; }
        public List<string> ValidPlayer2Moves { get; set; }
        private List<string> PlacedWalls { get; set; }

        public GameBoard()
        {
            Player1 = new Player("Player1");
            Player2 = new Player("Player2");
            Board = new int[17, 17];
            Board[0, 8] = 2;
            Board[16, 8] = 2;
            Winner = false;
            ValidWallMoves = new List<string>();
            ValidPlayer1Moves = new List<string>();
            ValidPlayer2Moves = new List<string>();
            PlacedWalls = new List<string>();
            BuildValidWallMoves();
            BuildValidPlayerMoves("Player1");
            BuildValidPlayerMoves("Player2");
            RemovedWalls = new List<string>();
        }

        //Copy constructor
        //Written By: Jackson
        public GameBoard(GameBoard oldGB, string player)
        {
            Player1 = new Player(oldGB.Player1);
            Player2 = new Player(oldGB.Player2);
            Board = (int[,])oldGB.Board.Clone();
            Winner = false;
            ValidWallMoves = new List<string>(oldGB.ValidWallMoves);
            ValidPlayer1Moves = new List<string>(oldGB.ValidPlayer1Moves);
            PlacedWalls = new List<string>(oldGB.PlacedWalls);
            GetValidPlayerMoves(player);
        }

        public bool IsWinner()
        {
            if (Player1.CurrentPosition.Row == 0)
            {
                Winner = true;
                return true;
            }
            if (Player2.CurrentPosition.Row == 16)
            {
                Winner = true;
                return true;
            }
            return false;
        }

        //ran at start
        private void BuildValidWallMoves()
        {
            string move = "";
            //build all avalible wall moves as strings
            for (int i = 1; i < 9; i++)
            {
                move = "a" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "b" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "c" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "d" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "e" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "f" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "g" + i.ToString() + "v";
                ValidWallMoves.Add(move);
                move = "h" + i.ToString() + "v";
                ValidWallMoves.Add(move);
            }
            for (int i = 1; i < 9; i++)
            {
                move = "a" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "b" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "c" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "d" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "e" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "f" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "g" + i.ToString() + "h";
                ValidWallMoves.Add(move);
                move = "h" + i.ToString() + "h";
                ValidWallMoves.Add(move);
            }
        }

        //ran at start
        private void BuildValidPlayerMoves(string player)
        {
            Coords position = new Coords();
            Coords coord1 = new Coords();
            Coords coord2 = new Coords();
            Coords coord3 = new Coords();
            if (player == "Player1")
            {
                position = Player1.CurrentPosition;
                //up
                coord1 = new Coords(position.Row - 2, position.Column);
                //right
                coord2 = new Coords(position.Row, position.Column + 2);
                //left
                coord3 = new Coords(position.Row, position.Column - 2);

                ValidPlayer1Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord1 }));
                ValidPlayer1Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord2 }));
                ValidPlayer1Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord3 }));
            }
            else
            {
                position = Player2.CurrentPosition;
                //down
                coord1 = new Coords(position.Row + 2, position.Column);
                //right
                coord2 = new Coords(position.Row, position.Column + 2);
                //left
                coord3 = new Coords(position.Row, position.Column - 2);

                ValidPlayer2Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord1 }));
                ValidPlayer2Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord2 }));
                ValidPlayer2Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord3 }));
            }
        }

        //ONLY FOR CONSOLE APP
        public void PrintBoard()
        {
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    Console.Write(Board[i, j] + " ");
                }
                Console.Write("\n");
            }
        }

        public void MakeMove(string move, bool player)
        {
            List<GameBoard.Coords> Coords = Conversions.MoveToArray(move);
            if(Coords.Count == 1)
            {
                if(player)
                {
                    MovePlayer(Coords[0], "Player1");
                }
                else
                {
                    MovePlayer(Coords[0], "Player2");
                }
            }
            else
            {
                PlaceWall(move);
                if (player)
                {
                    Player1.WallsLeft--;
                }
                else
                {
                    Player2.WallsLeft--;
                }
            }

            //empties the valid wall moves if the player has no walls left
            /// TODO
            // *IMPORTANT* if we end up using player2 perspective as well then this will remove valid moves for them as well
            ///
            if (Player1.WallsLeft <= 0)
            {
                ValidWallMoves = new List<string>();
            }
        }

        //places a wall given by the UI, given a string to store in the PlacedWalls list
        private void PlaceWall(string move)
        {
            List<Coords> coords = Conversions.MoveToArray(move);
            Coords coord1 = coords[0];
            Coords coord2 = coords[1];
            Coords coord3 = coords[2];

            PlacedWalls.Add(move);
            ValidWallMoves.Remove(move);
            RemovedWalls.Add(move);

            Wall wall = new Wall(coord1, coord2, coord3);

            Board[coord1.Row, coord1.Column] = 1;
            Board[coord2.Row, coord2.Column] = 1;
            Board[coord3.Row, coord3.Column] = 1;

            GetValidWallMoves(coords);
            //check the walls around each player after a wall is placed
            UpdateWallsAfterPlayerMove(Player1.CurrentPosition);
            UpdateWallsAfterPlayerMove(Player2.CurrentPosition);

            List<Coords> player1Moves = GetValidPlayerMoves("Player1");
            List<Coords> player2Moves = GetValidPlayerMoves("Player2");

            ValidPlayer1Moves = new List<string>();
            foreach (Coords coord in player1Moves)
            {
                ValidPlayer1Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord }));
            }

            ValidPlayer2Moves = new List<string>();
            foreach (Coords coord in player2Moves)
            {
                ValidPlayer2Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord }));
            }
        }

        //moves a player
        public void MovePlayer(Coords destination, string player)
        {
            Coords oldPosition = new Coords();
            Coords newPosition = destination;
            if (player == "Player1")
            {
                oldPosition = Player1.CurrentPosition;
                Board[Player1.CurrentPosition.Row, Player1.CurrentPosition.Column] = 0;
                Board[destination.Row, destination.Column] = 2;
                Player1.CurrentPosition = destination;
            }
            else
            {
                oldPosition = Player2.CurrentPosition;
                Board[Player2.CurrentPosition.Row, Player2.CurrentPosition.Column] = 0;
                Board[destination.Row, destination.Column] = 2;
                Player2.CurrentPosition = destination;
            }
            UpdateWallsAfterPlayerMove(newPosition);
            List<Coords> player1Moves = GetValidPlayerMoves("Player1");
            List<Coords> player2Moves = GetValidPlayerMoves("Player2");

            ValidPlayer1Moves = new List<string>();
            foreach (Coords coord in player1Moves)
            {
                ValidPlayer1Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord }));
            }

            ValidPlayer2Moves = new List<string>();
            foreach (Coords coord in player2Moves)
            {
                ValidPlayer2Moves.Add(Conversions.ArrayToMove(new List<Coords> { coord }));
            }
        }

        //checks the walls around the player after a pawn is moved
        private void UpdateWallsAfterPlayerMove(Coords newPosition)
        {
            #region Remove?
            ////find all walls around old position
            //Coords coord1 = new Coords(oldPosition.Row - 1, oldPosition.Column - 2);
            //Coords coord2 = new Coords(oldPosition.Row - 1, oldPosition.Column - 1);
            //Coords coord3 = new Coords(oldPosition.Row - 1, oldPosition.Column);
            //Wall topLeftHWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row - 1, oldPosition.Column);
            //coord2 = new Coords(oldPosition.Row - 1, oldPosition.Column + 1);
            //coord3 = new Coords(oldPosition.Row - 1, oldPosition.Column + 2);
            //Wall topRightHWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row + 1, oldPosition.Column);
            //coord2 = new Coords(oldPosition.Row + 1, oldPosition.Column + 1);
            //coord3 = new Coords(oldPosition.Row + 1, oldPosition.Column + 2);
            //Wall bottomRightHWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row - 1, oldPosition.Column - 2);
            //coord2 = new Coords(oldPosition.Row - 1, oldPosition.Column - 1);
            //coord3 = new Coords(oldPosition.Row - 1, oldPosition.Column);
            //Wall bottomLeftHWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row, oldPosition.Column + 1);
            //coord2 = new Coords(oldPosition.Row - 1, oldPosition.Column + 1);
            //coord3 = new Coords(oldPosition.Row - 2, oldPosition.Column + 1);
            //Wall topRightVWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row + 2, oldPosition.Column - 1);
            //coord2 = new Coords(oldPosition.Row + 1, oldPosition.Column - 1);
            //coord3 = new Coords(oldPosition.Row, oldPosition.Column - 1);
            //Wall topLeftVWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row, oldPosition.Column - 1);
            //coord2 = new Coords(oldPosition.Row - 1, oldPosition.Column - 1);
            //coord3 = new Coords(oldPosition.Row - 2, oldPosition.Column - 1);
            //Wall bottomLeftVWall = new Wall(coord1, coord2, coord3);

            //coord1 = new Coords(oldPosition.Row + 2, oldPosition.Column + 1);
            //coord2 = new Coords(oldPosition.Row + 1, oldPosition.Column + 1);
            //coord3 = new Coords(oldPosition.Row, oldPosition.Column + 1);
            //Wall bottomRightVWall = new Wall(coord1, coord2, coord3);

            //List<Wall> walls = new List<Wall>
            //{
            //    topLeftHWall,
            //    topLeftVWall,
            //    topRightHWall,
            //    topRightVWall,
            //    bottomLeftHWall,
            //    bottomLeftVWall,
            //    bottomRightHWall,
            //    bottomRightVWall
            //};

            /////////////////////////
            ////call tryPlaceWall and pass it all the walls
            //foreach (Wall wall in walls)
            //{
            //    if (!TryPlaceWall(wall))
            //    {
            //        if (ValidWallMoves.Remove(Conversions.ArrayToMove(wall.Coords)))
            //        {
            //            RemovedWalls.Add(Conversions.ArrayToMove(wall.Coords));
            //        }                    
            //    }
            //    else
            //    {
            //        if (!ValidWallMoves.Contains(Conversions.ArrayToMove(wall.Coords)))
            //        {
            //            ValidWallMoves.Add(Conversions.ArrayToMove(wall.Coords));
            //        }
            //    }
            //}
            #endregion

            //find all walls around new position
            Coords coord1 = new Coords(newPosition.Row - 1, newPosition.Column - 2);
            Coords coord2 = new Coords(newPosition.Row - 1, newPosition.Column - 1);
            Coords coord3 = new Coords(newPosition.Row - 1, newPosition.Column);
            Wall topLeftHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row - 1, newPosition.Column);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row - 1, newPosition.Column + 2);
            Wall topRightHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 1, newPosition.Column);
            coord2 = new Coords(newPosition.Row + 1, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row + 1, newPosition.Column + 2);
            Wall bottomRightHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row - 1, newPosition.Column - 2);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column - 1);
            coord3 = new Coords(newPosition.Row - 1, newPosition.Column);
            Wall bottomLeftHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row, newPosition.Column + 1);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row - 2, newPosition.Column + 1);
            Wall topRightVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 2, newPosition.Column - 1);
            coord2 = new Coords(newPosition.Row + 1, newPosition.Column - 1);
            coord3 = new Coords(newPosition.Row, newPosition.Column - 1);
            Wall topLeftVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row, newPosition.Column - 1);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column - 1);
            coord3 = new Coords(newPosition.Row - 2, newPosition.Column - 1);
            Wall bottomLeftVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 2, newPosition.Column + 1);
            coord2 = new Coords(newPosition.Row + 1, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row, newPosition.Column + 1);
            Wall bottomRightVWall = new Wall(coord1, coord2, coord3);


            //far walls
            coord1 = new Coords(newPosition.Row + 2, newPosition.Column + 3);
            coord2 = new Coords(newPosition.Row + 1, newPosition.Column + 3);
            coord3 = new Coords(newPosition.Row, newPosition.Column + 3);
            Wall farRightBottomVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row, newPosition.Column + 3);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column + 3);
            coord3 = new Coords(newPosition.Row - 2, newPosition.Column + 3);
            Wall farRightTopVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row - 3, newPosition.Column);
            coord2 = new Coords(newPosition.Row - 3, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row - 3, newPosition.Column + 2);
            Wall farTopRightHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row - 3, newPosition.Column - 2);
            coord2 = new Coords(newPosition.Row - 3, newPosition.Column - 1);
            coord3 = new Coords(newPosition.Row - 3, newPosition.Column);
            Wall farTopLeftHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 2, newPosition.Column - 3);
            coord2 = new Coords(newPosition.Row + 1, newPosition.Column - 3);
            coord3 = new Coords(newPosition.Row, newPosition.Column - 3);
            Wall farLeftBottomVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row, newPosition.Column - 3);
            coord2 = new Coords(newPosition.Row - 1, newPosition.Column - 3);
            coord3 = new Coords(newPosition.Row - 2, newPosition.Column - 3);
            Wall farLeftTopVWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 3, newPosition.Column);
            coord2 = new Coords(newPosition.Row + 3, newPosition.Column + 1);
            coord3 = new Coords(newPosition.Row + 3, newPosition.Column + 2);
            Wall farBottomRightHWall = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(newPosition.Row + 3, newPosition.Column - 2);
            coord2 = new Coords(newPosition.Row + 3, newPosition.Column - 1);
            coord3 = new Coords(newPosition.Row + 3, newPosition.Column);
            Wall farBottomLeftHWall = new Wall(coord1, coord2, coord3);

            List<Wall> wallsNew = new List<Wall>
            {
                topLeftHWall,
                topLeftVWall,
                topRightHWall,
                topRightVWall,
                bottomLeftHWall,
                bottomLeftVWall,
                bottomRightHWall,
                bottomRightVWall,
                farRightBottomVWall,
                farRightTopVWall,
                farLeftBottomVWall,
                farLeftTopVWall,
                farRightBottomVWall,
                farRightTopVWall,
                farTopLeftHWall,
                farTopRightHWall
            };

            ///////////////////////
            //call tryPlaceWall and pass it all the walls
            foreach (Wall wall in wallsNew)
            {
                if (!TryPlaceWall(wall))
                {
                    if (ValidWallMoves.Remove(Conversions.ArrayToMove(wall.Coords)))
                    {
                        RemovedWalls.Add(Conversions.ArrayToMove(wall.Coords));
                    }
                }
                else
                {
                    if (!ValidWallMoves.Contains(Conversions.ArrayToMove(wall.Coords)))
                    {
                        ValidWallMoves.Add(Conversions.ArrayToMove(wall.Coords));
                    }
                }
            }
        }

        //generates new player moves after pawn is moved
        private List<Coords> GetValidPlayerMoves(string player)
        {
            Coords currentPosition;
            Coords enemyPosition;
            if (player == "Player1")
            {
                currentPosition = Player1.CurrentPosition;
                enemyPosition = Player2.CurrentPosition;
            }
            else
            {
                currentPosition = Player2.CurrentPosition;
                enemyPosition = Player1.CurrentPosition;
            }

            //generate new player possible moves
            List<Coords> playerMoves = new List<Coords>();
            //up
            if (currentPosition.Row != 0)
            {
                if (Board[currentPosition.Row - 1, currentPosition.Column] == 0)
                {
                    playerMoves.Add(new Coords(currentPosition.Row - 2, currentPosition.Column));
                }
            }
            //left
            if (currentPosition.Column != 0)
            {
                if (Board[currentPosition.Row, currentPosition.Column - 1] == 0)
                {
                    playerMoves.Add(new Coords(currentPosition.Row, currentPosition.Column - 2));
                }
            }
            //right
            if (currentPosition.Column != 16)
            {
                if (Board[currentPosition.Row, currentPosition.Column + 1] == 0)
                {
                    playerMoves.Add(new Coords(currentPosition.Row, currentPosition.Column + 2));
                }
            }
            //down
            if (currentPosition.Row != 16)
            {
                if (Board[currentPosition.Row + 1, currentPosition.Column] == 0)
                {
                    playerMoves.Add(new Coords(currentPosition.Row + 2, currentPosition.Column));
                }
            }

            //if another player is at the spot you want to move to, generate new appropiate moves
            bool up = false, down = false, left = false, right = false;
            Coords upMove = new Coords(), downMove = new Coords(), leftMove = new Coords(), rightMove = new Coords();
            List<Coords> possibleNewMoves = new List<Coords>();
            Coords toRemove = new Coords(-1, -1);
            foreach (Coords coord in playerMoves)
            {
                if (coord.Row == enemyPosition.Row && coord.Column == enemyPosition.Column)
                {
                    //consider where the opponent pawn is in relation to your own
                    if (currentPosition.Row == enemyPosition.Row)
                    {
                        if (enemyPosition.Column < currentPosition.Column)
                        {
                            left = true;
                        }
                        else
                        {
                            right = true;
                        }
                    }
                    else if (currentPosition.Column == enemyPosition.Column)
                    {
                        if (enemyPosition.Row < currentPosition.Row)
                        {
                            up = true;
                        }
                        else
                        {
                            down = true;
                        }
                    }

                    /////////////////////////////////////
                    //add all possible new moves from the enemy position to a list

                    //up
                    if (enemyPosition.Row != 0)
                    {
                        if (Board[enemyPosition.Row - 1, enemyPosition.Column] == 0)
                        {
                            upMove = new Coords(enemyPosition.Row - 2, enemyPosition.Column);
                            possibleNewMoves.Add(upMove);
                        }
                    }
                    //left
                    if (enemyPosition.Column != 0)
                    {
                        if (Board[enemyPosition.Row, enemyPosition.Column - 1] == 0)
                        {
                            leftMove = new Coords(enemyPosition.Row, enemyPosition.Column - 2);
                            possibleNewMoves.Add(leftMove);
                        }
                    }
                    //right
                    if (enemyPosition.Column != 16)
                    {
                        if (Board[enemyPosition.Row, enemyPosition.Column + 1] == 0)
                        {
                            rightMove = new Coords(enemyPosition.Row, enemyPosition.Column + 2);
                            possibleNewMoves.Add(rightMove);
                        }
                    }
                    //down
                    if (enemyPosition.Row != 16)
                    {
                        if (Board[enemyPosition.Row + 1, enemyPosition.Column] == 0)
                        {
                            downMove = new Coords(enemyPosition.Row + 2, enemyPosition.Column);
                            possibleNewMoves.Add(downMove);
                        }
                    }
                    toRemove = coord;
                }
            }

            //determine the actual possible moves and add them to the playerMoves list
            // Looks for a wall behind the player we are trying to jump over, if there is a wall there
            // try to add the left and right jump to the player move list
            if (up)
            {
                if (possibleNewMoves.Contains(upMove))
                {
                    playerMoves.Add(upMove);
                }
                else
                {
                    if (possibleNewMoves.Contains(rightMove))
                    {
                        playerMoves.Add(rightMove);
                    }
                    if (possibleNewMoves.Contains(leftMove))
                    {
                        playerMoves.Add(leftMove);
                    }
                }
            }

            if (down)
            {
                if (possibleNewMoves.Contains(downMove))
                {
                    playerMoves.Add(downMove);
                }
                else
                {
                    if (possibleNewMoves.Contains(rightMove))
                    {
                        playerMoves.Add(rightMove);
                    }
                    if (possibleNewMoves.Contains(leftMove))
                    {
                        playerMoves.Add(leftMove);
                    }
                }
            }

            if (left)
            {
                if (possibleNewMoves.Contains(leftMove))
                {
                    playerMoves.Add(leftMove);
                }
                else
                {
                    if (possibleNewMoves.Contains(upMove))
                    {
                        playerMoves.Add(upMove);
                    }
                    if (possibleNewMoves.Contains(downMove))
                    {
                        playerMoves.Add(downMove);
                    }
                }
            }

            if (right)
            {
                if (possibleNewMoves.Contains(rightMove))
                {
                    playerMoves.Add(rightMove);
                }
                else
                {
                    if (possibleNewMoves.Contains(upMove))
                    {
                        playerMoves.Add(upMove);
                    }
                    if (possibleNewMoves.Contains(downMove))
                    {
                        playerMoves.Add(downMove);
                    }
                }
            }
            //////////////////////////////////////


            //THIS MIGHT NOT WORK BASED ON HOW toRemove = coord; WORKS WITH STRUCTS
            if (toRemove.Row != -1)
            {
                playerMoves.Remove(toRemove);
            }
            //foreach (var m in playerMoves)
            //{
            //    Debug.Log($"GetValidPlayerMove result: {m.Row} {m.Column}");
            //    Debug.Log("GetValidPlayerMove result: " + Conversions.ArrayToMove(new List<Coords> { m }));
            //}
            return playerMoves;
        }

        //updates valid walls based on the wall coords passed
        private void GetValidWallMoves(List<Coords> coords)
        {
            //horizontal vs vertical walls function to determine adjacent move validity
            if (coords[0].Row == coords[1].Row)
            {
                HorizontalWallUpdate(coords);
            }
            else
            {
                VerticalWallUpdate(coords);
            }
        }

        //Used by GetValidWallMoves to check adjacent walls when a horizontal wall is placed
        private void HorizontalWallUpdate(List<Coords> coords)
        {
            //build every wall using center coordinate
            Coords position = coords[1];

            //center horizontal walls
            Coords coord1 = new Coords(position.Row, position.Column - 5);
            Coords coord2 = new Coords(position.Row, position.Column - 4);
            Coords coord3 = new Coords(position.Row, position.Column - 3);
            Wall left1 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column - 3);
            coord2 = new Coords(position.Row, position.Column - 2);
            coord3 = new Coords(position.Row, position.Column - 1);
            Wall left2 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column + 1);
            coord2 = new Coords(position.Row, position.Column + 2);
            coord3 = new Coords(position.Row, position.Column + 3);
            Wall right1 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column + 3);
            coord2 = new Coords(position.Row, position.Column + 4);
            coord3 = new Coords(position.Row, position.Column + 5);
            Wall right2 = new Wall(coord1, coord2, coord3);


            //center vertical walls
            coord1 = new Coords(position.Row + 1, position.Column - 2);
            coord2 = new Coords(position.Row, position.Column - 2);
            coord3 = new Coords(position.Row - 1, position.Column - 2);
            Wall left = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 1, position.Column);
            coord2 = new Coords(position.Row, position.Column);
            coord3 = new Coords(position.Row - 1, position.Column);
            Wall center = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 1, position.Column + 2);
            coord2 = new Coords(position.Row, position.Column + 2);
            coord3 = new Coords(position.Row - 1, position.Column + 2);
            Wall right = new Wall(coord1, coord2, coord3);


            //top vertical walls
            coord1 = new Coords(position.Row - 1, position.Column - 2);
            coord2 = new Coords(position.Row - 2, position.Column - 2);
            coord3 = new Coords(position.Row - 3, position.Column - 2);
            Wall topLeft = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row - 1, position.Column);
            coord2 = new Coords(position.Row - 2, position.Column);
            coord3 = new Coords(position.Row - 3, position.Column);
            Wall topCenter = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row - 1, position.Column + 2);
            coord2 = new Coords(position.Row - 2, position.Column + 2);
            coord3 = new Coords(position.Row - 3, position.Column + 2);
            Wall topRight = new Wall(coord1, coord2, coord3);


            //bottom vertical walls
            coord1 = new Coords(position.Row + 3, position.Column - 2);
            coord2 = new Coords(position.Row + 2, position.Column - 2);
            coord3 = new Coords(position.Row + 1, position.Column - 2);
            Wall bottomLeft = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 3, position.Column);
            coord2 = new Coords(position.Row + 2, position.Column);
            coord3 = new Coords(position.Row + 1, position.Column);
            Wall bottomCenter = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 3, position.Column + 2);
            coord2 = new Coords(position.Row + 2, position.Column + 2);
            coord3 = new Coords(position.Row + 1, position.Column + 2);
            Wall bottomRight = new Wall(coord1, coord2, coord3);

            List<Wall> walls = new List<Wall>
            {
                left1,
                left2,
                right1,
                right2,
                left,
                center,
                right,
                topLeft,
                topCenter,
                topRight,
                bottomLeft,
                bottomCenter,
                bottomRight
            };
            foreach (Wall wall in walls)
            {
                if (!TryPlaceWall(wall))
                {
                    if (ValidWallMoves.Remove(Conversions.ArrayToMove(wall.Coords)))
                    {
                        RemovedWalls.Add(Conversions.ArrayToMove(wall.Coords));
                    }
                }
                else
                {
                    //New Wall Placement Should never make more walls available
                    //if (!ValidWallMoves.Contains(Conversions.ArrayToMove(wall.Coords)))
                    //{
                    //    ValidWallMoves.Add(Conversions.ArrayToMove(wall.Coords));
                    //}
                }
            }
        }

        //Used by GetValidWallMoves to check adjacent walls when a vertical wall is placed
        private void VerticalWallUpdate(List<Coords> coords)
        {
            //build every wall using center coordinate
            Coords position = coords[1];

            //center vertical walls
            Coords coord1 = new Coords(position.Row - 5, position.Column);
            Coords coord2 = new Coords(position.Row - 4, position.Column);
            Coords coord3 = new Coords(position.Row - 3, position.Column);
            Wall top1 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row - 3, position.Column);
            coord2 = new Coords(position.Row - 2, position.Column);
            coord3 = new Coords(position.Row - 1, position.Column);
            Wall top2 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 5, position.Column);
            coord2 = new Coords(position.Row + 4, position.Column);
            coord3 = new Coords(position.Row + 3, position.Column);
            Wall bottom1 = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 3, position.Column);
            coord2 = new Coords(position.Row + 2, position.Column);
            coord3 = new Coords(position.Row + 1, position.Column);
            Wall bottom2 = new Wall(coord1, coord2, coord3);


            //center horizontal walls
            coord1 = new Coords(position.Row - 2, position.Column - 1);
            coord2 = new Coords(position.Row - 2, position.Column);
            coord3 = new Coords(position.Row - 2, position.Column + 1);
            Wall top = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column - 1);
            coord2 = new Coords(position.Row, position.Column);
            coord3 = new Coords(position.Row, position.Column + 1);
            Wall center = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 2, position.Column - 1);
            coord2 = new Coords(position.Row + 2, position.Column);
            coord3 = new Coords(position.Row + 2, position.Column + 1);
            Wall bottom = new Wall(coord1, coord2, coord3);


            //left horizontal walls
            coord1 = new Coords(position.Row - 2, position.Column - 3);
            coord2 = new Coords(position.Row - 2, position.Column - 2);
            coord3 = new Coords(position.Row - 2, position.Column - 1);
            Wall leftTop = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column - 3);
            coord2 = new Coords(position.Row, position.Column - 2);
            coord3 = new Coords(position.Row, position.Column - 1);
            Wall leftCenter = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 2, position.Column - 3);
            coord2 = new Coords(position.Row + 2, position.Column - 2);
            coord3 = new Coords(position.Row + 2, position.Column - 1);
            Wall leftBottom = new Wall(coord1, coord2, coord3);


            //right horizontal walls
            coord1 = new Coords(position.Row - 2, position.Column + 1);
            coord2 = new Coords(position.Row - 2, position.Column + 2);
            coord3 = new Coords(position.Row - 2, position.Column + 3);
            Wall rightTop = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row, position.Column + 1);
            coord2 = new Coords(position.Row, position.Column + 2);
            coord3 = new Coords(position.Row, position.Column + 3);
            Wall rightCenter = new Wall(coord1, coord2, coord3);

            coord1 = new Coords(position.Row + 2, position.Column + 1);
            coord2 = new Coords(position.Row + 2, position.Column + 2);
            coord3 = new Coords(position.Row + 2, position.Column + 3);
            Wall rightBottom = new Wall(coord1, coord2, coord3);

            List<Wall> walls = new List<Wall>
            {
                top1,
                top2,
                bottom1,
                bottom2,
                top,
                center,
                bottom,
                leftTop,
                leftCenter,
                leftBottom,
                rightTop,
                rightCenter,
                rightBottom
            };
            foreach (Wall wall in walls)
            {
                if (!TryPlaceWall(wall))
                {
                    if (ValidWallMoves.Remove(Conversions.ArrayToMove(wall.Coords)))
                    {
                        RemovedWalls.Add(Conversions.ArrayToMove(wall.Coords));
                    }
                }
                else
                {
                    //New Wall Placement Should never make more walls available
                    //if (!ValidWallMoves.Contains(Conversions.ArrayToMove(wall.Coords)))
                    //{
                    //    ValidWallMoves.Add(Conversions.ArrayToMove(wall.Coords));
                    //}
                }
            }
        }

        //used to determine wall validity for both checking functions
        //try to place a wall on a *COPY* of the game board, if not a valid move, return false
        private bool TryPlaceWall(Wall wall)
        {
            Coords coord1 = wall.Coords[0];
            Coords coord2 = wall.Coords[1];
            Coords coord3 = wall.Coords[2];

            //check for out of bounds
            if (coord1.Row > 16 || coord1.Row < 0 || coord1.Column > 16 || coord1.Column < 0 ||
               coord2.Row > 16 || coord2.Row < 0 || coord2.Column > 16 || coord2.Column < 0 ||
               coord3.Row > 16 || coord3.Row < 0 || coord3.Column > 16 || coord3.Column < 0)
            {
                return false;
            }

            //check for overlapping walls
            if (Board[coord1.Row, coord1.Column] != 0 || Board[coord2.Row, coord2.Column] != 0 || Board[coord3.Row, coord3.Column] != 0)
            {
                return false;
            }

            int[,] tempBoard = (int[,])Board.Clone();

            tempBoard[coord1.Row, coord1.Column] = 1;
            tempBoard[coord2.Row, coord2.Column] = 1;
            tempBoard[coord3.Row, coord3.Column] = 1;
            //if the wall is placed is there an exit
            if (!CheckExitPlayer1(tempBoard) || !CheckExitPlayer2(tempBoard))
            {
                return false;
            }

            return true;
        }

        private bool CheckExitPlayer1(int[,] board)
        {
            //Player 1 is moving to row 0
            Stack<Coords> Stack = new Stack<Coords>();
            Stack<Coords> Vistited = new Stack<Coords>();
            Coords currentSpot = Player1.CurrentPosition;
            bool moveMade;
            string moveType = "";
            Vistited.Push(currentSpot);

            //try to move in a direction starting with up, if a move is made push all other avaliable moves
            //  onto the stack unless it has already been vistited
            //if a move can't be made and the stack is empty the player is enclosed
            bool exit = false;
            while (!exit)
            {
                moveMade = false;
                //up
                if (currentSpot.Row != 0)
                {
                    if (board[currentSpot.Row - 1, currentSpot.Column] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row - 2, currentSpot.Column);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "up";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }

                    }
                }
                //left
                if (currentSpot.Column != 0)
                {
                    if (board[currentSpot.Row, currentSpot.Column - 1] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row, currentSpot.Column - 2);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "left";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }
                    }
                }
                //right
                if (currentSpot.Column != 16)
                {
                    if (board[currentSpot.Row, currentSpot.Column + 1] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row, currentSpot.Column + 2);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "right";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }
                    }
                }
                //down
                if (currentSpot.Row != 16)
                {
                    if (board[currentSpot.Row + 1, currentSpot.Column] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row + 2, currentSpot.Column);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "down";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }
                    }
                }

                if (moveMade)
                {
                    switch (moveType)
                    {
                        case "up":
                            currentSpot.Row -= 2;
                            break;
                        case "down":
                            currentSpot.Row += 2;
                            break;
                        case "left":
                            currentSpot.Column -= 2;
                            break;
                        case "right":
                            currentSpot.Column += 2;
                            break;
                    }
                    Vistited.Push(currentSpot);
                }
                else
                {
                    if (Stack.Count > 0)
                    {
                        currentSpot = Stack.Pop();
                    }
                }
                //exit found
                if (currentSpot.Row == 0)
                {
                    exit = true;
                    break;
                }
                //exit impossible
                if (!moveMade && Stack.Count == 0)
                {
                    exit = false;
                    break;
                }
            }
            return exit;
        }

        private bool CheckExitPlayer2(int[,] board)
        {
            //Player 2 is moving to row 16
            Stack<Coords> Stack = new Stack<Coords>();
            Stack<Coords> Vistited = new Stack<Coords>();
            Coords currentSpot = Player2.CurrentPosition;
            bool moveMade;
            string moveType = "";
            Vistited.Push(currentSpot);
            bool exit = false;
            while (!exit)
            {
                moveMade = false;

                //down
                if (currentSpot.Row != 16)
                {
                    if (board[currentSpot.Row + 1, currentSpot.Column] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row + 2, currentSpot.Column);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "down";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }
                    }
                }
                //left
                if (currentSpot.Column != 0)
                {
                    if (board[currentSpot.Row, currentSpot.Column - 1] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row, currentSpot.Column - 2);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "left";
                            }
                            else
                            {

                                Stack.Push(coords);
                            }
                        }
                    }
                }
                //right
                if (currentSpot.Column != 16)
                {
                    if (board[currentSpot.Row, currentSpot.Column + 1] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row, currentSpot.Column + 2);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "right";
                            }
                            else
                            {
                                Stack.Push(coords);
                            }
                        }
                    }
                }
                //up
                if (currentSpot.Row != 0)
                {
                    if (board[currentSpot.Row - 1, currentSpot.Column] == 0)
                    {
                        Coords coords = new Coords(currentSpot.Row - 2, currentSpot.Column);
                        if (!Vistited.Contains(coords))
                        {
                            if (!moveMade)
                            {
                                moveMade = true;
                                moveType = "up";
                            }
                            else
                            {

                                Stack.Push(coords);

                            }
                        }
                    }
                }

                if (moveMade)
                {
                    switch (moveType)
                    {
                        case "up":
                            currentSpot.Row -= 2;
                            break;
                        case "down":
                            currentSpot.Row += 2;
                            break;
                        case "left":
                            currentSpot.Column -= 2;
                            break;
                        case "right":
                            currentSpot.Column += 2;
                            break;
                    }
                    Vistited.Push(currentSpot);
                }
                else
                {
                    if (Stack.Count > 0)
                    {
                        currentSpot = Stack.Pop();
                    }
                }
                if (currentSpot.Row == 16)
                {
                    exit = true;
                    break;
                }
                if (!moveMade && Stack.Count == 0)
                {
                    exit = false;
                    break;
                }
            }
            return exit;
        }

    }
}
