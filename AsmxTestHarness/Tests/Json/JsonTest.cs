using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsmxTestHarness.Tests.Json
{
    /// <summary>
    /// This class represents a specific test case for a specific web services method.
    /// </summary>
    public class JsonTest
    {
        private readonly AppSettings _settings;

        public JsonTest(string methodName, string inputPath, AppSettings settings)
        {
            MethodName = methodName;
            InputPath = inputPath;
            _settings = settings;
        }

        public string MethodName { get; }

        public string InputPath { get; }

        public string MethodData => File.ReadAllText(InputPath);
        
        public string TestFolder
        {
            get
            {
                var path = Path.GetDirectoryName(InputPath);
                if (path != null) 
                    return path;

                throw new FileNotFoundException("The directory name could not be determined for the input path (" + InputPath + ")");
            }
        }

        public string OutputPath => Path.Combine(TestFolder, "ActualOutput.json");

        public string ErrorPath => Path.Combine(TestFolder, "Error.txt");

        /// <summary>
        /// Invokes this method and writes the result to the OutputPath. If an exception occurs
        /// then the error message is written to the ErrorPath.
        /// </summary>
        public void Execute(Func<string, string, AppSettings, string> invoke)
        {
            var directory = new DirectoryInfo(TestFolder);

            var watch = Stopwatch.StartNew();

            Tracer.Info($"Starting {MethodName} {directory.Name} ...");

            try
            {
                // If there is an existing Error file and/or an existing Actual Output file
                // from a previous test run then delete it.

                if (File.Exists(ErrorPath)) File.Delete(ErrorPath);
                if (File.Exists(OutputPath)) File.Delete(OutputPath);

                // Invoke the web method
                var result = invoke(MethodName, MethodData, _settings);

                // Write the actual output to a file so we can compare to the expected output
                
                // *** We can't deserialize, parse, or format the output because it might not be valid JSON.
                var parsedJson = JToken.Parse(result);
                var beautified = parsedJson.ToString(Formatting.Indented);
                File.WriteAllText(OutputPath, beautified);

                // *** So we can only dump the raw output in the meantime
                // File.WriteAllText(OutputPath, result);
            }
            catch (WebException ex)
            {
                File.WriteAllText(ErrorPath, ex.Message);
                Tracer.Info($"    *** ERROR. {ex.Message}");
            }

            watch.Stop();

            var elapsed = $"    ... Completed. Elapsed Time = {watch.ElapsedMilliseconds:n0} ms";

            Tracer.Info(elapsed);
        }
    }
}