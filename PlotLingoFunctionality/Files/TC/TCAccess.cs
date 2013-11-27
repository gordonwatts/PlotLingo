
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace PlotLingoFunctionality.Files.TC
{
    /// <summary>
    /// Help with access to a TeamCity server
    /// </summary>
    public static class TCAccess
    {
        /// <summary>
        /// Hold onto the basic infomration about a TC full file url
        /// </summary>
        private struct TCInfo
        {
            /// <summary>
            /// The base url http://blahb.blah.edu:5000
            /// </summary>
            public string urlBase;

            /// <summary>
            /// Return the machine name in the url
            /// </summary>
            public string MachineName
            {
                get
                {
                    var idx = urlBase.IndexOf("//") + 2;
                    var r = urlBase.Substring(idx);
                    var last = r.IndexOf(":");
                    if (last < 0)
                        last = r.IndexOf("/");
                    if (last > 0)
                        r = r.Substring(0, last);
                    return r;
                }
            }

            /// <summary>
            /// The name of the build, like "bt11".
            /// </summary>
            public string buildName;

            /// <summary>
            /// The id of the build (like 2555).
            /// </summary>
            public string buildID;

            /// <summary>
            /// The path to the file, like "dataJZ2W-CutFlowPlots.root"
            /// </summary>
            public string path;

            /// <summary>
            /// Return the full URL to the item.
            /// </summary>
            public string FullURL
            {
                get
                {
                    return string.Format("{0}/repository/download/{1}/{2}:id/{3}", urlBase, buildName, buildID, path);
                }
            }

            /// <summary>
            /// The network credential we will use to access the server.
            /// </summary>
            public NetworkCredential cred;
        }

        /// <summary>
        /// Fetch the URL from TC for a particular artifact (e.g. a root file).
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The user can enter a specific URL for the artifiact that they get by cut/pasting the url from their web browser:
        /// 
        ///     http://tc-higgs.phys.washington.edu:8080/repository/download/bt13/2250:id/dataJZ2W-CutFlowPlots.root
        /// or
        ///     http://tc-higgs.phys.washington.edu:8080/repository/download/bt11/2284:id/CombPerf/FlavorTag/JetTagPerformanceCalibration/CalibrationResults/2012-Paper-new.tar.gz%21/2012-Paper-new-rel17_MC11b-CDI.root
        ///     
        /// We will then re-parse that, find the latest build ID, and get that file.
        /// </remarks>
        public async static Task<FileInfo> GetTCBuildArtifactURL(string tcFileURL, bool fetchLatest = true)
        {
            // First, we need to parse the URL into its component parts
            var info = ParseTCURL(tcFileURL);

            // If we are to get the latest ID, updat it.
            if (fetchLatest)
                info.buildID = await FetchLaestBuildID(info);

            // Last thing is to download this file locally. We can't use TFile::Open because of
            // username and password issues.

            return await DownloadFile(info);
        }

        /// <summary>
        /// Download the file. If there, return it.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private async static Task<FileInfo> DownloadFile(TCInfo info)
        {
            var outFile = new FileInfo(string.Format(@"{0}{1}-{2}-{3}", Path.GetTempPath(), info.buildName, info.buildID, Path.GetFileName(info.path)));
            if (outFile.Exists)
                return outFile;

            var req = CreateRequest(info, string.Format("builds/id:{0}/artifacts/files/{1}", info.buildID, info.path), "GET");
            ((HttpWebRequest)req).Accept = "application/octet-stream";
            using (var res = await req.GetResponseAsync())
            using (var readStream = res.GetResponseStream())
            {
                using (var writer = outFile.Create())
                {
                    await readStream.CopyToAsync(writer);
                    writer.Close();
                }
            }
            outFile.Refresh();

            return outFile;
        }

        /// <summary>
        /// Scan the TC server for the most recent build for this file.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private async static Task<string> FetchLaestBuildID(TCInfo info)
        {
            var req = CreateRequest(info, "builds", "GET", Tuple.Create("locator", string.Format("buildType:{0}", info.buildName)));
            using (var res = await req.GetResponseAsync())
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                var r = JsonConvert.DeserializeObject(await reader.ReadToEndAsync()) as JContainer;
                if ((int)r["count"] == 0)
                    throw new InvalidOperationException("No builds!");
                var maxBuildNumber = (from b in (JArray)r["build"]
                                      where (string)b["number"] != "N/A"
                                      orderby (int)b["number"] descending
                                      select b["id"]).First();
                return (string)maxBuildNumber;
            }
        }

        /// <summary>
        /// Parse the urls for a team city download
        /// </summary>
        private static Regex _urlParser = new Regex("^(?<base>.*)/repository/download/(?<bName>[^/]+)/(?<bID>[0-9]+):id/(?<path>.*)$");

        /// <summary>
        /// Parse a teamcity URL into its component parts.
        /// </summary>
        /// <param name="tcFileURL"></param>
        /// <returns></returns>
        private static TCInfo ParseTCURL(string tcFileURL)
        {
            var m = _urlParser.Match(tcFileURL);
            if (!m.Success)
                throw new ArgumentException(string.Format("Unable to parse the team city URL '{0}'", tcFileURL));

            return new TCInfo() { urlBase = m.Groups["base"].Value, buildName = m.Groups["bName"].Value, buildID = m.Groups["bID"].Value, path = m.Groups["path"].Value };
        }

        /// <summary>
        /// Create a URL request to send to teamcity.
        /// </summary>
        /// <param name="what"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static WebRequest CreateRequest(TCInfo info, string what, string method, params Tuple<string, string>[] args)
        {
            var urlbase = string.Format("{0}/httpAuth/app/rest/{1}/", info.urlBase, what);
            if (args.Length > 0)
            {
                char addon = '?';
                foreach (var arg in args)
                {
                    urlbase = string.Format("{0}{1}{2}={3}", urlbase, addon, arg.Item1, arg.Item2);
                    addon = '&';
                }
            }
            var req = WebRequest.Create(urlbase);
            req.Method = method;

            // Creds

            if (info.cred == null)
            {
                Credential cred;
                if (!NativeMethods.CredRead(info.MachineName, CRED_TYPE.GENERIC, 0, out cred))
                {
                    Console.WriteLine("Error getting credentials");
                    Console.WriteLine(string.Format("Use the credential control pannel, create a generic credential for windows domains for {0} with username and password", info.MachineName));
                    throw new InvalidOperationException();
                }
                string password;
                using (var m = new MemoryStream(cred.CredentialBlob, false))
                using (var sr = new StreamReader(m, System.Text.Encoding.Unicode))
                {
                    password = sr.ReadToEnd();
                }
                info.cred = new NetworkCredential(cred.UserName, password);
            }
            req.Credentials = info.cred;

            req.ContentType = "application/json";
            ((HttpWebRequest)req).Accept = "application/json";

            return req;
        }
    }
}
