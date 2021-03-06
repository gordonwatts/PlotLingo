﻿using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoLib.Functions
{
    [Export(typeof(IFunctionObject))]
    public class ListOperations : IFunctionObject
    {
        /// <summary>
        /// Add the values up!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object sum(IEnumerable<object> input)
        {
            // Since these are already evaluated objects (or should be), we don't
            // need a real context.
            var tmp = new RootContext();
            var r = input.Aggregate((v1, v2) =>
            {
                var fo = new FunctionExpression("+", new IExpression[] { new ObjectValue(v1), new ObjectValue(v2) });
                return fo.Evaluate(tmp);
            });
            return r;
        }

        /// <summary>
        /// If any objects are arrays, lift them. Do this only one level down.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<object> flatten(IEnumerable<object> input)
        {
            foreach (var o in input)
            {
                var asEnumerable = o as IEnumerable<object>;
                if (asEnumerable == null)
                {
                    yield return o;
                }
                else
                {
                    foreach (var o1 in asEnumerable)
                    {
                        yield return o1;
                    }
                }
            }
        }

        /// <summary>
        /// Divide a list of items by something else. Depends on the divide function being defined, of course!
        /// </summary>
        /// <param name="input"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static IEnumerable<object> OperatorDivide(IEnumerable<object> input, object divisor)
        {
            var c = new RootContext();
            var r = input.Select(num =>
            {
                var fo = new FunctionExpression("/", new ObjectValue(num), new ObjectValue(divisor));
                return fo.Evaluate(c);
            });
            return r;
        }

        /// <summary>
        /// Concat two lists.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static IEnumerable<object> OperatorPlus(IEnumerable<object> i1, IEnumerable<object> i2)
        {
            return i1.Concat(i2);
        }
    }
}
