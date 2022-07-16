namespace XOX.BLObjects
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
        public string Value = string.Empty;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
