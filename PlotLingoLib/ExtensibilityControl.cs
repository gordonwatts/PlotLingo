using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib
{
    /// <summary>
    /// Central class to control the extensibility of the PlotLingo
    /// app (loading up function and object method extension providers, etc.
    /// </summary>
    class ExtensibilityControl
    {
        private static ExtensibilityControl _single = new ExtensibilityControl();

        /// <summary>
        /// Get the extensibilty control object singleton.
        /// </summary>
        /// <returns></returns>
        public static ExtensibilityControl Get()
        {
            return _single;
        }

        /// <summary>
        /// Private ctor to make sure no one else can get at it.
        /// </summary>
        private ExtensibilityControl()
        {

        }

        /// <summary>
        /// Hold the parts for compositing here!
        /// </summary>
        private CompositionContainer _container;

        /// <summary>
        /// Init the composition container, and fill our internal lists.
        /// </summary>
        private void Init()
        {
            // We need to do this only once.
            if (_container != null)
                return;

            // Catalog with this assembly and anything else in the same directory
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ExtensibilityControl).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(typeof(ExtensibilityControl).Assembly.Location)));

            // And attach the container to the catalog, and run the composition.

            _container = new CompositionContainer(catalog);
            AttributedModelServices.ComposeParts(_container, this);
        }

#pragma warning disable 0649
        /// <summary>
        /// Internal list of function objects
        /// </summary>
        [ImportMany]
        IEnumerable<IFunctionObject> _functionObjects;
#pragma warning restore 0649

        /// <summary>
        /// Get the list of function objects where functions can be searched for.
        /// </summary>
        public IEnumerable<IFunctionObject> FunctionObjects { get { Init(); return _functionObjects; } }
    }
}
