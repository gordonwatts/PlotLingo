using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Hold onto a boolean value.
    /// </summary>
    class BoolValue : IExpression
    {
        /// <summary>
        /// Create a value
        /// </summary>
        /// <param name="v"></param>
        public BoolValue (bool v)
        {
            Value = v;
        }

        /// <summary>
        /// Get/Set the value of the bool.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Return the expression when we are doing the evaluation.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            return Value;
        }

        /// <summary>
        /// For debugging and pretty-printing.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
