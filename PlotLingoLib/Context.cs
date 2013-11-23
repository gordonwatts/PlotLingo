
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
        internal object GetVariableValue (string nv)
        {
            object r;
            if (_variables.TryGetValue(nv, out r))
                return r;
            throw new ArgumentException(string.Format("Variable {0} is not defined.", nv));
        }
    }
}
