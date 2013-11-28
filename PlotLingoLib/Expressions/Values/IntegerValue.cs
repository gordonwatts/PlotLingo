using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// An integer value that has been parsed.
    /// </summary>
    class IntegerValue : IExpression
    {
        /// <summary>
        /// Initialize with the value
        /// </summary>
        /// <param name="v"></param>
        public IntegerValue (int v)
        {
            Value = v;
        }

        /// <summary>
        /// Return the actual value
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            return Value;
        }

        /// <summary>
        /// Pretty-print in order to help with testing and debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// The value this guy represents.
        /// </summary>
        public int Value { get; private set; }
    }
}
