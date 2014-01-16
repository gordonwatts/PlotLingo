
using System;
using System.Collections.Generic;
namespace PlotLingoLib
{
    /// <summary>
    /// This is the top level stack Context. We track *everything* here, and no passing up the path either.
    /// </summary>
    public class RootContext : IScopeContext
    {
        public RootContext()
        {
            ExecutionContext = new ExecutionContext();
        }

        /// <summary>
        /// The variables we know about
        /// </summary>
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
        public Tuple<bool, object> GetVariableValueOrNull(string nv)
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
        public void AddExpressionStatementEvaluationCallback(Action<object> act)
        {
            _expressionEvaluationCallbacks.Add(act);
        }

        /// <summary>
        /// Remove an expression statement eval callback
        /// </summary>
        /// <param name="act">The saver that was passed to AddExpressionStatementEvaluationCallback.</param>
        public void RemoveExpressionStatementEvaluationCallback(Action<object> act)
        {
            _expressionEvaluationCallbacks.Remove(act);
        }

        /// <summary>
        /// Report objects that have been evaluated.
        /// </summary>
        /// <param name="obj"></param>
        public void ReportExpressionStatementEvaluation(object obj)
        {
            foreach (var a in _expressionEvaluationCallbacks)
            {
                a(obj);
            }
        }

        /// <summary>
        /// Get the current execution context
        /// </summary>
        public IExecutionContext ExecutionContext
        {
            get;
            private set;
        }
    }
}
