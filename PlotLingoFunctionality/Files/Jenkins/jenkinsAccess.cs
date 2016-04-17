using PlotLingoLib;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Files.Jenkins
{
    /// <summary>
    /// Wrapper code to access a Jenkins build artifact.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    public class jenkinsAccess : IFunctionObject
    {
        /// <summary>
        /// Fetch a Jenkins build artifact.
        /// Use the latestBuild link to always fetch the latest one.
        /// Use a specific build URL to fetch the current one.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static NTFile jenkins(string url)
        {
            var fi = JenkAccess.GetBuildArtifiact(url).Result;

            var f = NTFile.Open(fi.FullName, "READ");
            if (!f.IsOpen())
                throw new ArgumentException($"ROOT is unable to open file {fi.FullName} from the Jenkins build server ({url}).");

            return f;
        }
    }
}
