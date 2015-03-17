using System.ComponentModel.Composition;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Some basic string operations.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class StringOperations : IFunctionObject
    {
        /// <summary>
        /// Sum strings.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static string OperatorPlus(IScopeContext ctx, string o1, string o2)
        {
            return o1 + o2;
        }
    }
}
