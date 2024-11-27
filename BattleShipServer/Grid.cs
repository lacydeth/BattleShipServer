using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipServer
{
    public class Grid
    {
        private const int GridSize = 10;
        private readonly bool[,] ships;
        private readonly bool[,] hits;

        public Grid()
        {
            ships = new bool[GridSize, GridSize];
            hits = new bool[GridSize, GridSize];

            // Initialize ships (for example purposes, hard-coded here)
            PlaceShips();
        }

        // Example: Pre-place ships for simplicity
        private void PlaceShips()
        {
            ships[0, 0] = true; ships[0, 1] = true; ships[0, 2] = true; ships[0, 3] = true; ships[0, 4] = true; // Aircraft Carrier
            ships[2, 0] = true; ships[2, 1] = true; ships[2, 2] = true; ships[2, 3] = true; // Battleship
            ships[4, 0] = true; ships[4, 1] = true; ships[4, 2] = true; // Submarine
            ships[6, 0] = true; ships[6, 1] = true; ships[6, 2] = true; // Cruiser
            ships[8, 0] = true; ships[8, 1] = true; // Destroyer
        }

        public bool CheckHit(int x, int y)
        {
            if (ships[x, y])
            {
                hits[x, y] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllShipsSunk()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (ships[i, j] && !hits[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
