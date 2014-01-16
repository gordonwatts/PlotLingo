using System.Collections.Generic;

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
    }
}
