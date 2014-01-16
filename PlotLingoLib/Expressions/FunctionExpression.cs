using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
        public string FunctionName { get; set; }

        /// <summary>
        /// A list of arguments
        /// </summary>
        public IExpression[] Arguments { get; set; }

        /// <summary>
        /// Initialize a function expression.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="args"></param>
        public FunctionExpression(string fname, params IExpression[] args)
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
        public object Evaluate(IScopeContext c)
        {
            // Force evaluation of all arguments.
            var args = Arguments.Select(a => a.Evaluate(c)).ToArray();

            var funcs = FindFunction(c, ref args);

            // Now call the method.
            try
            {
                var r = funcs.Invoke(null, args);

                // Deal with post-hook call backs now
                r = c.ExecutionContext.ExecutePostCallHook(FunctionName, null, r);
                return r;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        /// <summary>
        /// Info about what we can do for each operand
        /// </summary>
        private struct OpInfo
        {
            public bool ReverseOperandsOK;
            public string MethodName;
        }

        /// <summary>
        /// Specifications for the binary operators we know about.
        /// </summary>
        private static Dictionary<string, OpInfo> _binaryOperators = new Dictionary<string, OpInfo>()
        {
            {"+", new OpInfo(){ MethodName = "OperatorPlus", ReverseOperandsOK = true}},
            {"*", new OpInfo(){ MethodName = "OperatorMultiply", ReverseOperandsOK = true}},
            {"-", new OpInfo(){ MethodName = "OperatorMinus", ReverseOperandsOK = false}},
            {"/", new OpInfo(){ MethodName = "OperatorDivide", ReverseOperandsOK = false}},
        };

        /// <summary>
        /// Find the proper function to call. Throw if we can't find it, or if we find more than one that matches.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="args"></param>
        /// <returns>The funciton invokation object</returns>
        /// <remarks>
        /// Normal Function Call:
        ///   - Search to see if we can't find the proper function
        ///   - If that fails, then try adding the Context onto the beginning of it
        ///   
        /// Operator:
        ///   - Replace with name.
        ///   - Search using same stradegy above
        ///   - If that fails, and the operator operand doesn't matter (e.g. + or -) reverse the arguments and repeat above.
        /// </remarks>
        private MethodInfo FindFunction(IScopeContext c, ref object[] args)
        {
            // Try to find some functions that make some sense.
            MethodInfo[] funcs = null;
            if (_binaryOperators.Keys.Contains(FunctionName))
            {
                var op = _binaryOperators[FunctionName];
                funcs = FindFunctionWithMaybeContextArg(op.MethodName, c, ref args);
                if (funcs.Length == 0 && op.ReverseOperandsOK)
                {
                    var argsR = args.Reverse().ToArray();
                    funcs = FindFunctionWithMaybeContextArg(op.MethodName, c, ref argsR);
                    if (funcs.Length > 0)
                        args = argsR;
                }
            }
            else
            {
                funcs = FindFunctionWithMaybeContextArg(FunctionName, c, ref args);
            }

            // See if we have reasonable results, and throw if not.
            if (funcs.Length == 0)
                throw new System.NotImplementedException(string.Format("Unknown function '{0}' referenced! {0}", FunctionName));

            if (funcs.Length > 1)
            {
                StringBuilder bld = new StringBuilder();
                foreach (var item in funcs)
                {
                    bld.AppendFormat("{0}.{1}", item.DeclaringType.Name, item.Name);
                }
                throw new System.NotImplementedException(string.Format("Function '{0}' referenced - but has more than one possible resolution in types {1}", FunctionName, bld.ToString()));
            }

            // And off to the races we go!
            return funcs[0];
        }

        /// <summary>
        /// Find the list of functions that match these arguments, perhaps with
        /// a Context argument pre-pended.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static MethodInfo[] FindFunctionWithMaybeContextArg(string fname, IScopeContext c, ref object[] args)
        {
            fname = fname.FixUpReserved();

            // All functions that look like they might be right. Fail if we don't find them or find too many.
            var funcs = FindFunctionFromFunctionObjects(fname, args);

            // If we couldn't find it, see if it is a special function - so put Context at the start.
            if (funcs.Length == 0)
            {
                var argsExtended = new object[] { c }.Concat(args).ToArray();
                funcs = FindFunctionFromFunctionObjects(fname, argsExtended);
                if (funcs.Length > 0)
                    args = argsExtended;
            }
            return funcs;
        }

        /// <summary>
        /// Search through all extensibility points for the function to call.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static MethodInfo[] FindFunctionFromFunctionObjects(string fname, object[] args)
        {
            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                         let m = fo.GetType().GetMethod(fname, args.Select(v => v.GetType()).ToArray())
                         where m != null
                         where m.IsStatic
                         select m).ToArray();
            return funcs;
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
