using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace AsmxTestHarness.Tests.Xml
{
    public class XmlTestRunner
    {
        private readonly AppSettings _settings;

        public XmlTestRunner(AppSettings settings)
        {
            _settings = settings;
        }

        public void Execute()
        {
            // Load the list of test cases
            var tests = LoadTests();

            // Execute each test case
            foreach (var test in tests)
                test.Execute(Invoke);

            // TODO: Compare the actual outputs with the expected outputs
        }

        /// <summary>
        /// Loads a collection of pre-defined test cases from the file system.
        /// </summary>
        private List<XmlTest> LoadTests()
        {
            var list = new List<XmlTest>();

            var path = Path.Combine(_settings.TestFolder, "Xml");

            var directory = new DirectoryInfo(path);

            var names = directory.GetDirectories();
            foreach (var name in names)
            {
                var tests = name.GetDirectories();
                foreach (var test in tests)
                {
                    var item = new XmlTest(name.Name, Path.Combine(name.FullName, test.Name, "Input.xml"), _settings);
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
            var request = (HttpWebRequest) WebRequest.Create(settings.AsmxEndpointUrl + "/" + methodName);

            // Add the parameters as key-value pairs with URL encoding
            var encoding = new ASCIIEncoding();
            var postData = encoding.GetBytes("UserName=" 
                                             + settings.AsmxUserName
                                             + "&Password=" + settings.AsmxPassword 
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
    }
}