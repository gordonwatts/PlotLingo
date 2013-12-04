using System;
using System.Linq;
using System.Reflection;
using System.Text;

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
        /// <returns></returns>
        public Tuple<bool, object> Evaluate(Context c, object obj, string methodName, object[] arguments)
        {
            var args = new object[] { obj }.Concat(arguments).ToArray();

            // All functions that look like they might be right. Fail if we find too many, but if we can't find one, then
            // that means we should let some other method binder attempt.
            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                         let m = fo.GetType().GetMethod(methodName, args.Select(v => v.GetType()).ToArray())
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
                throw new System.NotImplementedException(string.Format("Method '{0}' referenced - but has more than one possible resolution in types {1}", methodName, bld.ToString()));
            }

            // Now call the method.
            try
            {
                var r = funcs[0].Invoke(null, args);
                return new Tuple<bool, object>(true, r);
            }
            catch (TargetInvocationException e)
            {
                // We don't care above the invokation - just what actually threw it inside.
                throw e.InnerException;
            }
        }
    }
}
