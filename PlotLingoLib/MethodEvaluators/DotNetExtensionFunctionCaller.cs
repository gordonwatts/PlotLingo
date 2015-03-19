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
        public Tuple<bool, object> Evaluate(IScopeContext c, object obj, string methodName, object[] arguments)
        {
            var args = new object[] { obj }.Concat(arguments).ToArray();

            // All functions that look like they might be right. Fail if we find too many, but if we can't find one, then
            // that means we should let some other method binder attempt. Also, look for the context being passed first.
            var funcs = FindMethodForArgs(methodName, args);

            if (funcs.Length == 0)
            {
                var args1 = new object[] { c }.Concat(args).ToArray();
                funcs = FindMethodForArgs(methodName, args1);
                if (funcs.Length != 0)
                    args = args1;
            }

            if (funcs.Length == 0)
                return new Tuple<bool, object>(false, null);

            if (funcs.Length > 1)
            {
                StringBuilder bld = new StringBuilder();
                foreach (var item in funcs)
                {
                    bld.AppendFormat("{0}.{1}", item.DeclaringType.Name, item.Name);
                }
                throw new System.NotImplementedException(string.Format("Extension Method '{0}' referenced - but has more than one possible resolution in types {1}", methodName, bld.ToString()));
            }

            // Now call the method.
            var method = funcs[0];
            object[] allArgs = args;
            if (method.GetParameters().Length != args.Length)
            {
                var defaultArgs = method.GetParameters().Skip(args.Length)
                    .Select(a => a.HasDefaultValue ? a.DefaultValue : null);
                allArgs = args.Concat(defaultArgs).ToArray();
            }
            try
            {
                var r = method.Invoke(null, allArgs);
                return new Tuple<bool, object>(true, r);
            }
            catch (TargetInvocationException e)
            {
                // We don't care above the invokation - just what actually threw it inside.
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Find a method in the code, Account for use of C# reserved words by added "Reserved" onto the end of the method
        /// we are trying to find.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>Default arguments make this a little more difficult to deal with</remarks>
        private static MethodInfo[] FindMethodForArgs(string methodName, object[] args)
        {
            // Look for all functions that match the name we want.
            var argListTypes = args.Select(a => a.GetType()).ToArray();
            string methodNameFixed = methodName.FixUpReserved();

            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                         from m in fo.GetType().GetMethods()
                         where m.Name == methodNameFixed
                         where m.IsStatic
                         let newm = m.ArgumentListMatches(argListTypes, fo.GetType())
                         where newm != null
                         select newm).Distinct().ToArray();

            return funcs;
        }
    }
}
