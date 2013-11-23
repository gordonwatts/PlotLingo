using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib.Expressions
{
    /// <summary>
    /// A method call made to some object.
    /// </summary>
    internal class MethodCallExpression : IExpression
    {
        /// <summary>
        /// The target object
        /// </summary>
        private string _object;

        /// <summary>
        /// The name of the method we are to call along with its arguments.
        /// </summary>
        /// <remarks>We re-use the function expression here as it has everything
        /// we need.</remarks>
        private IExpression func;

        /// <summary>
        /// Initialize a method call expression.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        public MethodCallExpression(string obj, IExpression func)
        {
            this._object = obj;
            this.func = func;
        }

        /// <summary>
        /// Evaluate the method call.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public object Evaluate(Context c)
        {
            throw new System.NotImplementedException();
        }
    }
}
