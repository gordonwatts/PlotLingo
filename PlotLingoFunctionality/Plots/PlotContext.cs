using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ROOTNET.Interface;
using PlotLingoLib.Functions;

namespace PlotLingoFunctionality.Plots
{
    public class PlotContext : DrawingContext, IPlotScriptResult
    {
        /// <summary>
        /// The list of plots we are most interested in. This, basically, represents a gPad...
        /// </summary>
        private ROOTNET.NTH1[] _plots;

        /// <summary>
        /// Create the plot context with a list of plots
        /// </summary>
        /// <param name="nTH1"></param>
        public PlotContext(ROOTNET.NTH1[] nTH1)
        {
            this._plots = nTH1;
        }

        /// <summary>
        /// Get the list of plots
        /// </summary>
        public ROOTNET.NTH1[] Plots { get { return _plots; } }

        /// <summary>
        /// Cache the x axis title
        /// </summary>
        private string _yaxisTitle = null;

        /// <summary>
        /// Cache the y axis title
        /// </summary>
        private string _xaxisTitle = null;

        /// <summary>
        /// Get the default title for this plot.
        /// </summary>
        /// <returns></returns>
        protected override string DefaultTitle()
        {
            if (this._plots == null || _plots.Length == 0)
            {
                return "Plot";
            }
            else
            {
                return _plots[0].Title;
            }
        }

        /// <summary>
        /// Atlter the y axis of the canvas.
        /// </summary>
        /// <param name="yaxisName"></param>
        /// <returns></returns>
        public PlotContext yaxis(string yaxisName)
        {
            _yaxisTitle = yaxisName;
            return this;
        }

        /// <summary>
        /// Alter the x axis title.
        /// </summary>
        /// <param name="xaxisName"></param>
        /// <returns></returns>
        public PlotContext xaxis(string xaxisName)
        {
            _xaxisTitle = xaxisName;
            return this;
        }

        /// <summary>
        /// Abstract away various common drawing objects.
        /// </summary>
        class DrawingObject : DrawingContext.IDrawingObjects
        {
            private NTH1 _p;

            public DrawingObject (ROOTNET.Interface.NTH1 p)
            {
                _p = p;
            }
            public short LineColor
            {
                get { return _p.LineColor; }
                set { _p.LineColor = value; _p.MarkerColor = value; }
            }

            public short LineWidth
            {
                get { return _p.LineWidth; }
                set { _p.LineWidth = value; }
            }

            public short LineStyle
            {
                get { return _p.LineStyle; }
                set { _p.LineStyle = value; }
            }

            public NTObject NTObject
            {
                get { return _p; }
            }

            public string Title
            {
                get { return _p.Title; }
            }

            public bool hasTag(IScopeContext ctx, string tagname)
            {
                return Tags.hasTag(ctx, _p, tagname);
            }

            /// <summary>
            /// Get and set default drawing options
            /// </summary>
            public string DrawingOptions
            {
                get { return _p.Option; }
                set { _p.Option = value; }
            }

        }

        /// <summary>
        /// Return a simple interface for objects
        /// </summary>
        public override IEnumerable<IDrawingObjects> ObjectsToDraw
        {
            get { return _plots.Select(p => new DrawingObject(p)); }
        }

        private static int _canvasIndex = 0;

        /// <summary>
        /// Save the plot to output. This is where we do the heavy lifting of generating a plot.
        /// </summary>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> Save(IScopeContext ctx, string filenameStub)
        {
            // Now, do the pre-plot hook
            foreach (var act in _prePlotHook)
            {
                act(ctx, this);
            }

            // Initialize the canvas
            ROOTNET.NTCanvas c = null;
            _canvasIndex++;
            c = new ROOTNET.NTCanvas($"c{_canvasIndex}", GetTitle());

            if (_plots.Length > 0)
                _plots[0].Title = GetTitle();


            // x and y axis titles
            if (_plots.Length > 0)
            {
                if (_xaxisTitle != null)
                    _plots[0].Xaxis.Title = _xaxisTitle;
                if (_yaxisTitle != null)
                    _plots[0].Yaxis.Title = _yaxisTitle;
            }

            // Plot everything.
            var optS = _drawOptions;
            foreach (var p in _plots)
            {
                var perHistoOps = p.Option;
                p.Draw(optS + " " + perHistoOps);
                optS = _drawOptions + "SAME";
            }

            // And post-process
            foreach (var a in _postPlotHook)
            {
                a(ctx, this, c);
            }

            // Save it
            string[] formats = null;
            var formatsExpr = ctx.GetVariableValue("plotformats") as object[];
            if (formatsExpr != null)
                formats = formatsExpr.Cast<string>().ToArray();
            if (formats == null)
                formats = new string[] { "png" };

            var finfos = formats
                .Select(fmt =>
                {
                    var fout = new FileInfo(string.Format("{0}.{1}", filenameStub, fmt));
                    if (fout.Exists)
                    {
                        try
                        {
                            fout.Delete();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Had trouble removing the old file {0}", fout.FullName);
                            Console.WriteLine("  -> {0}", e.Message);
                        }
                    }
                    c.SaveAs(fout.FullName);
                    return fout;
                });
            return finfos.ToArray();
        }
    }
}
