
using ROOTNET;
using ROOTNET.Globals;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
namespace PlotLingoLib
{
    /// <summary>
    /// Parse a input file
    /// </summary>
    public static class RunPlot
    {
        /// <summary>
        /// Given a stream, read and plot as needed.
        /// </summary>
        /// <param name="reader"></param>
        public static IScopeContext Eval(StreamReader reader, IEnumerable<Action<object>> expressionEvaluationReporters = null, FileInfo mainScriptFile = null)
        {
            var sb = new StringBuilder();
            foreach (var l in reader.ReadFromReader())
            {
                sb.AppendLine(l);
            }

            return Eval(sb.ToString(), expressionEvaluationReporters, mainScriptFile);
        }

        /// <summary>
        /// Run evaluation after we have read in everything.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="scriptFile">The script file we are taking commands from - to establish a context.</param>
        /// <param name="expressionEvaluationReporters"></param>
        private static IScopeContext Eval(string sb, IEnumerable<Action<object>> expressionEvaluationReporters, FileInfo scriptFile = null)
        {
            try
            {
                // Parse the string into an expression statement list.
                var r = Grammar.ModuleParser.End().Parse(sb);

                // For exvaluation, get the context setup correctly.
                var c = new RootContext();
                ExtensibilityControl.Get().InitializeFunctionObjects(c);

                if (scriptFile != null)
                    c.ExecutionContext.ScriptFileContextPush(scriptFile);
                if (expressionEvaluationReporters != null)
                {
                    foreach (var a in expressionEvaluationReporters)
                    {
                        c.AddExpressionStatementEvaluationCallback(a);
                    }
                }

                // Now evaluate each statement, one after the other.
                foreach (var st in r)
                {
                    st.Evaluate(c);
                }

                // Some simple diagnostics
                Console.WriteLine("Parsed and evaluated {0} statements.", r.Length);

                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parsing file: {0}", e.Message);
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    Console.WriteLine(" -> {0}", e.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Evaulate a sequence of files.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="action"></param>
        public static IScopeContext Eval(this List<FileInfo> files, Action<object>[] actions = null, FileInfo mainScriptFile = null)
        {
            Init();
            for (int i = 0; i < 10; i++)
                try
                {
                    var sb = new StringBuilder();
                    foreach (var f in files)
                    {
                        using (var r = f.OpenText())
                        {
                            foreach (var l in r.ReadFromReader())
                            {
                                sb.AppendLine(l);
                            }
                        }
                    }

                    return Eval(sb.ToString(), actions, mainScriptFile);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed: {0}", e.Message);
                    i--;
                    Thread.Sleep(10);
                }
            return null;
        }

        /// <summary>
        /// Evaluate the contents of a file
        /// </summary>
        /// <param name="fi"></param>
        public static IScopeContext Eval(this FileInfo fi, IEnumerable<Action<object>> expressionEvaluationReporters = null)
        {
            Init();
            for (int i = 0; i < 10; i++)
                try
                {
                    using (var reader = fi.OpenText())
                    {
                        return Eval(reader, expressionEvaluationReporters);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed: {0}", e.Message);
                    i--;
                    Thread.Sleep(10);
                }
            return null;
        }

        /// <summary>
        /// Initalized ROOT environment
        /// </summary>
        private static NTApplication s_appHolder = null;
        private static void Init()
        {
            if (s_appHolder != null) return;

            var dummyArgc = new int[] { 0 };
            var dummyStringInfo = new string[] { "" };
            s_appHolder = new NTApplication("PlotLingoParser", dummyArgc, dummyStringInfo);
            gROOT.Value.Batch = true;
        }

        /// <summary>
        /// Read strings from a file and remove the comment lines before handing it back.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        /// <remarks>Implements commends with // at the start of the line, or # anywhere in the line. Doesn't escape the character if it is in quotes!</remarks>
        public static IEnumerable<string> ReadFromReader(this TextReader reader)
        {
            bool done = false;
            while (!done)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    done = true;
                    continue;
                }

                line = line.Trim();

                line = StripLineOfComments(line);

                if (line.Length != 0)
                    yield return line;
            }
        }

        /// <summary>
        /// Strip a line of all comments, but watch out for quotes!
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string StripLineOfComments(string line)
        {
            var pos = FindGoodCommentStart(line);
            if (pos >= 0)
                return line.Substring(0, pos).Trim();
            return line;
        }

        /// <summary>
        /// Find the first comment character, if there is one, and take into
        /// account quoted strings.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static int FindGoodCommentStart(string line)
        {
            int startPost = 0;

            while (startPost < line.Length)
            {
                // Find the first comment character in the line
                var commentChar = FirstCommentCharacter(line, startPost);

                // Next, see if we have a string to contend with.
                var quote = line.IndexOf("\"", startPost);
                if (quote < 0)
                    return commentChar;
                if (quote > commentChar)
                    return commentChar;

                // So, commentChar > quote - which might mean we are in trouble...

                var closeQuote = line.IndexOf("\"", quote + 1);
                if (closeQuote < 0)
                    return commentChar;

                startPost = closeQuote + 1;
            }

            return -1;
        }

        // Find the first comment character in the string
        private static int FirstCommentCharacter(string line, int startPos)
        {
            var pound = line.IndexOf("#", startPos);
            var doubleSlash = line.IndexOf("//", startPos);

            if (pound < 0)
                return doubleSlash;

            if (doubleSlash < 0)
                return pound;

            return Math.Min(pound, doubleSlash);
        }
    }
}
