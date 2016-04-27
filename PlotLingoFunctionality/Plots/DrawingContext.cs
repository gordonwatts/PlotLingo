using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Generic class that is a base class for all context's - to make life easier.
    /// </summary>
    public abstract class DrawingContext
    {
        /// <summary>
        /// Track a property bag for things that others want to associated with this plot context.
        /// </summary>
        private Dictionary<string, object> _prop = new Dictionary<string, object>();

        /// <summary>
        /// Grab a property from the plot context.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(string name)
        {
            object r;
            if (_prop.TryGetValue(name, out r))
            {
                return r;
            }
            return null;
        }

        /// <summary>
        /// Save a property in the plot contex.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        public void SetProperty(string name, object o)
        {
            _prop[name] = o;
        }

        /// <summary>
        /// Objects that you want to draw must implement this light wrapper
        /// </summary>
        public interface IDrawingObjects
        {
            /// <summary>
            /// Get the title of the object
            /// </summary>
            string Title { get; }

            /// <summary>
            /// Get/set the line color of the object
            /// </summary>
            short LineColor { get; set; }

            /// <summary>
            /// Get/Set the line width
            /// </summary>
            short LineWidth { get; set; }

            /// <summary>
            /// Return true if the object we are holding has this tag.
            /// </summary>
            /// <param name="ctx"></param>
            /// <param name="tagname"></param>
            /// <returns></returns>
            bool hasTag(IScopeContext ctx, string tagname);

            /// <summary>
            /// Get the nt object if needed.
            /// </summary>
            ROOTNET.Interface.NTObject NTObject { get; }
        }

        /// <summary>
        /// Get a list of all objects that are in here.
        /// </summary>
        public abstract IEnumerable<IDrawingObjects> ObjectsToDraw
        {
            get;
        }

        /// <summary>
        /// Contains the list of actions to be executed before an actual plot is made.
        /// These are run just before things are dumped out.
        /// </summary>
        protected List<Action<IScopeContext, DrawingContext>> _prePlotHook = new List<Action<IScopeContext, DrawingContext>>();

        /// <summary>
        /// Track all the things we should call once the plotting is, basically, done.
        /// </summary>
        protected List<Action<DrawingContext, ROOTNET.Interface.NTCanvas>> _postPlotHook = new List<Action<DrawingContext, ROOTNET.Interface.NTCanvas>>();

        /// <summary>
        /// Add a pre-plot hook
        /// </summary>
        /// <param name="act"></param>
        public void AddPreplotHook(Action<DrawingContext> act)
        {
            _prePlotHook.Add((c, p) => act(p));
        }

        /// <summary>
        /// Add a pre-plot hook that needs a context
        /// </summary>
        /// <param name="act"></param>
        public void AddPreplotHook(Action<IScopeContext, DrawingContext> act)
        {
            _prePlotHook.Add(act);
        }

        /// <summary>
        /// Add a hook to be called after the basic plotting is done.
        /// </summary>
        /// <param name="act"></param>
        public void AddPostplotHook(Action<DrawingContext, ROOTNET.Interface.NTCanvas> act)
        {
            _postPlotHook.Add(act);
        }
    }
}
