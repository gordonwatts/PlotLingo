﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.MethodEvaluators
{
    /// <summary>
    /// Look through the functions known to us and see if there are
    /// any that could be called like an object method.
    /// </summary>
    class DotNetExtensionFunctionCaller : IMethodEvaluator
    {
        /// <summary>
        /// Attempt to evaluate the expression.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Tuple<bool, object> Evaluate(Context c, Expressions.MethodCallExpression expr)
        {
            // Make sure we can get the object.
            var obj = expr.ObjectExpression.Evaluate(c);

            // For a function call, all arguments are evaluated ahead of time.
            var args = new object[] {obj}.Concat(expr.FunctionCall.Arguments.Select(a => a.Evaluate(c))).ToArray();

            // All functions that look like they might be right. Fail if we find too many, but if we can't find one, then
            // that means we should let some other method binder attempt.
            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                         let m = fo.GetType().GetMethod(expr.FunctionCall.FunctionName, args.Select(v => v.GetType()).ToArray())
                         where m != null
                         where m.IsStatic
                         select m).ToArray();

            if (funcs.Length == 0)
                return new Tuple<bool, object>(false, null);

            if (funcs.Length > 1)
            {
                StringBuilder bld = new StringBuilder();
                foreach (var item in funcs)
                {
                    bld.AppendFormat("{0}.{1}", item.DeclaringType.Name, item.Name);
                }
                throw new System.NotImplementedException(string.Format("Method '{0}' referenced - but has more than one possible resolution in types {1}", expr.FunctionCall.FunctionName, bld.ToString()));
            }

            // Now call the method.
            var r = funcs[0].Invoke(null, args);
            return new Tuple<bool, object>(true, r);
        }
    }
}