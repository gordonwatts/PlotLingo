
using System.IO;
namespace PlotLingoConsole
{
    static class Utils
    {
        /// <summary>
        /// Clean out any letters from the path that aren't legal windows characters.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static string FileNameSantize(this string fname)
        {
            var dirname = Path.GetDirectoryName(fname);
            var filename = Path.GetFileName(fname)
                .Replace("<", "-")
                .Replace(">", "-")
                .Replace(":", "-")
                .Replace("*", "-")
                .Replace("?", "-")
                .Replace("\"", "-")
                .Replace("|", "-")
                ;

            return Path.Combine(dirname, filename);
        }
    }
}
