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
        public static object Sum(IDictionary<object, object> input)
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
    }
}
