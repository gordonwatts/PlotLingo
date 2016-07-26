using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
        public string FunctionName { get; set; }

        /// <summary>
        /// A list of arguments
        /// </summary>
        public IExpression[] Arguments { get; set; }

        /// <summary>
        /// Initialize a function expression.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="args"></param>
        public FunctionExpression(string fname, params IExpression[] args)
        {
            FunctionName = fname;
            Arguments = args;
        }

        /// <summary>
        /// Evaluate a function.
        /// </summary>
        /// <param name="c">Context to use while doing the execution</param>
        /// <returns>Whatever the function returns</returns>
        /// <remarks>Use the IFunctionObject to decorate any function you want to add</remarks>
        public object Evaluate(IScopeContext c)
        {
            Func<object> funcCaller;
            if (_binaryOperators.ContainsKey(FunctionName))
            {
                funcCaller = FindBinaryFunction(c, Arguments, _binaryOperators[FunctionName]);
            }
            else
            {
                funcCaller = FindFunction(c, Arguments, FunctionName.FixUpReserved());
            }

            if (funcCaller == null)
                throw new NotImplementedException(string.Format("Unable to find function {0}", FunctionName));

            // Now call the method.
            try
            {
                var r = funcCaller(); // s.Invoke(null, args);

                // Deal with post-hook call backs now
                r = c.ExecutionContext.ExecutePostCallHook(FunctionName, null, r);
                return r;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        /// <summary>
        /// We are dealing with a binary function of some sort - like +, -, etc.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Arguments"></param>
        /// <param name="opInfo"></param>
        /// <returns></returns>
        private Func<object> FindBinaryFunction(IScopeContext c, IExpression[] Arguments, OpInfo opInfo)
        {
            if (Arguments.Length != 2)
                throw new InvalidOperationException(string.Format("Binary operation {0} needs exactly two arguments", opInfo.MethodName));

            var funcCaller = FindFunction(c, Arguments, opInfo.MethodName);
            if (funcCaller != null || !opInfo.ReverseOperandsOK)
                return funcCaller;

            return FindFunction(c, Arguments.Reverse(), opInfo.MethodName);
        }

        /// <summary>
        /// Internal function to hide looking at all function objects.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="arguments"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        private Func<object> FindFunction(IScopeContext c, IEnumerable<IExpression> arguments, string functionName)
        {
            return ExtensibilityControl.Get().FunctionFinders
                .Select(ff => ff.FindFunction(c, arguments, functionName))
                .Where(f => f != null)
                .FirstOrDefault();
        }

        /// <summary>
        /// Info about what we can do for each operand
        /// </summary>
        private struct OpInfo
        {
            public bool ReverseOperandsOK;
            public string MethodName;
        }

        /// <summary>
        /// Specifications for the binary operators we know about.
        /// </summary>
        private static Dictionary<string, OpInfo> _binaryOperators = new Dictionary<string, OpInfo>()
        {
            {"+", new OpInfo(){ MethodName = "OperatorPlus", ReverseOperandsOK = true}},
            {"*", new OpInfo(){ MethodName = "OperatorMultiply", ReverseOperandsOK = true}},
            {"-", new OpInfo(){ MethodName = "OperatorMinus", ReverseOperandsOK = false}},
            {"/", new OpInfo(){ MethodName = "OperatorDivide", ReverseOperandsOK = false}},
        };

        /// <summary>
        /// Pretty print for debugging and testing.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}(", FunctionName);
            bool first = true;
            foreach (var v in Arguments)
            {
                if (!first)
                    sb.Append(",");
                first = false;
                sb.Append(v.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
