using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Implement map-like operations
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class Map : IFunctionObject
    {
        /// <summary>
        /// Given a list of objects that are dictionarys, run the loop. Take the last result from each loop, and transform it into a
        /// list of values that gets passed back to the caller.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="loopControl"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static IEnumerable<object> map(IScopeContext ctx, IEnumerable<object> loopControl, ListOfStatementsExpression statements)
        {
            return loopControl.Select(iterLoopVars =>
            {
                var dict = iterLoopVars as IDictionary<object, object>;
                if (dict == null)
                    throw new ArgumentException("For loop over dictionary items - every item must be a dictionary!");

                var newScope = new ScopeContext(ctx);
                foreach (var varDefined in dict)
                {
                    newScope.SetVariableValueLocally(varDefined.Key.ToString(), varDefined.Value);
                }

                var v = statements.Evaluate(newScope);
                if (v == null)
                    throw new ArgumentNullException("Iteration of map returned null!");
                return v;
            }).ToArray();
        }
    }
}
