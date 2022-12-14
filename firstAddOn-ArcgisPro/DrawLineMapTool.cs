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
using firstAddOn_ArcgisPro.UI;

namespace firstAddOn_ArcgisPro
{
    public class DrawLineMapTool : MapTool
    {      
        public delegate void DrawLineSketchComplete(Geometry geometry);
        public event DrawLineSketchComplete OnDrawLineSketchComplete;


        public DrawLineMapTool()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Line;
            SketchOutputMode = SketchOutputMode.Map;
            DockpaneCalDistanceViewModel.Show();
        }

        protected override Task OnToolActivateAsync(bool active)
        {

            return base.OnToolActivateAsync(active);
        }

        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            OnDrawLineSketchComplete(geometry);
            return base.OnSketchCompleteAsync(geometry);
        }
    }
}
