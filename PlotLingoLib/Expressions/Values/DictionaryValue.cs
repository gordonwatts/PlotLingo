﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// A dictionary!
    /// </summary>
    class DictionaryValue : IExpression
    {
        /// <summary>
        /// Initialize with a sequence of values.
        /// </summary>
        /// <param name="values"></param>
        public DictionaryValue(IEnumerable<Tuple<IExpression, IExpression>> values)
        {
            Values = values.ToArray();
        }

        /// <summary>
        /// Empty dictionary initalizer.
        /// </summary>
        public DictionaryValue()
        {
            Values = new Tuple<IExpression, IExpression>[0];
        }

        /// <summary>
        /// Evaluate the dictionary for use. Cache result
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            if (_cached != null)
                return _cached;

            var result = new Dictionary<object, object>();

            foreach (var v in Values)
            {
                result[v.Item1.Evaluate(c)] = v.Item2.Evaluate(c);
            }
            _cached = result;
            return result;
        }

        /// <summary>
        /// Cache the result of evaluation to prevent multiple evaluation.
        /// </summary>
        private IDictionary<object, object> _cached = null;

        /// <summary>
        /// Dump for debugging and other reasons.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("{");
            bool first = true;
            foreach (var v in Values)
            {
                if (!first)
                    b.Append(", ");
                first = false;
                b.AppendFormat("{0} => {1}", v.Item1.ToString(), v.Item2.ToString());
            }
            b.Append("}");
            return b.ToString();
        }

        /// <summary>
        /// Get the list of values as expressions
        /// </summary>
        public Tuple<IExpression, IExpression>[] Values { get; private set; }
    }
}
