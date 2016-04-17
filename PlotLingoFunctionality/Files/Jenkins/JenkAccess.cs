using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Files.Jenkins
{
    /// <summary>
    /// Does the work of accessing the Jenkins server.
    /// </summary>
    class JenkAccess
    {
        /// <summary>
        /// Fetch a file that is an artifact. Cache it locally in a temp area.
        /// No need to re-fecth if we have it already.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fetchLatest"></param>
        /// <returns></returns>
        public static Task<FileInfo> GetBuildArtifiact (string url)
        {
            throw new NotImplementedException("Method GetBuildArtifact isn't implemented yet.");
        }
    }
}
