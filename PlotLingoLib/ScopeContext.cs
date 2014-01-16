using System;

namespace PlotLingoLib
{
    /// <summary>
    /// A scoping context, one down (or more) from the root context
    /// </summary>
    internal class ScopeContext : IScopeContext
    {
        /// <summary>
        /// Keeps track of the local context.
        /// </summary>
        private RootContext _local = new RootContext();

        /// <summary>
        /// Initialize the scope, with a parent context around.
        /// </summary>
        /// <param name="parent"></param>
        public ScopeContext(IScopeContext parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Store the variable we need to track. Unless declared in our scope,
        /// see if it is declared outside our scope. Otherwise, create it here.
        /// </summary>
        /// <param name="vname"></param>
        /// <param name="val"></param>
        public void SetVariableValue(string vname, object val)
        {
            if (_local.GetVariableValueOrNull(vname).Item1)
            {
                _local.SetVariableValue(vname, val);
            }
            else if (Parent.GetVariableValueOrNull(vname).Item1)
            {
                Parent.SetVariableValue(vname, val);
            }
            else
            {
                _local.SetVariableValue(vname, val);
            }
        }

        /// <summary>
        /// Set a variable value locally.
        /// </summary>
        /// <param name="vname">Variable Name</param>
        /// <param name="val">Value to set it to</param>
        internal void SetVariableValueLocally(string vname, object val)
        {
            _local.SetVariableValue(vname, val);
        }

        /// <summary>
        /// Return the variable name. Try local scope, and then march up the chain.
        /// </summary>
        /// <param name="vname"></param>
        /// <returns></returns>
        public object GetVariableValue(string vname)
        {
            var local = _local.GetVariableValueOrNull(vname);
            if (local.Item1)
                return local.Item2;
            return Parent.GetVariableValue(vname);
        }

        /// <summary>
        /// First try local. If that fails, march on up!
        /// </summary>
        /// <param name="vname"></param>
        /// <returns></returns>
        public Tuple<bool, object> GetVariableValueOrNull(string vname)
        {
            var local = _local.GetVariableValueOrNull(vname);
            if (local.Item1)
                return local;
            return Parent.GetVariableValueOrNull(vname);
        }

        /// <summary>
        /// Set a local variable. If defined locally, do that, otherwise, try the parent.
        /// If no where, create it here in the local.
        /// </summary>
        /// <param name="vname"></param>
        /// <param name="val"></param>
        public void AddInternalVariable(string vname, object val)
        {
            if (_local.GetInternalVariable(vname) != null)
            {
                _local.SetVariableValue(vname, val);
            }
            else if (Parent.GetInternalVariable(vname) != null)
            {
                Parent.AddInternalVariable(vname, val);
            }
            else
            {
                _local.SetVariableValue(vname, val);
            }
        }

        /// <summary>
        /// Get an internal variable. First with ourselves, then further up.
        /// </summary>
        /// <param name="vname"></param>
        /// <returns></returns>
        public object GetInternalVariable(string vname)
        {
            var v = _local.GetInternalVariable(vname);
            if (v != null)
                return v;
            return Parent.GetInternalVariable(vname);
        }

        /// <summary>
        /// Add an expression for eval callback to the local list.
        /// </summary>
        /// <param name="act"></param>
        public void AddExpressionStatementEvaluationCallback(Action<object> act)
        {
            _local.AddExpressionStatementEvaluationCallback(act);
        }

        /// <summary>
        /// Remove a callback from the expression callback.
        /// </summary>
        /// <param name="act"></param>
        public void RemoveExpressionStatementEvaluationCallback(Action<object> act)
        {
            _local.RemoveExpressionStatementEvaluationCallback(act);
        }

        /// <summary>
        /// Forward a new expression to everyone we know!
        /// </summary>
        /// <param name="obj"></param>
        public void ReportExpressionStatementEvaluation(object obj)
        {
            _local.ReportExpressionStatementEvaluation(obj);
        }


        /// <summary>
        /// Return the execution context
        /// </summary>
        public IExecutionContext ExecutionContext
        {
            get { return Parent.ExecutionContext; }
        }

        /// <summary>
        /// Get the parent context - the one we are down one from in scope
        /// </summary>
        public IScopeContext Parent { get; private set; }
    }
}
