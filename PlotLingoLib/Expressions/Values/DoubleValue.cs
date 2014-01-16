
namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Hold onto a double value
    /// </summary>
    class DoubleValue : IExpression
    {
        /// <summary>
        /// Init with the value we will hold onto.
        /// </summary>
        /// <param name="v"></param>
        public DoubleValue(double v)
        {
            Value = v;
        }

        /// <summary>
        /// Return the value, boxed.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(IScopeContext c)
        {
            return Value;
        }

        /// <summary>
        /// Get the value we are holding onto.
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// To aid with debugging and testing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
