using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Represents an array of objects of some sort.
    /// </summary>
    internal class ArrayValue : IExpression, IArray
    {
        /// <summary>
        /// Contains a list of array values
        /// </summary>
        /// <param name="values"></param>
        public ArrayValue(IExpression[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Return the array.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            return Values.Select(v => v.Evaluate(c)).ToArray();
        }

        /// <summary>
        /// How many values are there?
        /// </summary>
        public int Length { get { return Values.Length; } }

        /// <summary>
        /// The values for the array - as expressions.
        /// </summary>
        public IExpression[] Values { get; set; }
    }
}
