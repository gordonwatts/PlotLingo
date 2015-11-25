using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Some fun utilities for dealing with plots.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class PlotAttributes : IFunctionObject
    {
        public static PlotContext logy(IScopeContext c, PlotContext ctx)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.Logy = 1; });
            return ctx;
        }
    }
}
