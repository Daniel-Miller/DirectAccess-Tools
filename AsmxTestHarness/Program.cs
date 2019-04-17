using System;
using System.IO;
using System.Net;

using AsmxTestHarness.Tests.Json;
using AsmxTestHarness.Tests.Xml;

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

            // Execute the tests for published methods that require an XML input.
            var r1 = new XmlTestRunner(settings);
            r1.Execute();
            
            // Execute the tests for unpublished methods that return JSON outputs.
            var r2 = new JsonTestRunner(settings);
            r2.Execute();
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