using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace GeneralTestConsoleApp
{
    class WebRequestCallTest
    {
        public static void Main(string[] args)
        {
            string ModuleLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dataJson = Path.Combine(ModuleLocation, "data.json");

            string dataContent = File.ReadAllText(Path.Combine(ModuleLocation, "data.json"));
        }

        private static void PerformWebRequest(string url, string method, string basicauth, string username, string password, Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            request.KeepAlive = true;
            request.Method = method;

            request.AllowAutoRedirect = true;
            request.CookieContainer = new System.Net.CookieContainer();

            if (basicauth.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                //CredentialCache credentialCache = new CredentialCache();
                //credentialCache.Add(new System.Uri(url), "Basic", new NetworkCredential(username, password));
                //request.Credentials = new NetworkCredential(username, password);
                //request.PreAuthenticate = true;
                //r = requests.get('<MY_URI>', headers ={ 'Authorization': 'username:password'}) //In Python
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }
            //request.ContentType = "application/json";
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                StringBuilder paramString = new StringBuilder();
                paramString.Append($"username={username}&");
                paramString.Append($"password={password}");
                //paramString.Append($"referer=https://kgpmap.wde.woodside.com.au");

                byte[] data = Encoding.ASCII.GetBytes(paramString.ToString());
                request.ContentLength = data.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                request.ContentType = "application/x-www-form-urlencoded";
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseStr = getStringFromWebResponse(response);
            string outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            outputPath = Path.Combine(outputPath, "output.txt");
            Console.WriteLine($"File writing into : {outputPath}");
            File.WriteAllText(outputPath, responseStr);
        }
        private static void PerformPostRequest(string url)
        {
            string gpUrl = "xxxxx//GPServer/GPTaskName";
            string requestUrl = gpUrl + "/execute";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(gpUrl));
            
            request.KeepAlive = true;
            request.Method = "POST";

            request.AllowAutoRedirect = true;
            request.CookieContainer = new System.Net.CookieContainer();


            request.ContentType = "application/json";
            StringBuilder paramString = new StringBuilder();
           
            /**
             * 
             *  var params = {
                request: dictstring
            };
            **/
            string requestParamString = "[your json request param string]";
            paramString.Append($"request={requestParamString}");
            paramString.Append($"&f=json");//append like that if you need additional param //f=json to get as json response string, and you can do the same for additional gptask param value
        

            byte[] data = Encoding.ASCII.GetBytes(paramString.ToString());
            request.ContentLength = data.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
           

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseStr = getStringFromWebResponse(response);
            Console.WriteLine(responseStr);
        }



        public static string getStringFromWebResponse(WebResponse response)
        {
            // Get the stream containing content returned by the server.
            Stream responseDataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(responseDataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            responseDataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }
}
