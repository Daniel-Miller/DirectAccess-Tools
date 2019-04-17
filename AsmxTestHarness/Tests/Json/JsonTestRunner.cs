using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AsmxTestHarness.Tests.Json
{
    public class JsonTestRunner
    {
        private readonly AppSettings _settings;

        public JsonTestRunner(AppSettings settings)
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
        private List<JsonTest> LoadTests()
        {
            var list = new List<JsonTest>();

            var path = Path.Combine(_settings.TestFolder, "Json");

            var directory = new DirectoryInfo(path);

            var names = directory.GetDirectories();
            foreach (var name in names)
            {
                var tests = name.GetDirectories();
                foreach (var test in tests)
                {
                    var item = new JsonTest(name.Name, Path.Combine(name.FullName, test.Name, "Input.txt"), _settings);
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Invokes a web service method.
        /// </summary>
        private static string Invoke(string methodName, string methodParameters, AppSettings settings)
        {
            var query = settings.AshxEndpointUrl
                        + "?action="
                        + methodName
                        + "&"
                        + methodParameters;

            // Create an HTTP web request to the web service method
            var request = (HttpWebRequest) WebRequest.Create(query);
            request.Timeout = 500000;

            // Set the Basic Authorization token (if one is provided)
            if (settings.AshxAuthorizationToken != null)
                request.Headers["Authorization"] = "Basic " + settings.AshxAuthorizationToken;

            // Set the HTTP method
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";

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