using System;

namespace PlotLingoLib
{
    /// <summary>
    /// This is the context that gets passed around to all things that are executing.
    /// It represents the variables, etc., that are current in scope, etc.
    /// </summary>
    public interface IScopeContext
    {
        /// <summary>
        /// Set a variable to a particular value. If it hasn't been created it,
        /// create the variable.
        /// </summary>
        /// <param name="vname">Variable Name</param>
        /// <param name="val">Value to set it to</param>
        void SetVariableValue(string vname, object val);

        /// <summary>
        /// Return the value of a variable. Throw if we can't get it.
        /// </summary>
        /// <param name="vname">Variable Name</param>
        /// <returns>Value of variable</returns>
        object GetVariableValue(string vname);

        /// <summary>
        /// Return the value of a variable if we have it, or null if not, and return false as well.
        /// </summary>
        /// <param name="nv">Variable Name</param>
        /// <returns>Tuple, first value indicates if the variable was known, and the second its value if it was</returns>
        Tuple<bool, object> GetVariableValueOrNull(string nv);

        /// <summary>
        /// Add an internal variable, creating and setting it if needed, or replacing it.
        /// </summary>
        /// <param name="vname">Variable Name</param>
        /// <param name="val">Object Value</param>
        void AddInternalVariable(string vname, object val);

        /// <summary>
        /// Return an internal variable name.
        /// </summary>
        /// <param name="vname">Variable Name</param>
        /// <returns>Value of the variable, or null if it isn't defined.</returns>
        object GetInternalVariable(string vname);

        /// <summary>
        /// Add an action that gets called each time an expression statement is called, with the result
        /// </summary>
        /// <param name="act">The action to be called.</param>
        void AddExpressionStatementEvaluationCallback(Action<object> act);

        /// <summary>
        /// Remove an action that gets called each time a 
        /// </summary>
        /// <param name="act"></param>
        void RemoveExpressionStatementEvaluationCallback(Action<object> act);

        /// <summary>
        /// Called to trigger reporting of an object that has been evaluated in an expression statement.
        /// </summary>
        /// <param name="obj">The object that should be distributed to call the callbacks</param>
        void ReportExpressionStatementEvaluation(object obj);

        /// <summary>
        /// Returns the current execution context (something that is global!).
        /// </summary>
        IExecutionContext ExecutionContext { get; }
    }
}
