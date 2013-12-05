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
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorMultiply(IDictionary<object, object> op1, IDictionary<object, object> op2)
        {
            var r = new Dictionary<object, object>();
            var ctx = new Context();

            foreach (var item in op1)
            {
                if (op2.ContainsKey(item.Key))
                {
                    var calc = new FunctionExpression("*", new ObjectValue(item.Value), new ObjectValue(op2[item.Key]));
                    r[item.Key] = calc.Evaluate(ctx);
                }
            }
            return r;
        }

        /// <summary>
        /// Divide a dictionary by a histogram
        /// </summary>
        /// <param name="sDict"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorDivide(IDictionary<object, object> sDict, double constant)
        {
            var r = new Dictionary<object, object>();
            var cv = new DoubleValue(constant);
            var ctx = new Context();

            foreach (var item in sDict)
            {
                var calc = new FunctionExpression("/", new ObjectValue(item.Value), cv);
                r[item.Key] = calc.Evaluate(ctx);
            }
            return r;
        }

        /// <summary>
        /// Divide a constant by a dictionary of items. Apply to each member of the dict constant / member.
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="sDict"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorDivide(double constant, IDictionary<object, object> sDict)
        {
            var r = new Dictionary<object, object>();
            var cv = new DoubleValue(constant);
            var ctx = new Context();

            foreach (var item in sDict)
            {
                var calc = new FunctionExpression("/", cv, new ObjectValue(item.Value));
                r[item.Key] = calc.Evaluate(ctx);
            }
            return r;
        }

        /// <summary>
        /// Divide one matrix by the next
        /// </summary>
        /// <param name="num"></param>
        /// <param name="denom"></param>
        /// <returns></returns>
        public static Dictionary<object, object> OperatorDivide(IDictionary<object, object> num, IDictionary<object, object> denom)
        {
            var r = new Dictionary<object, object>();
            var ctx = new Context();

            foreach (var item in num)
            {
                if (!denom.ContainsKey(item.Key))
                {
                    throw new DivideByZeroException(string.Format("Dictionary entry for '{0}' can't be divided by a non-existant (zero!) dictionary value!", item.Key));
                }
                var calc = new FunctionExpression("/", new ObjectValue(item.Value), new ObjectValue(denom[item.Key]));
                r[item.Key] = calc.Evaluate(ctx);
            }

            return r;
        }
    }
}
