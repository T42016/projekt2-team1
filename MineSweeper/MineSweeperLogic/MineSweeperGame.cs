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
            State = State;
            Bus = bus;
        }

        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public int SizeX { get; }
        public int SizeY { get; }
        public int NumberOfMines { get; }
        public GameState State { get; private set; }
        public IServiceBus Bus { get; }
        private string symb;

        public PositionInfo PosInfo = new PositionInfo();

        public PositionInfo GetCoordinate(int x, int y)
        {
            if (x < 0 || y < 0) // Check for negative values
                throw new IndexOutOfRangeException();

            PosInfo.X = x;
            PosInfo.Y = y;

            if (PosInfo.Y >= SizeY)
                throw new IndexOutOfRangeException();

            if (PosInfo.X >= SizeX)
                throw new IndexOutOfRangeException();


            return PosInfo;
        }

        public void FlagCoordinate()
        {

            if (!PosInfo.IsOpen)
            {
                if (!PosInfo.IsFlagged)
                    PosInfo.IsFlagged = true;
                else
                    PosInfo.IsFlagged = false;
            }



        }

        public void ClickCoordinate()
        {
            if (!PosInfo.IsOpen)
            {
                if (PosInfo.HasMine)
                {
                    State = GameState.Lost;

                }
                else
                {
                    PosInfo.IsOpen = true;

                }
            }

        }

        public void ResetBoard()
        {
            PosInfo = new PositionInfo();


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

        #region CorrectPos

        #endregion
    }
}
