using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlotLingoLib.MethodEvaluators
{
    /// <summary>
    /// Try to evaluate the method on a .net object.
    /// </summary>
    [Export(typeof(IMethodEvaluator))]
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
                         select newm).Distinct().ToArray();

            if (funcs.Length == 0)
                return new Tuple<bool, object>(false, null);

            var method = funcs[0];
            if (funcs.Length > 1)
            {
                StringBuilder bld = new StringBuilder();
                foreach (var item in funcs)
                {
                    bld.AppendFormat("{0}.{1}", item.DeclaringType.Name, item.Name);
                }
                throw new System.NotImplementedException(string.Format("Method '{0}' referenced - but has more than one possible resolution in types {1}", methodName, bld.ToString()));
            }

            // And invoke the method and see what we can get back.
            // Optional arguments means we have to fill things in.
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

        /// <summary>
        /// See if method 1 can be used where method 2 cannot be.
        /// </summary>
        /// <param name="fcombo"></param>
        /// <returns></returns>
        private int ScoreTwoMethods(Tuple<MethodInfo, MethodInfo> fcombo)
        {
            var arg1Types = fcombo.Item1.GetParameters().Select(p => p.ParameterType);
            var arg2Types = fcombo.Item2.GetParameters().Select(p => p.ParameterType);

            var argPairs = arg1Types.Zip(arg2Types, (a1, a2) => Tuple.Create(a1, a2));

            var func1Better = from apair in argPairs
                              where apair.Item1 != apair.Item2
                              where apair.Item2.IsAssignableFrom(apair.Item1)
                              select 1;

            return func1Better.Sum();
        }
    }
}
