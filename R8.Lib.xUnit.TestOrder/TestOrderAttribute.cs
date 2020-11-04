using System;

namespace R8.Lib.xUnit.TestOrder
{
    /// <summary>
    /// Used by CustomOrderer
    /// </summary>
    public class TestOrderAttribute : Attribute
    {
        public int I { get; }

        public TestOrderAttribute(int i)
        {
            I = i;
        }
    }
}