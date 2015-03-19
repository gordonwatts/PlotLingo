using System;
using System.Linq;

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
        public Tuple<bool, object> Evaluate(IScopeContext c, object obj, string methodName, object[] args)
        {
            // Get the type of the object
            var t = obj.GetType();
            var argListTypes = args.Select(a => a.GetType()).ToArray();

            var funcs = (from m in t.GetMethods()
                         where m.Name == methodName
                         let newm = m.ArgumentListMatches(argListTypes, t)
                         where newm != null
                         select newm).ToArray();

            if (funcs.Length != 1)
                return new Tuple<bool, object>(false, null);

            // And invoke the method and see what we can get back.
            // Optional arguments means we have to fill things in.
            var method = funcs[0];
            object[] allArgs = args;
            if (method.GetParameters().Length != args.Length)
            {
                var defaultArgs = method.GetParameters().Skip(args.Length)
                    .Select(a => a.HasDefaultValue ? a.DefaultValue : null);
                allArgs = args.Concat(defaultArgs).ToArray();
            }
            var r = funcs[0].Invoke(obj, allArgs);
            return new Tuple<bool, object>(true, r);
        }
    }
}
