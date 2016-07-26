using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Look for the various IFunctionObject's for extensibility and defined functions.
    /// </summary>
    [Export(typeof(IFunctionFinder))]
    class ObjectFunctionCaller : IFunctionFinder
    {
        /// <summary>
        /// Returns method that will call the function object.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Arguments"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public Func<object> FindFunction(IScopeContext c, IEnumerable<IExpression> Arguments, string fname)
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

            var argtypelistItr = FunctionUtils.ZipArgs(args, parameterInfo)
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
                });

            // It is possible for some IExpressions to fail when we try to evaluate them on other
            // sets of input.
            try
            {
                var argtypelist = argtypelistItr.ToArray();

                // If any null's got through, then we are dead
                if (argtypelist.Where(a => a == null).Any())
                    return false;

                // Make sure all casts are taking into account using this squirly work around.
                var m = method.DeclaringType.GetMethod(method.Name, argtypelist);
                return m != null && m == method;
            }
            catch
            {
                return false;
            }
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


    }
}
