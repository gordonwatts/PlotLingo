﻿using Sprache;
using System.Text.RegularExpressions;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Represents a string value.
    /// </summary>
    public class StringValue : IExpression
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
        /// Pattern to find anything that needs replacement
        /// </summary>
        static Regex _findVarSub = new Regex(@"{([^{}]+)}");

        /// <summary>
        /// Evaluate a string value.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            // Find all variable subsitutions that have to be done in the string.
            var myl = _content;
            var m = _findVarSub.Match(myl);
            while (m != null && m.Success)
            {
                var vname = m.Groups[1].Value;

                object result = null;
                try
                {
                    var exprTree = Grammar.ExpressionParser.End().Parse(vname);
                    result = exprTree.Evaluate(c);
                }
                catch { }

                int position = m.Groups[1].Index;
                if (result != null)
                {
                    var s = result.ToString();
                    myl = myl.Replace("{" + vname + "}", s);
                    position += s.Length-1;
                }
                else
                {
                    position += m.Groups[1].Length;
                }
                m = position > myl.Length ? null : _findVarSub.Match(myl, position);
            }
            return myl;
        }

        /// <summary>
        /// Pretty print for debugging and testing.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("\"{0}\"", _content);
        }
    }
}
