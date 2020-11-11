using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaLongPaths;

namespace GeneralTestConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            string ModuleLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dataJson = Path.Combine(ModuleLocation, "data.json");

           string dataContent =  File.ReadAllText(Path.Combine(ModuleLocation, "data.json"));
            Rootobject canconvert = JsonConvert.DeserializeObject<Rootobject>(dataContent);
            Console.WriteLine("it can convert");

            var HierarchyPath = new ZlpDirectoryInfo(@"D:\EsriBitBucket\wa-woodside\RestEndPoints\AuthApi\Esri.WinAuthApi.FW\Views\Shared");
            List<string> PathListToFind = new List<string>();
            //string HierarchyPath = DirectoryPath;

            while (!HierarchyPath.FullName.Equals(@"D:\EsriBitBucket", StringComparison.OrdinalIgnoreCase))
            {
                PathListToFind.Add(HierarchyPath.FullName);
                HierarchyPath = HierarchyPath.Parent;
                Console.WriteLine(HierarchyPath.FullName);
            }
            //foreach (var filePath in folderPath.GetFiles())
            //{
            //    Console.Write("File {0} has a size of {1}",
            //        filePath.FullName,
            //        filePath.Length);
            //}

            Console.ReadLine();
        }
    }


    public class Rootobject
    {
        public Seismic Seismic { get; set; }
    }

    public class Seismic
    {
        public int WarningScale { get; set; }
        public List<FeatureClassConfig> LineFeatureClass { get; set; }
        public List<FeatureClassConfig> SurveyFeatureClass { get; set; }
    }

    public class FeatureClassConfig
    {
        public string Name { get; set; }
        public string[] Fields { get; set; }
    }


}
