using System.Linq.Expressions;

namespace R8.Lib
{
    public class ExpressionArgument
    {
        public ExpressionArgument(string name, LambdaExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        public ExpressionArgument()
        {
        }

        public void Deconstruct(out string name, out LambdaExpression expression)
        {
            name = Name;
            expression = Expression;
        }

        public string Name { get; set; }
        public LambdaExpression Expression { get; set; }

        public override string ToString()
        {
            return $".{Name}({Expression})";
        }
    }
}