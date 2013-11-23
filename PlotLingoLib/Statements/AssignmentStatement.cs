using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Statements
{
    /// <summary>
    /// Assignment. This will create or replace a current item.
    /// </summary>
    internal class AssignmentStatement : IStatement
    {
        private string _variable;
        private IExpression _expression;

        /// <summary>
        /// Initialize an assignment statement
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="expr"></param>
        public AssignmentStatement(string nv, IExpression expr)
        {
            this._variable = nv;
            this._expression = expr;
        }

        /// <summary>
        /// Assign the variable to the value.
        /// </summary>
        /// <param name="c"></param>
        public void Evaluate(Context c)
        {
            c.SetVariableValue(_variable, _expression.Evaluate(c));
        }
    }
}
