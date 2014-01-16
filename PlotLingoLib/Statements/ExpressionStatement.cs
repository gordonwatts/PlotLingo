using PlotLingoLib.Expressions;
using System.Collections.Generic;
using System.Linq;

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
        public IExpression Expression { get; private set; }

        /// <summary>
        /// Initialize an expression statement.
        /// </summary>
        /// <param name="expr"></param>
        public ExpressionStatement(IExpression expr)
        {
            Expression = expr;
        }

        /// <summary>
        /// We just evaluate the expression. Report it to anyone that is interested.
        /// </summary>
        /// <param name="c"></param>
        /// <remarks>Force execution of anythign that is returned as an IEnumerable</remarks>
        public void Evaluate(IScopeContext c)
        {
            var r = Expression.Evaluate(c);
            if (r is IEnumerable<object>)
            {
                r = (r as IEnumerable<object>).ToArray();
            }
            c.ReportExpressionStatementEvaluation(r);
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
