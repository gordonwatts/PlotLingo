using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlotLingoLib
{
    /// <summary>
    /// Utility extension functions to help with our functions
    /// </summary>
    static class FunctionUtils
    {
        /// <summary>
        /// if a function name is a reserved C# keyword, then append something to the end (by default "Reserved").
        /// </summary>
        /// <param name="funcName">Function name</param>
        /// <param name="addOn">What to add if the function name is a reserved word</param>
        /// <returns>The function name if it isn't a C# reserved keyword, or the function name + addOn if it is</returns>
        public static string FixUpReserved(this string funcName, string addOn = "Reserved")
        {
            if (_reservedWords.Contains(funcName))
                return funcName + addOn;
            return funcName;
        }

        /// <summary>
        /// All the C# reserved words, as taken from http://msdn.microsoft.com/en-us/library/x53a06bb.aspx
        /// </summary>
        private static HashSet<string> _reservedWords = new HashSet<string>() {
            "abstract",
            "as",
            "base",
            "bool",
            "break", 
            "byte",
            "case",
            "catch",
            "char", 
            "checked",
            "class", 
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked", 
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void", 
            "volatile",
            "while",
        };

        /// <summary>
        /// Helper class to prevent excessive evaluation of function
        /// arguments during a longer function search.
        /// </summary>
        public class ArgEvalHolder
        {
            /// <summary>
            /// Get the expression that this guy holds
            /// </summary>
            public IExpression Expression { get; private set; }
            private IScopeContext _context;
            public ArgEvalHolder(IScopeContext c, IExpression e)
            {
                Expression = e;
                _context = c;
            }

            object _cache = null;

            /// <summary>
            /// Get the type of the object
            /// </summary>
            public Type Type
            {
                get
                {
                    Evaluate();
                    return _cache.GetType();
                }
            }

            /// <summary>
            /// When the time comes to evaluate the parameter, we
            /// will evaluate the parameter!
            /// </summary>
            private void Evaluate()
            {
                if (_cache == null)
                    _cache = Expression.Evaluate(_context);
            }

            /// <summary>
            /// Get the value of the parameter
            /// </summary>
            public object Value
            {
                get
                {
                    Evaluate();
                    return _cache;
                }
            }
        }

        /// <summary>
        /// Short hand to hold onto pairing of parameter info and argument list
        /// </summary>
        public class ArgPairing
        {
            public ArgEvalHolder _arg;
            public ParameterInfo _parameter;
        }

        /// <summary>
        /// LINQ helper function to properly zip together an argument list and a parameter list so that one can simultaniously compare them.
        /// </summary>
        public static IEnumerable<ArgPairing> ZipArgs(IEnumerable<ArgEvalHolder> arglist, IEnumerable<ParameterInfo> parameters)
        {
            var nextParam = parameters.GetEnumerator();
            var nextArg = arglist.GetEnumerator();

            if (nextParam.MoveNext())
            {

                bool done = false;
                while (!done)
                {
                    if (nextParam.Current.ParameterType == typeof(IScopeContext))
                    {
                        yield return new ArgPairing() { _arg = null, _parameter = nextParam.Current };
                        done = !nextParam.MoveNext();
                    }
                    else
                    {
                        if (!nextArg.MoveNext())
                        {
                            yield return new ArgPairing() { _arg = null, _parameter = nextParam.Current };
                        }
                        else
                        {
                            yield return new ArgPairing() { _arg = nextArg.Current, _parameter = nextParam.Current };
                        }
                        done = !nextParam.MoveNext();
                    }
                }
            }
        }
    }
}
