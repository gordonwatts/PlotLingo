using PlotLingoLib.Expressions;

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
        public void Evaluate(Context c)
        {
            var r = Expression.Evaluate(c);
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
