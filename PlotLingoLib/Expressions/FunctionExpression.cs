using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// A function expression. Call a function and return its value.
    /// </summary>
    /// <remarks>This object is also used in method calls to store the method name and arguments</remarks>
    internal class FunctionExpression : IExpression
    {
        /// <summary>
        /// The function name
        /// </summary>
        public string FunctionName {get; set;}

        /// <summary>
        /// A list of arguments
        /// </summary>
        public IExpression[] Arguments { get; set; }

        /// <summary>
        /// Initialize a function expression.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="args"></param>
        public FunctionExpression(string fname, IExpression[] args)
        {
            FunctionName = fname;
            Arguments = args;
        }

        /// <summary>
        /// Evaluate a function.
        /// </summary>
        /// <param name="c">Context to use while doing the execution</param>
        /// <returns>Whatever the function returns</returns>
        /// <remarks>Use the IFunctionObject to decorate any function you want to add</remarks>
        public object Evaluate(Context c)
        {
            // Force evaluation of all arguments.
            var args = Arguments.Select(a => a.Evaluate(c)).ToArray();

            // All functions that look like they might be right. Fail if we don't find them or find too many.
            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                        let m = fo.GetType().GetMethod(FunctionName, args.Select(v => v.GetType()).ToArray())
                        where m != null
                        where m.IsStatic
                        select m).ToArray();

            if (funcs.Length == 0)
                throw new System.NotImplementedException(string.Format("Unknown function '{0}' referenced!", FunctionName));

            if (funcs.Length > 1)
            {
                StringBuilder bld = new StringBuilder();
                foreach (var item in funcs)
                {
                    bld.AppendFormat("{0}.{1}", item.DeclaringType.Name, item.Name);
                }
                throw new System.NotImplementedException(string.Format("Function '{0}' referenced - but has more than one possible resolution in types {1}", FunctionName, bld.ToString()));
            } 

            // Now call the method.
            var r = funcs[0].Invoke(null, args);

            // Deal with post-hook call backs now
            r = c.ExecutePostCallHook(FunctionName, null, r);
            return r;
        }

        /// <summary>
        /// Pretty print for debugging and testing.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}(", FunctionName);
            bool first = true;
            foreach (var v in Arguments)
            {
                if (!first)
                    sb.Append(",");
                first = false;
                sb.Append(v.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
