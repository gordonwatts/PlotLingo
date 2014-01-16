
namespace PlotLingoLib.Statements
{
    /// <summary>
    /// A statement that can be evaluated. A statement returns nothing.
    /// Presumably, it has side effects!
    /// </summary>
    interface IStatement
    {
        void Evaluate(IScopeContext c);
    }
}
