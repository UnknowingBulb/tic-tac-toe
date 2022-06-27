using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    public class User
    {
        public int Id;
        public string Name;
        public string Mark;

        /// <summary>
        /// Ход этого игрока или нет
        /// </summary>
        public bool Active;
    }
}
