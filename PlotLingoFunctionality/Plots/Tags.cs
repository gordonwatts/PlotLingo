
using PlotLingoLib;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Help deal with tags that can be associated with objects
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class Tags : IFunctionObject, IFunctionObjectInitalization
    {
        /// <summary>
        /// We establish the _tagInfo object in the right place initally.
        /// </summary>
        public void Initalize(IScopeContext rootContext)
        {
            rootContext.AddInternalVariable("_tagInfo", new Dictionary<object, List<string>>());
        }

        /// <summary>
        /// Get the tag info dictionary.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static Dictionary<object, List<string>> GetTagInfo(IScopeContext ctx)
        {
            var r = ctx.GetInternalVariable("_tagInfo") as Dictionary<object, List<string>>;
            if (r != null)
                return r;
            r = new Dictionary<object, List<string>>();
            ctx.AddInternalVariable("_tagInfo", r);
            return r;
        }

        /// <summary>
        /// Tag a particular object with some string
        /// </summary>
        /// <param name="objToTag"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public static object tag(IScopeContext ctx, object objToTag, string tagname)
        {
            List<string> info = null;
            var tinfo = GetTagInfo(ctx);
            if (!tinfo.TryGetValue(objToTag, out info))
            {
                info = new List<string>();
                tinfo[objToTag] = info;
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
        public static bool hasTag(IScopeContext ctx, object obj, string tagname)
        {
            List<string> info = null;
            var tinfo = GetTagInfo(ctx);
            if (!tinfo.TryGetValue(obj, out info))
                return false;
            return info.Contains(tagname);
        }

        /// <summary>
        /// Copy tags over from one to the other
        /// </summary>
        /// <param name="h"></param>
        /// <param name="clone"></param>
        internal static void CopyTags(IScopeContext ctx, object hOrig, object hDest)
        {
            List<string> info = null;
            var tinfo = GetTagInfo(ctx);
            if (tinfo.TryGetValue(hOrig, out info))
            {
                foreach (var t in info)
                {
                    tag(ctx, hDest, t);
                }
            }
        }
    }
}
