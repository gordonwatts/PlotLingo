using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Files.Jenkins
{
    static class JenkAccessUtils
    {
        /// <summary>
        /// Skip all items in a sequence until the last one that satisfies test. Then return that item along with
        /// everything else after it till the end of the sequence. WARNING: this must cache the sequence!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static IEnumerable<T> SkipUntilLast<T>(this IEnumerable<T> source, Func<T, bool> test)
        {
            var list = new List<T>();
            foreach (var item in source)
            {
                if (test(item))
                {
                    list.Clear();
                }
                list.Add(item);
            }
            return list;
        }
    }

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
            var artifactInfo = await jenksInfo.GetArtifactInfo();

            // Build the file path where we will store it. If it is already there,
            // then we are done!
            var location = new FileInfo($"{Path.GetTempPath()}\\PlotLingo\\{artifactInfo.JobName}\\{artifactInfo.BuildNumber}-{artifactInfo.ArtifactName}");
            if (location.Exists)
            {
                return location;
            }

            // If isn't there, then download it.
            await jenksInfo.Download(artifactInfo, location);
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
            /// Access to Jenkins info
            /// </summary>
            Lazy<JenkinsEndPoint> _JenkinsEndPoint = new Lazy<JenkinsEndPoint>(() => new JenkinsEndPoint());

            Uri _artifactURI;

            string _jobName;
            string _buildName;
            string _artifactName;

            public JInfo(string url)
            {
                _artifactURI = new Uri(url);
                var segments = _artifactURI.Segments;

                // Get the job and artifact.
                var artifactInfo = segments.SkipUntilLast(s => s == "job/").Skip(1).Select(s => s.Trim('/')).ToArray();
                if (artifactInfo.Length != 4 && artifactInfo[2] == "artifact")
                {
                    throw new ArgumentException($"The Jenkins artifact URI '{url}' is not in a format I recognize (.../jobname/build/artifact/artifact-name)");
                }
                _jobName = artifactInfo[0];
                _buildName = artifactInfo[1];
                _artifactName = artifactInfo[3];
            }

            /// <summary>
            /// Get everything setup. Some of this setup may require going up to the server.
            /// </summary>
            /// <returns></returns>
            private async Task Init()
            {
                // The only key here is if the build number is not determined at this point.
                if (_buildName == "lastSuccessfulBuild")
                {
                    _buildName = (await GetLastSuccessfulBuild()).ToString();
                }
            }

            /// <summary>
            /// Fetch the last successful build.
            /// </summary>
            /// <returns></returns>
            private async Task<int> GetLastSuccessfulBuild()
            {
                // Build the job URI, which will then return the JSON.
                var jobURIStem = GetJobURIStem();
                var r = await _JenkinsEndPoint.Value.FetchJSON<JenkinsDomain.JenkinsJob>(jobURIStem);

                if (r.lastSuccessfulBuild == null)
                {
                    throw new InvalidOperationException($"This Jenkins job does not yet have a successful build! {jobURIStem.OriginalString}");
                }

                return r.lastSuccessfulBuild.number;
            }

            /// <summary>
            /// Return a Uri of the job stem.
            /// </summary>
            /// <returns></returns>
            private Uri GetJobURIStem()
            {
                return new Uri(_artifactURI.OriginalString.Substring(0, _artifactURI.OriginalString.IndexOf(_jobName) + _jobName.Length));
            }

            /// <summary>
            /// Parse the URL to figure out everything we need
            /// </summary>
            /// <returns></returns>
            public async Task<Info> GetArtifactInfo()
            {
                await Init();
                return new Info()
                {
                    JobName = _jobName,
                    BuildNumber = int.Parse(_buildName),
                    ArtifactName = _artifactName
                };
            }

            /// <summary>
            /// Download the artifact!
            /// </summary>
            /// <param name="artifactInfo"></param>
            /// <returns></returns>
            internal async Task Download(Info artifactInfo, FileInfo destination)
            {
                // Build the url
                var jobURI = GetJobURIStem();
                var artifactUri = new Uri($"{jobURI.OriginalString}/{artifactInfo.BuildNumber}/artifact/{artifactInfo.ArtifactName}");

                await _JenkinsEndPoint.Value.DownloadFile(artifactUri, destination);
            }
        }
    }
}
