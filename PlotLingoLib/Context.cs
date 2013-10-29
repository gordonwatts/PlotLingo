
using System.Collections.Generic;
namespace PlotLingoLib
{
    /// <summary>
    /// Keep track of the execution context
    /// </summary>
    class Context
    {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        internal void SetVariableValue(string nv, object p)
        {
            _variables[nv] = p;
        }
    }
}
