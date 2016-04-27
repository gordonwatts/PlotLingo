using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ROOTNET.Interface;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Generate plots for various graphs
    /// </summary>
    public class GraphContext : DrawingContext, IPlotScriptResult
    {
        NTGraph[] _g;
        string _xaxisTitle = "";
        string _yaxisTitle = "";
        string _title = "";
        bool _logx = false;
        bool _logy = false;

        /// <summary>
        /// Initialize with a full list of graphs
        /// </summary>
        /// <param name="nTGraph"></param>
        public GraphContext(NTGraph[] nTGraph)
        {
            _g = nTGraph;
        }

        public GraphContext(NTGraph g)
        {
            _g = new NTGraph[] { g };
        }

        /// <summary>
        /// Get the name of the plot for drawing
        /// </summary>
        public string Name
        {
            get { return string.IsNullOrWhiteSpace(_title) ? _g[0].Name : _title; }
        }

        /// <summary>
        /// Get at a graf object
        /// </summary>
        class DrawingObject : DrawingContext.IDrawingObjects
        {
            NTGraph _g;

            public DrawingObject (ROOTNET.Interface.NTGraph g)
            {
                _g = g;
            }

            public short LineColor
            {
                get { return _g.LineColor; }
                set { _g.LineColor = value; }
            }

            public short LineWidth
            {
                get { return _g.LineWidth; }
                set { _g.LineWidth = value; }
            }

            public NTObject NTObject
            {
                get { return _g; }
            }

            public string Title
            {
                get { return _g.Title; }
            }

            public bool hasTag(IScopeContext ctx, string tagname)
            {
                return Tags.hasTag(ctx, _g, tagname);
            }
        }

        /// <summary>
        /// Return an abstract list of our plotting objects.
        /// </summary>
        public override IEnumerable<IDrawingObjects> ObjectsToDraw
        {
            get { return _g.Select(myg => new DrawingObject(myg)); }
        }

        public GraphContext yaxis(string yaxisName)
        {
            _yaxisTitle = yaxisName;
            return this;
        }

        public GraphContext xaxis(string yaxisName)
        {
            _xaxisTitle = yaxisName;
            return this;
        }

        public GraphContext title(string t)
        {
            _title = t;
            return this;
        }

        /// <summary>
        /// Save to a file.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> Save(IScopeContext ctx, string filenameStub)
        {
            // Do pre-plot actions.
            foreach (var ph in _prePlotHook)
            {
                ph(ctx, this);
            }

            // Initialize the canvas
            var c = new ROOTNET.NTCanvas();
            if (!string.IsNullOrWhiteSpace(_title))
            {
                _g[0].Title = _title;
            }
            c.Title = _g[0].Title;

            c.Logx = _logx ? 1 : 0;
            c.Logy = _logy ? 1 : 0;

            // Plot everything.
            var optS = "";
            foreach (var p in _g)
            {
                if (!string.IsNullOrWhiteSpace(_xaxisTitle))
                {
                    p.Xaxis.Title = _xaxisTitle;
                }
                if (!string.IsNullOrWhiteSpace(_yaxisTitle))
                {
                    p.Yaxis.Title = _yaxisTitle;
                }
                p.Draw(optS);
                optS = "SAME";
            }

            // Post plot actions
            foreach (var ph in _postPlotHook)
            {
                ph(this, c);
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