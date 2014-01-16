
using PlotLingoLib;
using System.Collections.Generic;
using System.IO;
namespace PlotLingoFunctionality
{
    /// <summary>
    /// Anything that generates an object that is interesting for the outside guys to look at,
    /// should generate it with this attached. It can invoke some post-processing to generate an output.
    /// </summary>
    public interface IPlotScriptResult
    {
        /// <summary>
        /// Return the name of this guy - to help with any file output.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Save the output to a filename stub given, and return full file specs
        /// </summary>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        IEnumerable<FileInfo> Save(RootContext ctx, string filenameStub);
    }
}
