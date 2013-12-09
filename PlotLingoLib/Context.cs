
using System;
using System.Collections.Generic;
namespace PlotLingoLib
{
    /// <summary>
    /// Keep track of the execution context
    /// </summary>
    public class Context
    {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        /// <summary>
        /// Save a variable
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="p"></param>
        public void SetVariableValue(string nv, object p)
        {
            _variables[nv] = p;
        }

        /// <summary>
        /// Return the value of a variable
        /// </summary>
        /// <param name="nv"></param>
        /// <returns></returns>
        public object GetVariableValue(string nv)
        {
            object r;
            if (_variables.TryGetValue(nv, out r))
                return r;
            throw new ArgumentException(string.Format("Variable {0} is not defined.", nv));
        }

        /// <summary>
        /// Cache of private variables that we keep around - should never be used for public variable
        /// resolution.
        /// </summary>
        private Dictionary<string, object> _internalVariables = new Dictionary<string, object>();

        /// <summary>
        /// Add internal variable.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="val"></param>
        public void AddInternalVariable(string v, object val)
        {
            _internalVariables[v] = val;
        }

        /// <summary>
        /// Return an internal variable, or null if we don't know about it.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public object GetInternalVariable(string v)
        {
            object val = null;
            if (_internalVariables.TryGetValue(v, out val))
                return val;
            return null;
        }

        /// <summary>
        /// Return the variable value if we know about it, or null.
        /// </summary>
        /// <param name="nv">Variable to look up</param>
        /// <returns>Null if the variable isn't defiend, or the value</returns>
        internal Tuple<bool, object> GetVariableValueOrNull(string nv)
        {
            object r;
            if (_variables.TryGetValue(nv, out r))
                return Tuple.Create(true, r);
            return Tuple.Create(false, (object)null);
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
        private Dictionary<string, Dictionary<string, Func<object, object, object>>> _postCallHooks = new Dictionary<string, Dictionary<string, Func<object, object, object>>>();

        /// <summary>
        /// Add a call back that is called after each time the method is called.
        /// </summary>
        /// <param name="functionName">Function to call. First argument is the object this was called against (or null if a function) and the second is the return value from the function or method. And returns the new value of the result object (which may be the same as the old one).</param>
        /// <param name="callback">Name of the method or function that should trigger this callback</param>
        /// <param name="slotname">The slot where the callback is called. One function per slot, and overwrite anything that was there.</param>
        public void AddPostCallHook(string functionName, string slotname, Func<object, object, object> callback)
        {
            if (!_postCallHooks.ContainsKey(functionName))
                _postCallHooks[functionName] = new Dictionary<string, Func<object, object, object>>();

            _postCallHooks[functionName][slotname] = callback;
        }

        /// <summary>
        /// Given a method or function name, find all applicable hooks and call them. Returning
        /// the altered object.
        /// </summary>
        /// <param name="fmName"></param>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal object ExecutePostCallHook(string fmName, object obj, object result)
        {
            Dictionary<string, Func<object, object, object>> callbacks;
            if (_postCallHooks.TryGetValue(fmName, out callbacks))
            {
                foreach (var cb in callbacks)
                {
                    result = cb.Value(obj, result);
                }
            }
            return result;
        }
    }
}
