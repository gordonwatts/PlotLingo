using Jenkins.Core;
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
        /// No need to re-fetch if we have it already.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fetchLatest"></param>
        /// <returns></returns>
        public static async Task<FileInfo> GetBuildArtifiact (string url)
        {
            // Fetch access to the server
            var jenksInfo = new JInfo(url);

            // Next, determine the job information for this guy
            var artifactInfo = jenksInfo.GetArtifactInfo();

            // Build the file path where we will store it. If it is already there,
            // then we are done!
            var location = new FileInfo($"{Path.GetTempPath()}\\PlotLingo\\{artifactInfo.JobName}\\{artifactInfo.BuildNumber}-{artifactInfo.ArtifactName}");
            if (location.Exists)
            {
                return location;
            }

            // If isn't there, then download it.
            await jenksInfo.Download(artifactInfo);
            location.Refresh();
            if (!location.Exists)
            {
                throw new InvalidOperationException($"Unable to download the Jenkins build artifact at the URL {url}.");
            }
            return location;
        }

        /// <summary>
        /// Tracks access to a particular server. Though currently it looks like it only does a single file. Perhaps
        /// when we need something better.
        /// </summary>
        class JInfo
        {
            /// <summary>
            /// Info on an artifact
            /// </summary>
            public class Info
            {
                public string JobName;
                public string ArtifactName;
                public int BuildNumber;
            }

            /// <summary>
            /// The URL that contains the server info
            /// </summary>
            Uri _serverURI;

            /// <summary>
            /// Access to Jenkins info
            /// </summary>
            IJenkinsRestClient _JenkinsEndPoint;

            public JInfo (string url)
            {
                _serverURI = new Uri(url);
                Init();
            }

            /// <summary>
            /// Get the server stuff configured.
            /// </summary>
            public void Init()
            {
                var fact = new JenkinsRestFactory();
                _JenkinsEndPoint = fact.GetClient();
            }

            /// <summary>
            /// Parse the URL to figure out everything we need
            /// </summary>
            /// <returns></returns>
            public Info GetArtifactInfo()
            {
                var j = _JenkinsEndPoint.GetJobAsync(_serverURI.OriginalString);
                return null;
            }

            internal Task Download(Info artifactInfo)
            {
                throw new NotImplementedException();
            }
        }
    }
}
