using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using NLog;
using System.IO;
using CrosscutUtility;

namespace firstAddOn_ArcgisPro
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;
        public ILoggerManager ModuleLogManager;
        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// Always one module for each addin
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("firstAddOn_ArcgisPro_Module"));
            }
        }

        public SearchCatalogEditBox RibbonSearchCatalogEditBox { get; set; }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        protected override bool Initialize()
        {
            string ModuleLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string nLogConfigLocation = Path.Combine(ModuleLocation, "nlog.config");
            
            ModuleLogManager = new LoggerManager(nLogConfigLocation);
            ArcGIS.Desktop.Core.Events.ProjectClosingEvent.Subscribe(OnProjectClosing);
            return base.Initialize();
            //return true;
        }
        /// <summary>
        /// Event handler for ProjectClosing event.
        /// </summary>
        /// <param name="args">The ProjectClosing arguments.</param>
        /// <returns></returns>
        private Task OnProjectClosing(ArcGIS.Desktop.Core.Events.ProjectClosingEventArgs args)
        {

            // if already canceled, ignore
            if (args.Cancel)
                return Task.CompletedTask;

            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
            Console.WriteLine(config == null);
            var targetProjFolder = @"c:\temp";
            var result = MessageBox.Show($@"Soll das aktuelle Projekt in ein anderes Verzeichnis als          {targetProjFolder} kopiert werden ?", "Projekt sichern", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
            return Task.CompletedTask;
        }
        #endregion Overrides

    }
}
