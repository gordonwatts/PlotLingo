
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
        public VariableValue(string vname)
        {
            VariableName = vname;
        }

        /// <summary>
        /// Evaluate the variable
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            return c.GetVariableValue(VariableName);
        }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// For easy debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return VariableName;
        }
    }
}
