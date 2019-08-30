using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace firstAddOn_ArcgisPro.UIButton
{
    internal class BtnTestLogging : Button
    {
        protected override void OnClick()
        {
            Module1.Current.ModuleLogManager.LogDebug("Test debug");
            Module1.Current.ModuleLogManager.LogError("Test error log");
            Module1.Current.ModuleLogManager.LogInfo("Test infoLog");
            Module1.Current.ModuleLogManager.LogWarn("Test warning log");
        }
    }
}
