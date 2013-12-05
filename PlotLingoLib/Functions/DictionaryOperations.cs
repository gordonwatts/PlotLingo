using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Functions and extension methods that are dictionary focused
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class DictionaryOperations : IFunctionObject
    {
        /// <summary>
        /// Sum the values together.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object sum(IDictionary<object, object> input)
        {
            // Can't sum if empty because we don't know what type of value to return!
            if (input.Count == 0)
                throw new NotSupportedException("Unable to sum an empty dictionary.");

            var tmp = new Context();
            var r = input.Values.Aggregate((v1, v2) =>
            {
                var fo = new FunctionExpression("+", new IExpression[] { new ObjectValue(v1), new ObjectValue(v2) });
                return fo.Evaluate(tmp);
            });
            return r;
        }

        /// <summary>
        /// Multipley a dictionary by a constant. Multiplies each individual item.
        /// </summary>
        /// <param name="sDict"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorMultiply(IDictionary<object, object> sDict, double constant)
        {
            var r = new Dictionary<object, object>();
            var cv = new DoubleValue(constant);
            var ctx = new Context();

            foreach (var item in sDict)
            {
                var calc = new FunctionExpression("*", new ObjectValue(item.Value), cv);
                r[item.Key] = calc.Evaluate(ctx);
            }
            return r;
        }

        /// <summary>
        /// Multiply the first dict by the second. Items that are missing are treated as zeros.
        /// </summary>
        /// <param name="numDict"></param>
        /// <param name="denomDict"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorMultiply(IDictionary<object, object> numDict, IDictionary<object, object> denomDict)
        {
            var r = new Dictionary<object, object>();
            var ctx = new Context();

            foreach (var item in numDict)
            {
                if (denomDict.ContainsKey(item.Key))
                {
                    var calc = new FunctionExpression("*", new ObjectValue(item.Value), new ObjectValue(denomDict[item.Key]));
                    r[item.Key] = calc.Evaluate(ctx);
                }
            }
            return r;
        }
    }
}
