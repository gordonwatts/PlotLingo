using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.MethodEvaluators
{
    /// <summary>
    /// Try to evaluate the method on a .net object.
    /// </summary>
    class DotNetMethodCall : IMethodEvaluator
    {
        /// <summary>
        /// Do the evaluation.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Tuple<bool, object> Evaluate(Context c, MethodCallExpression expr)
        {
            // Make sure we can get the object.
            var obj = c.GetVariableValue(expr.Object);
            var t = obj.GetType();
            
            // For a function call, all arguments are evaluated ahead of time.
            var args = expr.FunctionCall.Arguments.Select(a => a.Evaluate(c)).ToArray();

            // Now, see if we can't get the method name from there.
            var method = t.GetMethod(expr.FunctionCall.FunctionName, args.Select(v => v.GetType()).ToArray());
            if (method == null)
                return new Tuple<bool, object>(false, null);

            // And invoke the method and see what we can get back.
            var r = method.Invoke(obj, args);
            return new Tuple<bool, object>(true, r);
        }
    }
}
