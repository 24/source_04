using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace pb.Web
{
    public static class Certificate
    {
        public static void DesactivateServerCertificateValidation()
        {
            // from C# Ignore certificate errors? http://stackoverflow.com/questions/2675133/c-sharp-ignore-certificate-errors
            ServicePointManager.ServerCertificateValidationCallback += ForceCertificateValidation;  // (sender, cert, chain, sslPolicyErrors) => true;
        }

        public static void ActivateServerCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback -= ForceCertificateValidation; // (sender, cert, chain, sslPolicyErrors) => true;
        }

        private static bool ForceCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
