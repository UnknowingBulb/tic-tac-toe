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
        public List<Cell> Cells;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="size">Количество ячеек на поле по высоте/ширине</param>
        public Field(int size = 3)
        {
            Cells = new List<Cell>();
            for (int x=0; x<size; x++)
            {
                for (int y = 0; y< size; y++)
                {
                    Cells.Add(new Cell(x, y));
                }
            }
        }
    }
}
