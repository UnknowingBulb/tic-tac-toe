using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    /// <summary>
    /// Класс с полем
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Ячейки на поле
        /// </summary>
        public Cell[,] Cells;

        private int size;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="size">Количество ячеек на поле по высоте/ширине</param>
        public Field(int size = 3)
        {
            this.size = size;
            Cells = new Cell[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Cells[x, y] = new Cell(x, y);
                }
            }
        }

        public bool IsGameCompleted()
        {
            bool completed = false;
            //TODO: check for more optimal ways. I purpously did not searched on start for good solution
            //Check for horizontal lines
            for (int x = 0; x < size; x++)
            {
                completed = true;
                for (int y = 0; y < size - 1; y++)
                {
                    if (Cells[x, y].Value != Cells[x, y + 1].Value 
                        || Cells[x, y + 1].Value == string.Empty || Cells[x, y].Value == string.Empty)
                    {
                        completed = false;
                        break;
                    }
                }
            }
            if (completed == true)
                return completed;

            //Check for vertical lines
            for (int y = 0; y < size; y++)
            {
                completed = true;
                for (int x = 0; x < size - 1; x++)
                {
                    if (Cells[x, y].Value != Cells[x + 1, y].Value
                        || Cells[x, y].Value == string.Empty || Cells[x + 1, y].Value == string.Empty)
                    {
                        completed = false;
                        break;
                    }
                }
            }
            if (completed == true)
                return completed;

            completed = true;
            //Check for diagonals. For this game I take strictrly end-to-end diagonal
            for (int x = 0; x < size - 1; x++)
            {
                if (!((Cells[x, x].Value == Cells[x + 1, x + 1].Value) || 
                    (Cells[x , size - x - 1].Value == Cells[x + 1, size - x - 2].Value))
                        || Cells[x, x].Value == string.Empty || Cells[x, size - x - 1].Value == string.Empty)
                {
                    completed = false;
                    break;
                }
            }
            return completed;
        }
    }
}
