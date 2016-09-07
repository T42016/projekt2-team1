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

        public PositionInfo GetCoordinate(int x, int y)
        {
            
            return null;


        }

        public void FlagCoordinate()
        {
        }

        public void ClickCoordinate()
        {
            
        }

        public void ResetBoard()
        {

        }

        public void DrawBoard()
        {
            
            for (int i = 0; i < SizeY; i++)
            {
                for (int j = 0; j < SizeX; j++)
                {


                    if (j == PosX && i == PosY)
                    {
                        Bus.Write("? ", ConsoleColor.DarkCyan);

                    }
                    else
                    {
                        Bus.Write("? ");
                    }
                }

                Console.WriteLine();
            }

            Bus.Write("! "); 
        }

        #region MoveCursor Methods

        public void MoveCursorUp()
        {
            if (PosY > 0)
            {
                PosY--;
            }
        }

        public void MoveCursorDown()
        {
            if(PosY < SizeY -1)
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
            if(PosX +1  < SizeX)
                PosX++;
        }

        #endregion

        #region CorrectPos
        #endregion
    }
}
