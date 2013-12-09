
using PlotLingoLib;
using System.Collections.Generic;
using System.ComponentModel.Composition;
namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Help deal with tags that can be associated with objects
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class Tags : IFunctionObject
    {
        /// <summary>
        /// Track the tagging info
        /// </summary>
        private static Dictionary<object, List<string>> _tagInfo = new Dictionary<object, List<string>>();

        /// <summary>
        /// Tag a particular object with some string
        /// </summary>
        /// <param name="objToTag"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public static object tag(object objToTag, string tagname)
        {
            List<string> info = null;
            if (!_tagInfo.TryGetValue(objToTag, out info))
            {
                info = new List<string>();
                _tagInfo[objToTag] = info;
            }
            if (!info.Contains(tagname))
                info.Add(tagname);

            return objToTag;
        }

        /// <summary>
        /// Return true if the object contains a tag.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public static bool hasTag(object obj, string tagname)
        {
            List<string> info = null;
            if (!_tagInfo.TryGetValue(obj, out info))
                return false;
            return info.Contains(tagname);
        }

        /// <summary>
        /// Copy tags over from one to the other
        /// </summary>
        /// <param name="h"></param>
        /// <param name="clone"></param>
        internal static void CopyTags(object hOrig, object hDest)
        {
            List<string> info = null;
            if (_tagInfo.TryGetValue(hOrig, out info))
            {
                foreach (var t in info)
                {
                    tag(hDest, t);
                }
            }
        }
    }
}
