using System.Configuration;

namespace AsmxTestHarness
{
    public class AppSettings
    {
        public string EndpointUrl => ConfigurationManager.AppSettings["DirectAccess.EndpointUrl"];
        public string UserName => ConfigurationManager.AppSettings["DirectAccess.UserName"];
        public string Password => ConfigurationManager.AppSettings["DirectAccess.Password"];
        public string TestFolder => ConfigurationManager.AppSettings["DirectAccess.TestFolder"];
    }
}