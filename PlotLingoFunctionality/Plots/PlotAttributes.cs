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
            ctx.AddPostplotHook((sctx, myctx, canvas) => { canvas.Logx = 1; });
            return ctx;
        }
        public static DrawingContext logy(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((sctx, myctx, canvas) => { canvas.Logy = 1; });
            return ctx;
        }
        public static DrawingContext grid(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((sctx, myctx, canvas) => { canvas.Grid = 1; });
            return ctx;
        }
        public static DrawingContext logz(IScopeContext c, DrawingContext ctx)
        {
            ctx.AddPostplotHook((sctx, myctx, canvas) => { canvas.Logz = 1; });
            return ctx;
        }

        public static DrawingContext size(IScopeContext c, DrawingContext ctx, int width, int height)
        {
            ctx.AddPostplotHook((ssct, myctx, canvas) => { canvas.SetCanvasSize((uint) width, (uint) height); });
            return ctx;
        }

        /// <summary>
        /// Set the default canvas size when we create a new canvas.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void set_default_canvas_size(int width, int height)
        {
            DrawingContext.DefaultCanvasHeight = (uint) height;
            DrawingContext.DefaultCanvasWidth = (uint) width;
        }

        public static DrawingContext drawingOptionAfter(IScopeContext c, DrawingContext ctx, int nskip, string options)
        {
            ctx.AddPreplotHook(dc =>
            {
                int count = 0;
                foreach (var p in dc.ObjectsToDraw)
                {
                    count++;
                    if (count > nskip)
                    {
                        p.DrawingOptions += " " + options;
                    }
                }
            });

            return ctx;
        }
    }
}
