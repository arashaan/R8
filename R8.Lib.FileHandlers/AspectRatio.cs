namespace R8.Lib.FileHandlers
{
    public class AspectRatio
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AspectRatio(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AspectRatio()
        {
        }

        public int X { get; set; }
        public int Y { get; set; }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        #region Overrides of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{X}:{Y}";
        }

        #endregion Overrides of Object
    }
}