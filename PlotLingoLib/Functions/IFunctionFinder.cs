using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Interface for a function finder. It will scan (whatever) to look for a function that can be called
    /// in this context and with these arguments and the given name. Remember to export this guy so it is picked
    /// up by the infrastructure.
    /// </summary>
    public interface IFunctionFinder
    {
        /// <summary>
        /// Find the function. Return null if it can't be found.
        /// </summary>
        /// <param name="c">The running context</param>
        /// <param name="Arguments">The list of arguments to be passed</param>
        /// <param name="fname">function name</param>
        /// <returns></returns>
        Func<object> FindFunction(IScopeContext c, IEnumerable<IExpression> Arguments, string fname);
    }
}