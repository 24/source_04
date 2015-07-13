using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Xml.XPath;
using System.Xml.Linq;
using pb;

// from WCF CORS (plus JSON & REST) - Complete Example http://www.productiverage.com/wcf-cors-plus-json-rest-complete-example

namespace Test_wcf_service
{
    public class CorsOptionsMessage
    {
    }

    public class CorsEnablingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        private CorsHeaders _corsHeaders = null;

        public CorsEnablingBehavior()
        {
            _corsHeaders = CorsHeaders.CurrentCorsHeaders;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsMessageInspector(_corsHeaders));
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override Type BehaviorType { get { return typeof(CorsEnablingBehavior); } }

        protected override object CreateBehavior()
        {
            return new CorsEnablingBehavior();
        }
    }

    public class CorsMessageInspector : IDispatchMessageInspector
    {
        private static bool __trace = false;
        private CorsHeaders _corsHeaders = null;

        public CorsMessageInspector(CorsHeaders corsHeaders)
        {
            _corsHeaders = corsHeaders;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (__trace)
            {
                pb.Trace.WriteLine("AfterReceiveRequest :");
                TraceMessage(request);
                pb.Trace.WriteLine();
            }

            if (request.Properties.ContainsKey("httpRequest"))
            {
                HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)request.Properties["httpRequest"];
                if (httpRequest.Method == "OPTIONS")
                {
                    if (__trace)
                    {
                        pb.Trace.WriteLine("  receive options request activate cors");
                        pb.Trace.WriteLine();
                    }
                    return new CorsOptionsMessage();
                }
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (__trace)
                pb.Trace.WriteLine("BeforeSendReply :");
            if (reply.Properties.ContainsKey("httpResponse"))
            {
                HttpResponseMessageProperty httpResponse = (HttpResponseMessageProperty)reply.Properties["httpResponse"];

                if (_corsHeaders != null)
                {
                    foreach (var header in _corsHeaders.AllMessageHeaders)
                        httpResponse.Headers.Add(header.Key, header.Value);
                }

                if (__trace)
                {
                    if (_corsHeaders != null)
                        pb.Trace.WriteLine("  add cors headers to reply");
                    else
                        pb.Trace.WriteLine("  no cors headers to add to reply");
                    pb.Trace.WriteLine();
                }

                if (correlationState is CorsOptionsMessage)
                {
                    if (__trace)
                    {
                        pb.Trace.WriteLine("  cors activated :");
                        pb.Trace.WriteLine("    add options cors headers to reply");
                        pb.Trace.WriteLine("    change reply status to ok");
                        pb.Trace.WriteLine();
                    }

                    foreach (var header in _corsHeaders.OptionsMessageHeaders)
                        httpResponse.Headers.Add(header.Key, header.Value);

                    httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }

            if (__trace)
            {
                TraceMessage(reply);
                pb.Trace.WriteLine();
            }
        }

        private static void TraceMessage(Message message)
        {
            //foreach (KeyValuePair<string, object> keyvalue in message.Properties)
            //    pb.Trace.WriteLine("  key \"{0}\" type \"{1}\" value \"{2}\"", keyvalue.Key, keyvalue.Value.GetType().FullName, keyvalue.Value);

            Uri requestUri = message.Properties.ContainsKey("Via") ? (Uri)message.Properties["Via"] : null;
            HttpRequestMessageProperty httpRequest = message.Properties.ContainsKey("httpRequest") ? (HttpRequestMessageProperty)message.Properties["httpRequest"] : null;
            if (requestUri != null || httpRequest != null)
            {
                pb.Trace.WriteLine("  HttpRequest :");
                pb.Trace.WriteLine("    Url {0}", requestUri);
                if (httpRequest != null)
                {
                    pb.Trace.WriteLine("    Method {0}", httpRequest.Method);
                    foreach (string header in httpRequest.Headers)
                        pb.Trace.WriteLine("    header \"{0}\" = \"{1}\"", header, httpRequest.Headers[header]);
                }
            }

            if (message.Properties.ContainsKey("httpResponse"))
            {
                HttpResponseMessageProperty httpResponse = (HttpResponseMessageProperty)message.Properties["httpResponse"];
                pb.Trace.WriteLine("  httpResponse :");
                pb.Trace.WriteLine("    StatusCode {0} - {1}", (int)httpResponse.StatusCode, httpResponse.StatusCode);
                pb.Trace.WriteLine("    StatusDescription {0}", httpResponse.StatusDescription);
                foreach (string header in httpResponse.Headers)
                    pb.Trace.WriteLine("    header \"{0}\" = \"{1}\"", header, httpResponse.Headers[header]);
            }
        }
    }
}
