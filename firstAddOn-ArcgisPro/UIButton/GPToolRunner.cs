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
using ArcGIS.Desktop.GeoProcessing;
using ArcGIS.Desktop.Editing.Templates;
using firstAddOn_ArcgisPro.DTO;
using ArcGIS.Core.Internal;
using ArcGIS.Desktop.Core.Geoprocessing;
using System.IO;

namespace firstAddOn_ArcgisPro.UIButton
{
    internal class GPToolRunner : Button
    {        
        protected async override void OnClick()
        {
            try
            {

                //string toolboxPath = @"D:\RnD_Data\EsriTraining\PythonGP\Scripts\CreateFCAndFields.pyt\CreateFeatureClassAndFields";
                string toolboxPath = @"D:\RnD_Data\EsriTraining\PythonGP\ScriptTool\CreateFCAndFields.tbx\CreateFCAndFields";
                string gdbPath = @"D:\RnD_Data\EsriTraining\PythonGP\Data\SanJuan.gdb";
                
                string fcName = "TempFeature_" + DateTime.Now.ToString("yyyyMMddHHmmssf");
                string geometryType = "POINT";
                string hasZValue = "DISABLED";
                //SpatialReference outSR = SpatialReferenceBuilder.CreateSpatialReference(4326);
                int outSR = 4326;
                List<FieldInfo> fieldInfoList = new List<FieldInfo>();
                fieldInfoList.Add(new FieldInfo()
                {
                    aliasName = "Home",
                    name = "Home",
                    fieldType = (int)FieldType.String,
                    length = 255
                });
                fieldInfoList.Add(new FieldInfo()
                {
                    aliasName = "Created By",
                    name = "Created By",
                    fieldType = (int)FieldType.String,
                    length = 255
                });
                fieldInfoList.Add(new FieldInfo()
                {
                    aliasName = "Age",
                    name = "Age",
                    fieldType = (int)FieldType.Integer
                });
                fieldInfoList.Add(new FieldInfo()
                {
                    aliasName = "Updated Date",
                    name = "Updated Date",
                    fieldType = (int)FieldType.Date
                });
                bool result = false;
                string message = "";
                string strFieldInfoList = "[['school_name', 'TEXT', 'Name', 255, 'Hello world', ''],"+
                                    "['street_number', 'LONG', 'Street Number', None, 35, 'StreetNumDomain'],"+
                                    "['year_start', 'DATE', 'Year Start', None, '2017-08-09 16:05:07', '']]";
                List<List<string>> FieldInfoList = new List<List<string>>()
                {
                    new List<string>(){ "school_name", "TEXT", "Name", "255", "Hello world", "" },
                    new List<string>(){ "year_start", "DATE", "Year Start", "None", "2017-08-09 16:05:07", "" }
                };

                //Correct way of import
                List<string> strFieldList = new List<string>()
                {
                    "'school_name' 'TEXT' 'Name' 255 'Hello world' ''",
                    "'year_start' 'DATE' 'Year Start' None '2017-08-09 16:05:07' ''"
                };

                var arguments = Geoprocessing.MakeValueArray(gdbPath,fcName, geometryType, hasZValue, outSR, strFieldList, result, message);
                Geoprocessing.OpenToolDialog(toolboxPath, arguments);
                //var gpResult = Geoprocessing.ExecuteToolAsync(toolboxPath, arguments);
                //gpResult.Wait();
                //IGPResult ProcessingResult = gpResult.Result;
                //if (ProcessingResult.IsFailed)
                //{
                //    string errorMessage = "";
                //    foreach (IGPMessage gpMessage in ProcessingResult.Messages)
                //    {
                //        errorMessage += $"{{Error Code: {gpMessage.ErrorCode}, Text :  {gpMessage.Text} }}";
                //    }
                //    MessageBox.Show($"Geoprocessing fail {errorMessage}");
                //}
                //else
                //{
                //    MessageBox.Show("Process  success");
                 
                //}
                //using (Geodatabase fileGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("dkfs"))))

                //using (FeatureClassDefinition featureClass = fileGeodatabase.GetDefinition<FeatureClassDefinition>("dkfs"))

                //{
                //    this.FieldListToAdd = featureClass.GetFields();
                //}

               
               
            }
            catch(Exception exce)
            {
                MessageBox.Show($"Exception occured {exce.Message}{Environment.NewLine}{exce.StackTrace}");
            }
        }
    }
}
////string ExportTifFile = SourceFileName;
//var parameters = Geoprocessing.MakeValueArray(
//     tempFolderPath, // Output path
//    ExportTifFile, // Raster name : make it as tif file
//     rasterInfo.CellSize.ToString(), // Cellsize
//     this.XYZInputModel.PixelType, // pixel type
//     this._lastSelectedSR, // Spatial reference,
//     1, "", SourceFileName,
//     "128 128", "NONE", ""
//     ); // Bands count

////IReadOnlyList<KeyValuePair<string, string>> environments = Geoprocessing.MakeEnvironmentArray(extent: pEnvelope);
//var gpResult = Geoprocessing.ExecuteToolAsync("CreateRasterDataset_management", parameters, null,
//                    CancelableProgressor.None, GPExecuteToolFlags.GPThread);
//gpResult.Wait();
//                    IGPResult ProcessingResult = gpResult.Result;
//                    if (ProcessingResult.IsFailed)
//                    {
//                        foreach (IGPMessage message in ProcessingResult.Messages)
//                        {
//                            ConversionToolModule.Current.ModuleLogManager.LogError("Error on create raster dataset tool calling");
//                            ConversionToolModule.Current.ModuleLogManager.LogError($"{{Error Code: {message.ErrorCode}, Text :  {message.Text} }}");
//                        }
//                        Geoprocessing.ShowMessageBox(ProcessingResult.Messages, "Raster creation error", GPMessageBoxStyle.Error);
//                    }