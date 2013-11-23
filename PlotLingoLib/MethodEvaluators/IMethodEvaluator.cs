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
        /// <param name="expr"></param>
        /// <returns></returns>
        Tuple<bool, object> Evaluate(Context c, MethodCallExpression expr);
    }
}
