using System.ComponentModel.Composition;
using System.IO;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Simple operations on files.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class FileOperations : IFunctionObject
    {
        /// <summary>
        /// Read in a file, return the complete contents as an array of strings.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="filename">The filename to read. Hopefully not too big!</param>
        /// <returns></returns>
        public static string readfile(IScopeContext c, string filename)
        {
            // THe search stradegy isn't easy. If this is a relative file, then we need to look near the currently
            // executing script. If the currently executing script is "in memory", then we don't care.

            if (!Path.IsPathRooted(filename) && c.ExecutionContext.ExecutingScript)
            {
                filename = Path.Combine(c.ExecutionContext.CurrentScriptFile.DirectoryName, filename);
            }

            // Attempt to open the file and read it in!
            var f = new FileInfo(filename);
            if (!f.Exists)
                throw new FileNotFoundException(string.Format("Could not load file '{0}' for reading", filename), filename);

            using (var rdr = f.OpenText())
            {
                return rdr.ReadToEnd();
            }
        }
    }
}
