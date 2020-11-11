using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebRequestTester
{
    class Program
    {   

        public static void Main(string[] args)
        {
            try
            {


                string url = ConfigurationManager.AppSettings["url"] + string.Empty;
                string method = ConfigurationManager.AppSettings["method"] + string.Empty;
                string basicauth = ConfigurationManager.AppSettings["basicauth"] + string.Empty;
                string username = ConfigurationManager.AppSettings["username"] + string.Empty;
                string password = ConfigurationManager.AppSettings["password"] + string.Empty;

                Console.WriteLine($"Start requesting from {url}");
                Uri uri = new Uri(url);
                PerformWebRequest(url, method, basicauth, username, password, uri);

                Console.WriteLine("process finished successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error occur ");
            }
            Console.WriteLine("Press enter to close");

            Console.ReadLine();
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
            if(method.Equals("POST", StringComparison.OrdinalIgnoreCase))
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
