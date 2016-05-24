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
        public static DrawingContext logx(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.Logx = 1; });
            return ctx;
        }
        public static DrawingContext logy(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.Logy = 1; });
            return ctx;
        }
        public static DrawingContext grid(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.Grid = 1; });
            return ctx;
        }
        public static DrawingContext logz(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.Logz = 1; });
            return ctx;
        }

        public static DrawingContext size(IScopeContext c, DrawingContext ctx, int width, int height)
        {
            ctx.AddPostplotHook((myctx, canvas) => { canvas.SetCanvasSize((uint) width, (uint) height); });
            return ctx;
        }
    }
}
