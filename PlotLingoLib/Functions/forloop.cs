using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Implement for loop constructs
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class ForLoop : IFunctionObject
    {
        /// <summary>
        /// A regular for loop, using a list of dictionaries as the items we loop over. For each iteration, a dictionary is
        /// pulled off the loopControl sequence. The keys are all evaluated to strings, and then used as variable names that
        /// can be referenced in the body of the loop. They are set to the value of the dictionary value.
        /// </summary>
        /// <param name="ctx">Scope context for variable definitions, etc.</param>
        /// <param name="loopControl">A list of dictionaries that we used to set the loop variables</param>
        /// <param name="statements">The list of statements we will process</param>
        /// <returns></returns>
        /// <remarks>Becuase "for" is a reserved word, this function needs the "Reserved" tacked onto the end. During method
        /// resolution, the language core should take care of this.</remarks>
        public static object forReserved(IScopeContext ctx, IEnumerable<object> loopControl, ListOfStatementsExpression statements)
        {
            object result = null;

            foreach (var iterLoopVars in loopControl)
            {
                var dict = iterLoopVars as IDictionary<object, object>;
                if (dict == null)
                    throw new ArgumentException("For loop over dictionary items - every item must be a dictionary!");

                var newScope = new ScopeContext(ctx);
                foreach (var varDefined in dict)
                {
                    newScope.SetVariableValueLocally(varDefined.Key.ToString(), varDefined.Value);
                }

                result = statements.Evaluate(newScope);
            }

            return result;
        }

        /// <summary>
        /// Loop over an array with a variable index.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="loopControl"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static object forReserved(IScopeContext ctx, VariableValue loopIndex, IEnumerable<object> loopControl, ListOfStatementsExpression statements)
        {
            var indexList = loopControl.Select(i => new Dictionary<object, object>() { { loopIndex.VariableName, i } });
            return forReserved(ctx, indexList, statements);
        }
    }
}
