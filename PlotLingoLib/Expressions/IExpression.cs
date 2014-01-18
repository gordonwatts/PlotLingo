
namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// An expression that can be evaluated
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Evaluate the expression in the given context
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        object Evaluate(IScopeContext c);
    }
}
