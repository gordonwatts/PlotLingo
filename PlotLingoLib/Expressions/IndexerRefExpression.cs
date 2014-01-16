using System;
using System.Collections;
using System.Collections.Generic;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// Hold onto a dictionary or array reference that was done with brackets. For example, arr[1] or dict["dude"].
    /// </summary>
    internal class IndexerRefExpression : IExpression
    {
        private IExpression _dictOrArray;
        private IExpression _indexer;

        /// <summary>
        /// Initialize to get ourselves up and running and do the index.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public IndexerRefExpression(IExpression left, IExpression right)
        {
            this._dictOrArray = left;
            this._indexer = right;
        }

        /// <summary>
        /// Hold onto the evaluation so we don't re-run it.
        /// </summary>
        private object _result = null;

        /// <summary>
        /// Evaluate the guy, and cache the result.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            if (_result != null)
                return _result;

            var dictHope = _dictOrArray.Evaluate(c);
            if (!typeof(IDictionary).IsAssignableFrom(dictHope.GetType()))
                throw new InvalidOperationException("Unable to do dictionary or array de-reference from anything but a dictionary!");
            var dict = dictHope as IDictionary;

            var k = _indexer.Evaluate(c);
            if (k == null)
                throw new ArgumentException("null valued key for dictionary lookup");

            if (!dict.Contains(k))
                throw new KeyNotFoundException(string.Format("Unable to locate the key '{0}' in the dictionary", k.ToString()));

            _result = dict[k];
            return _result;
        }

        /// <summary>
        /// Return a nicely formatted version of this lookup.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", _dictOrArray.ToString(), _indexer.ToString());
        }
    }
}
