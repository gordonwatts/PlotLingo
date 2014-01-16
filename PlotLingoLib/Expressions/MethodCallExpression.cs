using PlotLingoLib.MethodEvaluators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// A method call made to some object.
    /// </summary>
    internal class MethodCallExpression : IExpression
    {
        /// <summary>
        /// The target object
        /// </summary>
        public IExpression ObjectExpression { get; set; }

        /// <summary>
        /// The name of the method we are to call along with its arguments.
        /// </summary>
        /// <remarks>We re-use the function expression here as it has everything
        /// we need.</remarks>
        public FunctionExpression FunctionCall { get; set; }

        /// <summary>
        /// Initialize a method call expression.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        public MethodCallExpression(IExpression obj, FunctionExpression func)
        {
            ObjectExpression = obj;
            FunctionCall = func;
        }

        /// <summary>
        /// Default list of method evaluators
        /// </summary>
        private List<IMethodEvaluator> _evaluators = new List<IMethodEvaluator>()
        {
            new DotNetExtensionFunctionCaller(),
            new DotNetMethodCall()
        };

        /// <summary>
        /// Evaluate the method call. Use a list of evaluators to try to accomplish the call.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            // All functions and the source object must evaluate correctly. Further, since we want to do the
            // evaluate only once, we do it here, at the top.
            var obj = ObjectExpression.Evaluate(c);
            var args = FunctionCall.Arguments.Select(a => a.Evaluate(c)).ToArray();

            // Find the first evaluator that can figure out what this is.
            var goodEval = _evaluators.Select(e => e.Evaluate(c, obj, FunctionCall.FunctionName, args)).Where(r => r.Item1).FirstOrDefault();
            if (goodEval != null)
            {
                return c.ExecutionContext.ExecutePostCallHook(FunctionCall.FunctionName, obj, goodEval.Item2);
            }
            throw new InvalidOperationException(string.Format("Don't know how to call the function {0} on the object {1} of type {2}.", FunctionCall.ToString(), ObjectExpression.ToString(), obj.GetType().Name));
        }

        /// <summary>
        /// Pretty-print to help with debugging and testing.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", ObjectExpression.ToString(), FunctionCall.ToString());
        }
    }
}
