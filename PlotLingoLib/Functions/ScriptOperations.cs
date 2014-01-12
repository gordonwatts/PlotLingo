
using Sprache;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Things that manipulate the script. For example, the include function.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class ScriptOperations : IFunctionObject
    {
        /// <summary>
        /// Include antoher source file. It is loaded and executed immediately, in the same global
        /// context.
        /// </summary>
        /// <param name="filename">Filename to include, if relative, w.r.t. the initially invoked script.</param>
        /// <returns>Returns whatever the last statement of the script returns.</returns>
        public static object include(Context c, string filename)
        {
            var content = FileOperations.readfile(c, filename);

            // Parse for comments, etc.

            var sb = new StringBuilder();
            using (var rdr = (new StringReader(content)))
            {
                foreach (var l in rdr.ReadFromReader())
                {
                    sb.AppendLine(l);
                }

            }

            // Now, push the script context, and off we go.

            c.ScriptFileContextPush(new FileInfo(filename));
            try
            {
                return eval(c, sb.ToString());
            }
            finally
            {
                c.ScriptFileContextPop();
            }
        }

        /// <summary>
        /// Parse and evaluate a single line of script. Done in place, and immediately, and in the
        /// global scope.
        /// </summary>
        /// <param name="scriptline">text to execute</param>
        /// <returns>Whatever the value of the script line is</returns>
        public static object eval(Context c, string scriptline)
        {
            var statements = Grammar.ModuleParser.End().Parse(scriptline);
            object result = null;
            Action<object> saver = o => result = o;
            c.AddExpressionStatementEvaluationCallback(saver);

            try
            {
                foreach (var s in statements)
                {
                    s.Evaluate(c);
                }
            }
            finally
            {
                c.RemoveExpressionStatementEvaluationCallback(saver);
            }
            return result;
        }

        /// <summary>
        /// Returns the full path of the currently executing script.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string currentscript(Context c)
        {
            return c.CurrentScriptFile.FullName;
        }
    }
}
