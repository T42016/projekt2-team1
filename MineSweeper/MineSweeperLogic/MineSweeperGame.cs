using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeperLogic
{
    public class MineSweeperGame
    {

        public MineSweeperGame(int sizeX, int sizeY, int nrOfMines, IServiceBus bus)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            NumberOfMines = nrOfMines;
            if (nrOfMines > SizeX * SizeY)
                NumberOfMines = (SizeX * SizeY) / (sizeX);
            Bus = bus;
            PosInfo = new PositionInfo[SizeX, SizeY];
            ResetBoard();
        }

        private IServiceBus Bus;
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public int SizeX { get; }
        public int SizeY { get; }
        public int NumberOfMines { get; }
        public GameState State { get; private set; }
        public int nrOfOpened { get; private set; }
        public int nrOfFlagged { get; private set; }


        private PositionInfo[,] PosInfo;
        private string symb;

        public PositionInfo GetCoordinate(int x, int y)
        {
            return PosInfo[x, y];
        }

        public void FlagCoordinate()
        {
            if (!PosInfo[PosX, PosY].IsOpen)
            {
                if (!PosInfo[PosX, PosY].IsFlagged)
                    PosInfo[PosX, PosY].IsFlagged = true;
                else
                    PosInfo[PosX, PosY].IsFlagged = false;
            }


            nrOfFlagged = 0;
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (PosInfo[x, y].IsFlagged)
                    {
                        nrOfFlagged++;
                    }
                }
            }
        }

        public void ClickCoordinate()
        {
            if (!PosInfo[PosX, PosY].IsOpen && !PosInfo[PosX, PosY].IsFlagged)
            {

                if (PosInfo[PosX, PosY].HasMine)
                {

                    for (int y = 0; y < PosInfo.GetLength(1); y++)
                    {
                        for (int x = 0; x < PosInfo.GetLength(0); x++)
                        {
                            if (PosInfo[x, y].HasMine)
                                PosInfo[x, y].IsOpen = true;
                        }
                    }
                    State = GameState.Lost;
                }


                else if (PosInfo[PosX, PosY].NrOfNeighbours != 0)
                {
                    PosInfo[PosX, PosY].IsOpen = true;


                    nrOfOpened = 0;
                    for (int y = 0; y < PosInfo.GetLength(1); y++)
                    {
                        for (int x = 0; x < PosInfo.GetLength(0); x++)
                        {
                            if (PosInfo[x, y].IsOpen && !PosInfo[x, y].HasMine)
                            {
                                nrOfOpened++;
                            }
                        }
                    }
                    if (nrOfOpened == ((SizeX * SizeY) - NumberOfMines))
                        State = GameState.Won;
                }

                else
                {
                    FloodFill(PosX, PosY);


                    nrOfOpened = 0;
                    for (int y = 0; y < PosInfo.GetLength(1); y++)
                    {
                        for (int x = 0; x < PosInfo.GetLength(0); x++)
                        {
                            if (PosInfo[x, y].IsOpen && !PosInfo[x, y].HasMine)
                            {
                                nrOfOpened++;
                            }
                        }
                    }
                    if (nrOfOpened == ((SizeX * SizeY) - NumberOfMines))
                        State = GameState.Won;
                }
            }


            nrOfFlagged = 0;
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (PosInfo[x, y].IsFlagged)
                    {
                        nrOfFlagged++;
                    }
                }
            }
        }

        public void ResetBoard()
        {

            for (int y = 0; y < PosInfo.GetLength(1); y++)
            {
                for (int x = 0; x < PosInfo.GetLength(0); x++)
                {
                    PosInfo[x, y] = new PositionInfo();
                    PosInfo[x, y].X = x;
                    PosInfo[x, y].Y = y;
                }
            }


            int currentMines = 0;
            while (currentMines < NumberOfMines)
            {
                int randX = Bus.Next(SizeX);
                int randY = Bus.Next(SizeY);
                if (PosInfo[randX, randY].HasMine == false)
                {
                    PosInfo[randX, randY].HasMine = true;
                    currentMines++;
                }
            }


            for (int y = 0; y < PosInfo.GetLength(1); y++)
            {
                for (int x = 0; x < PosInfo.GetLength(0); x++)
                {

                    if (PosInfo[x, y].Y == 0 && PosInfo[x, y].X == 0)
                    {
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].Y == 0 && PosInfo[x, y].X == (SizeX - 1))
                    {
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].Y == (SizeY - 1) && PosInfo[x, y].X == 0)
                    {
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].Y == (SizeY - 1) && PosInfo[x, y].X == (SizeX - 1))
                    {
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].Y == 0)
                    {
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].Y == (SizeY - 1))
                    {
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].X == 0)
                    {
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else if (PosInfo[x, y].X == (SizeX - 1))
                    {
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }


                    else
                    {
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }
                }
            }
            State = GameState.Playing;
            nrOfOpened = 0;
            nrOfFlagged = 0;
        }

        public void DrawBoard()
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (x == PosX && y == PosY)
                    {
                        if (GetCoordinate(x, y).IsFlagged)
                            symb = "! ";
                        else if (GetCoordinate(x, y).IsOpen)
                        {
                            if (GetCoordinate(x, y).HasMine)
                                symb = "X ";
                            else if (GetCoordinate(x, y).NrOfNeighbours == 0)
                                symb = ". ";
                            else
                                symb = GetCoordinate(x, y).NrOfNeighbours + " ";
                        }
                        else
                            symb = "? ";
                        Bus.Write(symb, ConsoleColor.DarkCyan);
                    }
                    else
                    {
                        if (GetCoordinate(x, y).IsFlagged)
                            symb = "! ";
                        else if (GetCoordinate(x, y).IsOpen)
                        {
                            if (GetCoordinate(x, y).HasMine)
                                symb = "X ";
                            else if (GetCoordinate(x, y).NrOfNeighbours == 0)
                                symb = ". ";
                            else
                                symb = GetCoordinate(x, y).NrOfNeighbours + " ";
                        }
                        else
                            symb = "? ";
                        Bus.Write(symb);
                    }
                }
                Bus.WriteLine();
            }
        }

        #region MoveCursor Methods

        public void MoveCursorUp()
        {
            if (PosY > 0)
                PosY--;
        }

        public void MoveCursorDown()
        {
            if (PosY < SizeY - 1)
                PosY++;
        }

        public void MoveCursorLeft()
        {
            if (PosX > 0)
                PosX--;
        }

        public void MoveCursorRight()
        {
            if (PosX < SizeX - 1)
                PosX++;
        }

        #endregion

        private void FloodFill(int x, int y)
        {

            if ((x > PosInfo.GetLength(0) - 1) || (x < 0))
                return;


            if ((y > PosInfo.GetLength(1) - 1) || (y < 0))
                return;


            if (PosInfo[x, y].IsOpen || PosInfo[x, y].HasMine || PosInfo[x, y].IsFlagged)
                return;
            else if (PosInfo[x, y].NrOfNeighbours == 0)
            {
                PosInfo[x, y].IsOpen = true;


                FloodFill(x + 1, y);

                FloodFill(x - 1, y);

                FloodFill(x, y - 1);

                FloodFill(x, y + 1);


                return;
            }
            else
            {
                PosInfo[x, y].IsOpen = true;
                return;
            }
        }
    }
}
