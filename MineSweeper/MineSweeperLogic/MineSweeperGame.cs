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



        }

        public void ClickCoordinate()
        {
            if (!PosInfo[PosX, PosY].IsOpen)
            {
                if (PosInfo[PosX, PosY].HasMine)
                {
                    State = GameState.Lost;

                }
                else
                {
                    PosInfo[PosX, PosY].IsOpen = true;

                }
            }

        }

        public void ResetBoard()
        {
            // Creates epmty PosInfo
            for (int y = 0; y < PosInfo.GetLength(1); y++)
            {
                for (int x = 0; x < PosInfo.GetLength(0); x++)
                {
                    PosInfo[x, y] = new PositionInfo();
                    PosInfo[x, y].X = x;
                    PosInfo[x, y].Y = y;
                    PosInfo[x, y].HasMine = false;
                    PosInfo[x, y].IsFlagged = false;
                    PosInfo[x, y].IsOpen = false;
                    PosInfo[x, y].NrOfNeighbours = 0;
                }
            }

            // Add mines in random PosInfo
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

            // Calculates neighbours
            for (int y = 0; y < PosInfo.GetLength(1); y++)
            {
                for (int x = 0; x < PosInfo.GetLength(0); x++)
                {
                    // Checks if position is in x = 0, y = 0 corner
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
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }
                    else if (PosInfo[x, y].Y == 0)
                    {
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }
                    else if (PosInfo[x, y].Y == (SizeY - 1))
                    {
                        if (PosInfo[x - 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x - 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }
                    else if (PosInfo[x, y].X == 0)
                    {
                        if (PosInfo[x, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y + 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y - 1].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                        if (PosInfo[x + 1, y].HasMine)
                            PosInfo[x, y].NrOfNeighbours++;
                    }
                }
            }
            State = GameState.Playing;
        }

        public void DrawBoard()
        {
            for (int i = 0; i < SizeY; i++)
            {
                for (int j = 0; j < SizeX; j++)
                {
                    if (j == PosX && i == PosY)
                    {
                        if (GetCoordinate(j, i).IsFlagged)
                            symb = "! ";
                        else if (GetCoordinate(j, i).IsOpen)
                        {
                            if (GetCoordinate(j, i).HasMine)
                                symb = "X ";
                            else if (GetCoordinate(j, i).NrOfNeighbours == 0)
                                symb = ". ";
                            else
                                symb = GetCoordinate(j, i).NrOfNeighbours + " ";
                        }
                        else
                            symb = "? ";
                        Bus.Write(symb, ConsoleColor.DarkCyan);
                    }
                    else
                    {
                        if (GetCoordinate(j, i).IsFlagged)
                            symb = "! ";
                        else if (GetCoordinate(j, i).IsOpen)
                        {
                            if (GetCoordinate(j, i).HasMine)
                                symb = "X ";
                            else if (GetCoordinate(j, i).NrOfNeighbours == 0)
                                symb = ". ";
                            else
                                symb = GetCoordinate(j, i).NrOfNeighbours + " ";
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

        public
            void MoveCursorUp()
        {
            if (PosY > 0)
            {
                PosY--;
            }
        }

        public void MoveCursorDown()
        {
            if (PosY < SizeY - 1)
                PosY++;
        }

        public void MoveCursorLeft()
        {
            if (PosX > 0)
            {
                PosX--;
            }
        }

        public void MoveCursorRight()
        {
            if (PosX + 1 < SizeX)
                PosX++;
        }

        #endregion

    }
}
