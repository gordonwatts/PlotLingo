using PlotLingoLib.MethodEvaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlotLingoLib;
using ROOTNET.Globals;
using System.ComponentModel.Composition;
using PlotLingoLib.Functions;
using PlotLingoLib.Expressions;
using ROOTNET;

namespace PlotLingoFunctionality.ROOT
{
    /// <summary>
    /// Allow calling of a ROOT function. This is a function in the global namespace.
    /// </summary>
    [Export(typeof(IFunctionFinder))]
    class ROOTFunction : IFunctionFinder
    {
        /// <summary>
        /// Look through all the ROOT macros and see if we can't find a proper function and call it.
        /// Generate a function object that will do the job.
        /// Allows user to access macros in a file.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Arguments"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
        public Func<object> FindFunction(IScopeContext c, IEnumerable<IExpression> Arguments, string methodName)
        {
            NTInterpreter.Instance().UpdateListOfGlobalFunctions();
            var func = gROOT.Value.ListOfGlobalFunctions
                .Where(lf => lf.Name == methodName)
                .Cast<NTFunction>()
                .ToArray();

            if (func.Length == 0)
            {
                return null;
            }

            // We aren't going to do arguments yet...
            var f = func.Where(lf => lf.ListOfMethodArgs.Size == Arguments.Count()).FirstOrDefault();
            if (f == null)
            {
                throw new ArgumentException($"Unfortunately, the ROOT global function '{methodName}' requires a number other than {Arguments.Count()} arguments. I don't know how to make them match!");
            }

            // Put the arguments into string array that we can send to the execute method.
            var argList = Arguments
                .Select(a => a.Evaluate(c))
                .Select(a => ToProtectedString(a));

            var args = new StringBuilder();
            foreach (var a in argList)
            {
                if (args.Length > 0)
                {
                    args.Append(",");
                }
                args.Append(a);
            }

            // Next, we can execute it.
            return () =>
            {
                NTInterpreter.Instance().Execute(methodName, args.ToString());
                return null;
            };
        }

        /// <summary>
        /// Add quotes where we need it as we pass things to CINT.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string ToProtectedString (object o)
        {
            if (o is string) {
                return $"\"{o.ToString()}\"";
            }
            return o.ToString();
        }
    }
}
