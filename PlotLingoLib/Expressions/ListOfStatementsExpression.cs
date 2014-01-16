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
    internal class ListOfStatementsExpression : IExpression
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
            if (statements == null)
                throw new ArgumentNullException("Statements must not be null");

            Statements = statements.ToArray();
        }

        /// <summary>
        /// Execute the expression list.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            var newcontext = new ScopeContext(c);
            object result = null;
            Action<object> saver = a => { result = a; };
            newcontext.AddExpressionStatementEvaluationCallback(saver);
            try
            {
                foreach (var s in Statements)
                {
                    s.Evaluate(newcontext);
                }

                return result;
            }
            finally
            {
                c.RemoveExpressionStatementEvaluationCallback(saver);
            }
        }
    }
}
