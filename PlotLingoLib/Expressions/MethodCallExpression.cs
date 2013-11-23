﻿using PlotLingoLib.MethodEvaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Object { get; set; }

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
        public MethodCallExpression(string obj, FunctionExpression func)
        {
            Object = obj;
            FunctionCall = func;
        }

        /// <summary>
        /// Default list of method evaluators
        /// </summary>
        private List<IMethodEvaluator> _evaluators = new List<IMethodEvaluator>()
        {
            new DotNetMethodCall()
        };

        /// <summary>
        /// Evaluate the method call. Use a list of evaluators to try to accomplish the call.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            var goodEval = _evaluators.Select(e => e.Evaluate(c, this)).Where(r => r.Item1).FirstOrDefault();
            if (goodEval != null)
                return goodEval.Item2;
            throw new InvalidOperationException(string.Format("Don't know how to call the function {0} on the object {1}.", FunctionCall.ToString(), Object));
        }
    }
}