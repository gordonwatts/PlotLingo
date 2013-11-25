using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.MethodEvaluators
{
    /// <summary>
    /// Anything that can evaluate a method call should impelment this
    /// </summary>
    interface IMethodEvaluator
    {
        /// <summary>
        /// Evaluate the expression. Return true if we successfully did
        /// the evaluation.
        /// </summary>
        /// <param name="args">Evaluated arguments to the method</param>
        /// <param name="c">Context for evaluation</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="obj">The object against which to call</param>
        /// <returns>Value with true if the call succeeded, and then also the result</returns>
        Tuple<bool, object> Evaluate(Context c, object obj, string methodName, object[] args);
    }
}
