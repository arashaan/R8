using System;

namespace R8.Lib.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OrderAttribute : Attribute
    {
        public int X { get; set; }

        public OrderAttribute(int x)
        {
            X = x;
        }
    }
}