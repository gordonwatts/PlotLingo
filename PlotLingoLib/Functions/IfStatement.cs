using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Implement an inline if statemnet (a "?" operator written out the long way).
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class IfStatement : IFunctionObject
    {
        /// <summary>
        /// Simple if statement.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="logicalTest"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static object ifReserved(IScopeContext ctx, IExpression logicalTest, ListOfStatementsExpression statements)
        {
            // See if we have fired
            var testResult = logicalTest.Evaluate(ctx);
            if (testResult.GetType() != typeof(bool) && testResult.GetType() != typeof(int))
            {
                throw new ArgumentException($"The test {logicalTest.ToString()} did not evaluate to an integer or a boolean");
            }

            var shouldExecute = testResult.GetType() == typeof(bool)
                ? (bool)testResult
                : (int)testResult != 0;
            
            if (shouldExecute)
            {
                var newScope = new ScopeContext(ctx);
                return statements.Evaluate(newScope);
            }
            return null;
        }
    }
}
