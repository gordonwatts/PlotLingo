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
        /// Keep track of our title.
        /// </summary>
        private string _title = null;

        /// <summary>
        /// Cache the x axis title
        /// </summary>
        private string _yaxisTitle = null;

        /// <summary>
        /// Cache the y axis title
        /// </summary>
        private string _xaxisTitle = null;

        /// <summary>
        /// Get the filename.
        /// </summary>
        private string _filename;

        /// <summary>
        /// Drawing options
        /// </summary>
        private string _drawOptions = "";

        /// <summary>
        /// Init the title if it hasn't been already.
        /// </summary>
        private void InitTitleAndFileName()
        {
            if (_title == null)
            {
                if (this._plots == null || _plots.Length == 0)
                {
                    _title = "Plot";
                }
                else
                {
                    _title = _plots[0].Title;
                }
            }

            if (_filename == null)
            {
                _filename = _title;
            }
        }

        /// <summary>
        /// Alter the title of the canvas.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public PlotContext title(string title)
        {
            _title = title;
            return this;
        }

        /// <summary>
        /// List of statements that should be executed after we've built the context.
        /// </summary>
        private List<string> _evalLines = new List<string>();

        /// <summary>
        /// Evaluate a line after we've generated the plot. So you can run a macro
        /// or similar.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public PlotContext eval(string line)
        {
            _evalLines.Add(line);
            return this;
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
        /// Alter the name of the file.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public PlotContext filename(string fname)
        {
            _filename = fname;
            return this;
        }

        /// <summary>
        /// Add a draw option
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public PlotContext addDrawOption(string opt)
        {
            _drawOptions += " " + opt;
            return this;
        }

        /// <summary>
        /// Return the name of this script.
        /// </summary>
        public string Name
        {
            get { InitTitleAndFileName(); return _filename; }
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
                set { _p.LineColor = value; }
            }

            public short LineWidth
            {
                get { return _p.LineWidth; }
                set { _p.LineWidth = value; }
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
            InitTitleAndFileName();

            // Now, do the pre-plot hook
            foreach (var act in _prePlotHook)
            {
                act(ctx, this);
            }

            // Initialize the canvas
            ROOTNET.NTCanvas c = null;
            _canvasIndex++;
            c = new ROOTNET.NTCanvas($"c{_canvasIndex}", _title);

            if (_plots.Length > 0)
                _plots[0].Title = _title;


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
                p.Draw(optS);
                optS = _drawOptions + " SAME";
            }

            // Post-process any eval lines.
            foreach (var l in _evalLines)
            {
                ScriptOperations.eval(ctx, l);
            }

            // And post-process
            foreach (var a in _postPlotHook)
            {
                a(this, c);
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
