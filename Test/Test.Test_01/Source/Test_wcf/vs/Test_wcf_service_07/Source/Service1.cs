using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Test_wcf_service
{
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

        // WebInvokeAttribute
        // [WebInvoke(UriTemplate = "Sub?x={x}&y={y}")]
        // [WebInvoke(Method = "POST", UriTemplate = "Mod?x={x}&y={y}")]
        // UriTemplate and UriTemplateTable https://msdn.microsoft.com/en-us/library/bb675245(v=vs.110).aspx
    }
}
