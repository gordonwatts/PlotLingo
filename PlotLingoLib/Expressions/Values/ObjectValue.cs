
namespace PlotLingoLib.Expressions.Values
{
    /// <summary>
    /// Trival holder object that will stash any sort of object.
    /// </summary>
    class ObjectValue : IExpression
    {
        public ObjectValue(object v)
        {
            Value = v;
        }

        public object Evaluate(IScopeContext c)
        {
            return Value;
        }

        /// <summary>
        /// Get the object this value represents.
        /// </summary>
        public object Value { get; private set; }
    }
}
