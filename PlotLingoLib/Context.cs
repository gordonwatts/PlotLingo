
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

        /// <summary>
        /// Maintain a list of post-function and method call hooks.
        /// </summary>
        private Dictionary<string, List<Func<object, object, object>>> _postCallHooks = new Dictionary<string, List<Func<object, object, object>>>();

        /// <summary>
        /// Add a call back that is called after each time the method is called.
        /// </summary>
        /// <param name="functionName">Function to call. First argument is the object this was called against (or null if a function) and the second is the return value from the function or method. And returns the new value of the result object (which may be the same as the old one).</param>
        /// <param name="callback">Name of the method or function that should trigger this callback</param>
        internal void AddPostCallHook (string functionName, Func<object, object, object> callback)
        {
            if (!_postCallHooks.ContainsKey(functionName))
                _postCallHooks[functionName] = new List<Func<object, object, object>>();

            _postCallHooks[functionName].Add(callback);
        }

        /// <summary>
        /// Given a method or function name, find all applicable hooks and call them. Returning
        /// the altered object.
        /// </summary>
        /// <param name="fmName"></param>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal object ExecutePostCallHook (string fmName, object obj, object result)
        {
            List<Func<object, object, object>> callbacks;
            if (_postCallHooks.TryGetValue(fmName, out callbacks))
            {
                foreach (var cb in callbacks)
                {
                    result = cb(obj, result);
                }
            }
            return result;
        }
    }
}
