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
    public class GraphContext : IPlotScriptResult
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

        public GraphContext logx(bool doit = true)
        {
            _logx = doit;
            return this;
        }
        public GraphContext logy(bool doit = true)
        {
            _logy = doit;
            return this;
        }

        /// <summary>
        /// Contains the list of actions to be executed before an actual plot is made.
        /// These are run just before things are dumped out.
        /// </summary>
        private List<Action<IScopeContext, GraphContext>> _prePlotHook = new List<Action<IScopeContext, GraphContext>>();

        /// <summary>
        /// Track all the things we should call once the plotting is, basically, done.
        /// </summary>
        private List<Action<GraphContext, ROOTNET.Interface.NTCanvas>> _postPlotHook = new List<Action<GraphContext, ROOTNET.Interface.NTCanvas>>();

        /// <summary>
        /// Add a pre-plot hook
        /// </summary>
        /// <param name="act"></param>
        public void AddPreplotHook(Action<GraphContext> act)
        {
            _prePlotHook.Add((c, p) => act(p));
        }

        /// <summary>
        /// Add a pre-plot hook that needs a context
        /// </summary>
        /// <param name="act"></param>
        public void AddPreplotHook(Action<IScopeContext, GraphContext> act)
        {
            _prePlotHook.Add(act);
        }

        /// <summary>
        /// Add a hook to be called after the basic plotting is done.
        /// </summary>
        /// <param name="act"></param>
        public void AddPostplotHook(Action<GraphContext, ROOTNET.Interface.NTCanvas> act)
        {
            _postPlotHook.Add(act);
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