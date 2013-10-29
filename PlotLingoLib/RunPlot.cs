
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
        public static void Eval(StreamReader reader)
        {
            var sb = new StringBuilder();
            foreach (var l in reader.ReadFromReader())
            {
                sb.AppendLine(l);
            }

            try
            {
                var r = Grammar.ModuleParser.End().Parse(sb.ToString());

                var c = new Context();
                foreach (var st in r)
                {
                    st.Evaluate(c);
                }

                Console.WriteLine(r.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parsing file: {0}", e.Message);
            }
        }

        /// <summary>
        /// Evaluate the contents of a file
        /// </summary>
        /// <param name="fi"></param>
        public static void Eval(this FileInfo fi)
        {
            for (int i = 0; i < 10; i++)
                try
                {
                    using (var reader = fi.OpenText())
                    {
                        Eval(reader);
                        return;
                    }
                }
                catch (IOException e)
                {
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
