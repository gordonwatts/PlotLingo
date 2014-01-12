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
        /// Read in a file, return the copmlete contents as an array of strings.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="filename">The filename to read. Hopefully not too big!</param>
        /// <returns></returns>
        public static string readfile(Context c, string filename)
        {
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
