using PlotLingoLib;
using System;
using System.ComponentModel.Composition;

namespace PlotLingoFunctionality.Files.TC
{
    [Export(typeof(IFunctionObject))]
    public class teamcityAccess : IFunctionObject
    {
        public static ROOTNET.Interface.NTFile teamcity(string url)
        {
            var fi = TCAccess.GetTCBuildArtifactURL(url, fetchLatest: true).Result;

            var f = ROOTNET.NTFile.Open(fi.FullName);
            if (!f.IsOpen())
                throw new ArgumentException(string.Format("ROOT is unable to open file {0}.", fi.FullName));

            return f;
        }
    }
}
