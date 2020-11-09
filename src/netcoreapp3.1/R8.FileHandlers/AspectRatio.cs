namespace R8.FileHandlers
{
    public class AspectRatio
    {
        /// <summary>
        /// Initializes an <see cref="AspectRatio"/> instance that representing X and Y
        /// </summary>
        /// <param name="x">An <see cref="int"/> value that representing X</param>
        /// <param name="y">An <see cref="int"/> value that representing Y</param>
        public AspectRatio(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes an <see cref="AspectRatio"/> instance that representing X and Y
        /// </summary>
        public AspectRatio()
        {
        }

        /// <summary>
        /// Gets or sets an <see cref="int"/> value that representing X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="int"/> value that representing Y
        /// </summary>
        public int Y { get; set; }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
}