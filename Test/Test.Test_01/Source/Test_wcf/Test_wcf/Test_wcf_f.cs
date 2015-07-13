using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using pb;

namespace Test.Test_wcf
{
    public static class Test_wcf_f
    {
        public static void Test_wcf_01()
        {
            Trace.WriteLine("Test_wcf_01");
            ServiceDescription serviceDescription = ServiceDescription.GetService(typeof(Service1));
            TraceServiceDescription(serviceDescription);
        }

        public static void TraceServiceDescription(ServiceDescription serviceDescription)
        {
            Trace.WriteLine("ServiceDescription :");
            Trace.WriteLine("  Name              : \"{0}\"", serviceDescription.Name);
            Trace.WriteLine("  ConfigurationName : \"{0}\"", serviceDescription.ConfigurationName);
            Trace.WriteLine("  Namespace         : \"{0}\"", serviceDescription.Namespace);
            Trace.WriteLine("  ServiceType       : \"{0}\"", serviceDescription.ServiceType.zGetTypeName());
            Trace.WriteLine("  Behaviors count   : {0}", serviceDescription.Behaviors.Count);
            foreach (var behavior in serviceDescription.Behaviors)
            {
                Trace.WriteLine("  Behavior          : \"{0}\"", behavior.GetType().zGetTypeName());
            }
            Trace.WriteLine("  Endpoints count   : {0}", serviceDescription.Endpoints.Count);
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                Trace.WriteLine("  Endpoint          : \"{0}\"", endpoint.Name);
            }
        }
    }


    [ServiceContract()]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        // result : "toto"
        public string GetString()
        {
            return "toto";
        }
    }
}
