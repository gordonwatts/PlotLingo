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

            // Now, see if we can't get the method name from there.
            var method = t.GetMethod(expr.FunctionCall.FunctionName);
            if (method == null)
                return new Tuple<bool, object>(false, null);

            // And invoke the method and see what we can get back.

            var r = method.Invoke(obj, new object[] { });
            return new Tuple<bool, object>(true, r);
        }
    }
}
