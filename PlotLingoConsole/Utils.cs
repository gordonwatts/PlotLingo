
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
            return fname
                .Replace("<", "-")
                .Replace(">", "-")
                .Replace(":", "-")
                .Replace("*", "-")
                .Replace("?", "-")
                .Replace("\"", "-")
                .Replace("|", "-")
                .Replace("\\", "")
                ;
        }
    }
}
