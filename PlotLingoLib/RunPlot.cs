
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
        public static void Eval(StreamReader reader, IEnumerable<Action<object>> expressionEvaluationReporters = null)
        {
            var sb = new StringBuilder();
            foreach (var l in reader.ReadFromReader())
            {
                sb.AppendLine(l);
            }

            Eval(sb.ToString(), expressionEvaluationReporters);
        }

        /// <summary>
        /// Run evaluation after we have read in everything.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="expressionEvaluationReporters"></param>
        private static void Eval(string sb, IEnumerable<Action<object>> expressionEvaluationReporters)
        {
            try
            {
                // Parse the string into an expression statement list.
                var r = Grammar.ModuleParser.End().Parse(sb);

                // For exvaluation, get the context setup correctly.
                var c = new Context();
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parsing file: {0}", e.Message);
            }
        }

        /// <summary>
        /// Evaulate a sequence of files.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="action"></param>
        public static void Eval(this List<FileInfo> files, Action<object>[] actions = null)
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

            Eval(sb.ToString(), actions);
        }

        /// <summary>
        /// Evaluate the contents of a file
        /// </summary>
        /// <param name="fi"></param>
        public static void Eval(this FileInfo fi, IEnumerable<Action<object>> expressionEvaluationReporters = null)
        {
            for (int i = 0; i < 10; i++)
                try
                {
                    using (var reader = fi.OpenText())
                    {
                        Eval(reader, expressionEvaluationReporters);
                        return;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed: {0}", e.Message);
                    i--;
                    Thread.Sleep(10);
                }
        }

        /// <summary>
        /// Read strings from a file and remove the comment lines before handing it back.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        /// <remarks>Implements commends with // at the start of the line, or # anywhere in the line. Doesn't escape the character if it is in quotes!</remarks>
        private static IEnumerable<string> ReadFromReader(this StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line.StartsWith("//"))
                    continue;

                var commentIndex = line.IndexOf("#");
                if (commentIndex >= 0)
                {
                    line = line.Substring(0, commentIndex);
                }
                line = line.Trim();

                if (line.Length != 0)
                    yield return line;
            }
        }
    }
}
