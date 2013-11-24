using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Statements
{
    /// <summary>
    /// An expression statement. Evalute an expression, and discard the result.
    /// </summary>
    internal class ExpressionStatement : IStatement
    {
        /// <summary>
        /// Expression this statement represents
        /// </summary>
        public IExpression Expression {get; private set;}

        /// <summary>
        /// Initialize an expression statement.
        /// </summary>
        /// <param name="expr"></param>
        public ExpressionStatement(IExpression expr)
        {
            Expression = expr;
        }

        /// <summary>
        /// We just evaluate the expression.
        /// </summary>
        /// <param name="c"></param>
        public void Evaluate(Context c)
        {
            Expression.Evaluate(c);
        }

        /// <summary>
        /// Pretty print for debugging and testing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0};", Expression.ToString());
        }
    }

}
