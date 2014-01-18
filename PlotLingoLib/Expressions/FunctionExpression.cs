using System;
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
            Func<object> funcCaller;
            if (_binaryOperators.ContainsKey(FunctionName))
            {
                funcCaller = FindBinaryFunction(c, Arguments, _binaryOperators[FunctionName]);
            }
            else
            {
                funcCaller = FindFunction(c, Arguments, FunctionName.FixUpReserved());
            }

            if (funcCaller == null)
                throw new NotImplementedException(string.Format("Unable to find function {0}", FunctionName));

#if false
            var args = Arguments.Select(a => a.Evaluate(c)).ToArray();

            var funcs = FindFunction(c, ref args);
#endif
            // Now call the method.
            try
            {
                var r = funcCaller(); // s.Invoke(null, args);

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
        /// We are dealing with a binary function of some sort - like +, -, etc.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Arguments"></param>
        /// <param name="opInfo"></param>
        /// <returns></returns>
        private Func<object> FindBinaryFunction(IScopeContext c, IExpression[] Arguments, OpInfo opInfo)
        {
            if (Arguments.Length != 2)
                throw new InvalidOperationException(string.Format("Binary operation {0} needs exactly two arguments", opInfo.MethodName));

            var funcCaller = FindFunction(c, Arguments, opInfo.MethodName);
            if (funcCaller != null || !opInfo.ReverseOperandsOK)
                return funcCaller;

            return FindFunction(c, Arguments.Reverse(), opInfo.MethodName);
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
        /// Returns method that will call the function object.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Arguments"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        private static Func<object> FindFunction(IScopeContext c, IEnumerable<IExpression> Arguments, string fname)
        {
            // Get a list of all all functions that match the name we are using
            var allFuncs = ListOfFuncsWithName(fname);

            // Next, sort them by number of IExpression arguments. The more there are, the more we want to
            // try first. This isn't perfect, but it does establish a very definite precedence order.
            var byExpressionArgsUS = from m in allFuncs
                                     group m by m.GetParameters().Where(p => p.ParameterType.Name == "IExpression").Count();
            var byExpressionArgs = from g in byExpressionArgsUS
                                   orderby g.Key descending
                                   select g;

            // Now, run down them and see which ones match.
            var args = Arguments.Select(a => new FunctionUtils.ArgEvalHolder(c, a)).ToArray();
            var method = (from mgroup in byExpressionArgs
                          from m in mgroup
                          where MatchArgList(m, args)
                          select m).FirstOrDefault();

            if (method == null)
                return null;

            return () => method.Invoke(null, ArgList(c, args, method.GetParameters()).ToArray());
        }

        /// <summary>
        /// Return the argument list to use when calling a method of this parameter set. Assume that
        /// this is going to work.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        /// <remarks>Undefined if the argument list doesn't match!</remarks>
        private static IEnumerable<object> ArgList(IScopeContext c, IEnumerable<FunctionUtils.ArgEvalHolder> args, IEnumerable<ParameterInfo> ps)
        {
            return FunctionUtils.ZipArgs(args, ps)
                .Select(pair => TranslateToParameter(c, pair));
        }

        /// <summary>
        /// Given the parameter and argument, return the object that should actually be handed to the method that we are calling.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        private static object TranslateToParameter(IScopeContext c, FunctionUtils.ArgPairing pair)
        {
            if (typeof(IScopeContext).IsAssignableFrom(pair._parameter.ParameterType))
                return c;
            if (typeof(IExpression).IsAssignableFrom(pair._parameter.ParameterType)
                        && pair._parameter.ParameterType.IsAssignableFrom(pair._arg.Expression.GetType()))
                return pair._arg.Expression;

            return pair._arg.Value;
        }

        /// <summary>
        /// Does this parameter list match with the argument list we were given?
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>The frameowrk is tricking - and method invokations can use cast's as well as
        /// type conversions. So in other than special cases, we end up having to use the
        /// framework's reflection to see if we can get this right.</remarks>
        private static bool MatchArgList(MethodInfo method, FunctionUtils.ArgEvalHolder[] args)
        {
            var parameterInfo = method.GetParameters();

            var argtypelist = FunctionUtils.ZipArgs(args, parameterInfo)
                .Select(arg =>
                {
                    // A reference to the execution context anywhere is also legal.
                    if (typeof(IScopeContext).IsAssignableFrom(arg._parameter.ParameterType))
                        return typeof(IScopeContext);

                    // If we are here we can't generate an argument. So there had better
                    // be one if we are to have a match.
                    if (arg._arg == null)
                        return (Type)null;

                    // If the parameter is an IExpression, then we by default have it as all arguments
                    // come in that way. Note the reverse logic here for the is assignable from... we want to treat
                    // any arg using IExpression specially...
                    if (typeof(IExpression).IsAssignableFrom(arg._parameter.ParameterType)
                        && arg._parameter.ParameterType.IsAssignableFrom(arg._arg.Expression.GetType()))
                        return arg._parameter.ParameterType;

                    // If we got here, then we will let the framework figure out the last. We have to do this
                    // because there are some implicit cast operations we can't automatically detect with
                    // IsAssignableFrom.
                    return arg._arg.Type;
                })
                .ToArray();

            // If any null's got through, then we are dead
            if (argtypelist.Where(a => a == null).Any())
                return false;

            // Make sure all casts are taking into account using this squirly work around.
            var m = method.DeclaringType.GetMethod(method.Name, argtypelist);
            return m != null && m == method;
        }

        /// <summary>
        /// Returns a list of all functions we can find that have the proper name.
        /// </summary>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        private static IEnumerable<MethodInfo> ListOfFuncsWithName(string fname)
        {
            var funcs = (from fo in ExtensibilityControl.Get().FunctionObjects
                         from m in fo.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static)
                         where m.Name == fname
                         select m).ToArray();
            return funcs;
        }

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
