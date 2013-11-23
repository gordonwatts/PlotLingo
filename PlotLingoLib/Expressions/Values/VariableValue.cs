using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Represents a variable expression - retursn the value of the variable.
    /// </summary>
    class VariableValue : IExpression
    {
        /// <summary>
        /// Initialize the variable
        /// </summary>
        /// <param name="vname"></param>
        public VariableValue (string vname)
        {
            VariableName = vname;
        }

        /// <summary>
        /// Evaluate the variable
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            return c.GetVariableValue(VariableName);
        }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }
    }
}
