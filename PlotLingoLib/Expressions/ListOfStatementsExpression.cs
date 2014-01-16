using PlotLingoLib.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// Hodl onto a list of statements, and return the value of the last statement
    /// we've run.
    /// </summary>
    class ListOfStatementsExpression : IExpression
    {
        /// <summary>
        /// Track the statements that we are going to be looking at.
        /// </summary>
        public IStatement[] Statements { get; private set; }

        /// <summary>
        /// Initialize with our statement list.
        /// </summary>
        /// <param name="statements">Initial statement list, flattened into an array when we use it.</param>
        public ListOfStatementsExpression(IEnumerable<IStatement> statements)
        {
            // TODO: Complete member initialization
            this.Statements = statements.ToArray();
        }

        /// <summary>
        /// Execute the expression list.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            throw new NotImplementedException();
        }
    }
}
