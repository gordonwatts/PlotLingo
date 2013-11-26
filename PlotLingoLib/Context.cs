
using System;
using System.Collections.Generic;
namespace PlotLingoLib
{
    /// <summary>
    /// Keep track of the execution context
    /// </summary>
    class Context
    {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        /// <summary>
        /// Save a variable
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="p"></param>
        internal void SetVariableValue(string nv, object p)
        {
            _variables[nv] = p;
        }

        /// <summary>
        /// Return the value of a variable
        /// </summary>
        /// <param name="nv"></param>
        /// <returns></returns>
        internal object GetVariableValue(string nv)
        {
            object r;
            if (_variables.TryGetValue(nv, out r))
                return r;
            throw new ArgumentException(string.Format("Variable {0} is not defined.", nv));
        }

        /// <summary>
        /// Maintain a list of actions to perform when an expression is evaluated.
        /// </summary>
        private List<Action<object>> _expressionEvaluationCallbacks = new List<Action<object>>();

        /// <summary>
        /// Add an action to be performed when an expression is evaluated.
        /// </summary>
        /// <param name="act"></param>
        internal void AddExpressionStatementEvaluationCallback(Action<object> act)
        {
            _expressionEvaluationCallbacks.Add(act);
        }

        /// <summary>
        /// Report objects that have been evaluated.
        /// </summary>
        /// <param name="obj"></param>
        internal void ReportExpressionStatementEvaluation(object obj)
        {
            foreach (var a in _expressionEvaluationCallbacks)
            {
                a(obj);
            }
        }
    }
}
