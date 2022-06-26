﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    /// <summary>
    /// Отдельная ячейка поля
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Координата x
        /// </summary>
        public int x;

        /// <summary>
        /// Координата y
        /// </summary>
        public int y;

        /// <summary>
        /// Чем заполнена, если заполнена
        /// </summary>
        public string value = string.Empty;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
