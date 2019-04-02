using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace AsmxTestHarness
{
    internal class Program
    {
        private static void Main()
        {
            // Get application settings from App.config
            var settings = new AppSettings();

            // Setup tracing
            Tracer.SourceName = "AsmxTestHarness";
            Tracer.TraceFilePath = GetTraceFilePath(settings);

            // Ignore warnings related to expired self-signed SSL certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Load the list of test cases
            var tests = LoadTests(settings);

            // Execute each test case
            foreach (var test in tests)
                test.Execute(Invoke);

            // TODO: Compare the actual outputs with the expected outputs
        }

        /// <summary>
        /// Loads a collection of pre-defined test cases from the file system.
        /// </summary>
        private static List<Test> LoadTests(AppSettings settings)
        {
            var list = new List<Test>();

            var directory = new DirectoryInfo(settings.TestFolder);

            var names = directory.GetDirectories();
            foreach (var name in names)
            {
                var tests = name.GetDirectories();
                foreach (var test in tests)
                {
                    var item = new Test(name.Name, Path.Combine(name.FullName, test.Name, "Input.xml"), settings);
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Invokes a web service method.
        /// </summary>
        private static string Invoke(string methodName, string methodData, AppSettings settings)
        {
            // Create an HTTP web request to the web service method
            var request = (HttpWebRequest) WebRequest.Create(settings.EndpointUrl + "/" + methodName);

            // Add the parameters as key-value pairs with URL encoding
            var encoding = new ASCIIEncoding();
            var postData = encoding.GetBytes("UserName=" 
                                             + settings.UserName
                                             + "&Password=" + settings.Password 
                                             + "&xml=" + HttpUtility.UrlEncode(methodData, Encoding.ASCII));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = postData.Length;

            // Convert the request to a stream and submit it
            var requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            // Get the response from the web server and read it back into a string variable
            var response = (HttpWebResponse) request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) throw new WebException("Failed to get a response stream from the HTTP response");
            var streamReader = new StreamReader(responseStream, Encoding.ASCII);
            var result = streamReader.ReadToEnd();
            response.Close();
            streamReader.Close();
            
            // Return the result to the caller
            return result;
        }

        private static string GetTraceFilePath(AppSettings settings)
        {
            var path = settings.TestFolder;

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory Not Found: {path}");

            return Path.Combine(path, $"Trace {DateTime.Today:yyyy-MM-dd}.txt");
        }
    }
}