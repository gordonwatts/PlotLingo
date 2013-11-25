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
        public Tuple<bool, object> Evaluate(Context c, object obj, string methodName, object[] args)
        {
            // Get the type of the object
            var t = obj.GetType();
            
            // Now, see if we can't get the method name from there.
            var method = t.GetMethod(methodName, args.Select(v => v.GetType()).ToArray());
            if (method == null)
                return new Tuple<bool, object>(false, null);

            // And invoke the method and see what we can get back.
            var r = method.Invoke(obj, args);
            return new Tuple<bool, object>(true, r);
        }
    }
}
