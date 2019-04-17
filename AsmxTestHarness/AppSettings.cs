using System.Configuration;

namespace AsmxTestHarness
{
    public class AppSettings
    {
        public string TestFolder => ConfigurationManager.AppSettings["DirectAccess.TestFolder"];

        public string AsmxEndpointUrl => ConfigurationManager.AppSettings["DirectAccess.AsmxEndpointUrl"];
        public string AsmxUserName => ConfigurationManager.AppSettings["DirectAccess.AsmxUserName"];
        public string AsmxPassword => ConfigurationManager.AppSettings["DirectAccess.AsmxPassword"];
        
        public string AshxEndpointUrl => ConfigurationManager.AppSettings["DirectAccess.AshxEndpointUrl"];
        public string AshxAuthorizationToken => ConfigurationManager.AppSettings["DirectAccess.AshxAuthorizationToken"];
    }
}