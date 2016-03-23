using System.Net;

namespace Download.Print
{
    public static class TestCertificate
    {
        public static void Init()
        {
            ServicePointManager.ServerCertificateValidationCallback +=  (sender, cert, chain, sslPolicyErrors) => true;
        }
    }
}
