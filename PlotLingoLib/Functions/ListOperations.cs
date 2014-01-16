using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoLib.Functions
{
    [Export(typeof(IFunctionObject))]
    class ListOperations : IFunctionObject
    {
        /// <summary>
        /// Add the values up!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object sum(IEnumerable<object> input)
        {
            // Since these are already evalutaed objects (or should be), we don't
            // need a real context.
            var tmp = new RootContext();
            var r = input.Aggregate((v1, v2) =>
            {
                var fo = new FunctionExpression("+", new IExpression[] { new ObjectValue(v1), new ObjectValue(v2) });
                return fo.Evaluate(tmp);
            });
            return r;
        }
    }
}
