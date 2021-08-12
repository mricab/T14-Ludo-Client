using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoMatch
{
    public class Board // V1.5
    {
        public int size;
        public string[] cells;
        public int[] restPositions;
        public int[] startPositions;
        public List<int> road;

        public Board(string[] data)
        {
            int.TryParse(data[0], out this.size);
            cells = new string[data.Length - 1];
            Array.Copy(data, 1, cells, 0, data.Length - 1);
            findRestPositions();
            findStartPositions();
            findRoad();
        }

        private void findRestPositions()
        {
            restPositions = new int[16]; //0-3 -> p1, 4-7 -> p2, 8-11 -> p3, 12-15 -> p4
            int p1 = 0; int p2 = 4; int p3 = 8; int p4 = 12;
            for (int i = 0; i < cells.Length; i++)
            {
                int cell = int.Parse(cells[i]);
                if (cell >= 11 && cell <= 14)
                {
                    switch (cell)
                    {
                        case 11: restPositions[p1] = i; p1++; break;    // Red      p1
                        case 12: restPositions[p2] = i; p2++; break;    // Yellow   p2
                        case 13: restPositions[p3] = i; p3++; break;    // Green    p3
                        case 14: restPositions[p4] = i; p4++; break;    // Blue     p4
                        default: break;
                    }
                }
            }
        }

        public int[] getRestPositions(int players)
        {
            int[] data = new int[players * 4];
            Array.Copy(restPositions, 0, data, 0, players * 4);
            return data;
        }

        private void findStartPositions()
        {
            startPositions = new int[4]; //0 -> p1, 1 -> p2, 2 -> p3, 3 -> p4
            for (int i = 0; i < cells.Length; i++)
            {
                int cell = int.Parse(cells[i]);
                if (cell >= 21 && cell <= 24)
                {
                    switch (cell)
                    {
                        case 21: startPositions[0] = i; break;    // Red      p1
                        case 22: startPositions[1] = i; break;    // Yellow   p2
                        case 23: startPositions[2] = i; break;    // Green    p3
                        case 24: startPositions[3] = i; break;    // Blue     p4
                        default: break;
                    }
                }
            }
        }

        private void findRoad()
        {
            // To do
        }

        public string[] BoardData()
        {
            string[] data = new string[1 + cells.Length];
            data[0] = size.ToString();
            Array.Copy(cells, 0, data, 1, cells.Length);
            return data;
        }
    }
}
