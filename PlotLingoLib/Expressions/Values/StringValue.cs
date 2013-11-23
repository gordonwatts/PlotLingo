using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Represents a string value.
    /// </summary>
    internal class StringValue : IExpression
    {
        /// <summary>
        /// The string we are going to be returning.
        /// </summary>
        private string _content;

        /// <summary>
        /// Initalize a string value.
        /// </summary>
        /// <param name="content"></param>
        public StringValue(string content)
        {
            this._content = content;
        }

        /// <summary>
        /// Evaluate a string value.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            return _content;
        }
    }
}
