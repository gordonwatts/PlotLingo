using PlotLingoLib.Expressions.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// A function expression. Call a function and return its value.
    /// </summary>
    /// <remarks>This object is also used in method calls to store the method name and arguments</remarks>
    internal class FunctionExpression : IExpression
    {
        /// <summary>
        /// The function name
        /// </summary>
        public string FunctionName {get; set;}

        /// <summary>
        /// A list of arguments
        /// </summary>
        public IExpression[] Arguments { get; set; }

        /// <summary>
        /// Initialize a function expression.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="args"></param>
        public FunctionExpression(string fname, IExpression[] args)
        {
            FunctionName = fname;
            Arguments = args;
        }

        /// <summary>
        /// Evaluate a function.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            if (FunctionName == "file")
            {
                return File.Execute(Arguments.Select(e => e.Evaluate(c)).ToArray());
            }
            throw new System.NotImplementedException(string.Format("Unknown function '{0}' referenced!", FunctionName));
        }
    }
}
