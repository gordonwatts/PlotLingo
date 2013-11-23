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
        private IExpression _expr;

        public ExpressionStatement(IExpression expr)
        {
            this._expr = expr;
        }

        /// <summary>
        /// We just evaluate the expression.
        /// </summary>
        /// <param name="c"></param>
        public void Evaluate(Context c)
        {
            _expr.Evaluate(c);
        }
    }

}
