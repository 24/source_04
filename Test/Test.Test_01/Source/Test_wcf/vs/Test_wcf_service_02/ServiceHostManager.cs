using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace Test_wcf_service
{
    public class ServiceHostManager
    {
        private static bool __trace = false;
        private static bool __configureService = false;
        private ServiceHost _serviceHost = null;

        public void StartService()
        {
            StopService();
            Trace.WriteLine("create ServiceHost(typeof(Service1))");

            if (!__configureService)
                _serviceHost = new ServiceHost(typeof(Service1));
            else
            {
                _serviceHost = new ServiceHost(typeof(Service1), new Uri("http://localhost:8701/Test_wcf_service/"));

                WebHttpBinding webHttpBinding = new WebHttpBinding();
                webHttpBinding.CrossDomainScriptAccessEnabled = true;
                ServiceEndpoint serviceEndpoint = _serviceHost.AddServiceEndpoint(typeof(Service1), webHttpBinding, "service1");
                serviceEndpoint.Behaviors.Add(new WebHttpBehavior());
                //serviceEndpoint.Behaviors.Add(new CorsEnablingBehavior());

                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                _serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            if (__trace)
                TraceServiceDescription(_serviceHost.Description);

            Trace.WriteLine("open ServiceHost");
            _serviceHost.Open();
            Trace.WriteLine("service is started");
            Trace.WriteLine();
        }

        public void StopService()
        {
            if (_serviceHost != null)
            {
                if (_serviceHost.State == CommunicationState.Opened || _serviceHost.State == CommunicationState.Faulted || _serviceHost.State == CommunicationState.Opening)
                {
                    Trace.WriteLine("close ServiceHost");
                    _serviceHost.Close();
                    _serviceHost = null;
                    Trace.WriteLine("service is stopped");
                    Trace.WriteLine();
                }
            }
        }

        public static void TraceServiceDescription(ServiceDescription serviceDescription)
        {
            Trace.WriteLine("ServiceDescription :");
            Trace.WriteLine("  Name              : \"{0}\"", serviceDescription.Name);
            Trace.WriteLine("  ConfigurationName : \"{0}\"", serviceDescription.ConfigurationName);
            Trace.WriteLine("  Namespace         : \"{0}\"", serviceDescription.Namespace);
            Trace.WriteLine("  ServiceType       : \"{0}\"", serviceDescription.ServiceType.FullName);
            Trace.WriteLine("  Behaviors count   : {0}", serviceDescription.Behaviors.Count);
            foreach (var behavior in serviceDescription.Behaviors)
            {
                Trace.WriteLine("  Behavior          : \"{0}\"", behavior.GetType().FullName);
            }
            Trace.WriteLine("  Endpoints count   : {0}", serviceDescription.Endpoints.Count);
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                Trace.WriteLine("  Endpoint          : \"{0}\"", endpoint.Name);
                Trace.WriteLine("  Endpoint address  : \"{0}\"", endpoint.Address.Uri);
                Trace.WriteLine("  Endpoint binding name : \"{0}\"", endpoint.Binding.Name);
                Trace.WriteLine("  Endpoint binding namespace : \"{0}\"", endpoint.Binding.Namespace);
                Trace.WriteLine("  Endpoint contract type : \"{0}\"", endpoint.Contract.ContractType.FullName);
                foreach (var operation in endpoint.Contract.Operations)
                {
                    Trace.WriteLine("  Endpoint contract operation name : \"{0}\"", operation.Name);
                    Trace.WriteLine("  Endpoint contract operation begin method : \"{0}\"", operation.BeginMethod != null ? operation.BeginMethod.Name : "null");
                    Trace.WriteLine("  Endpoint contract operation end method : \"{0}\"", operation.EndMethod != null ? operation.EndMethod.Name : "null");
                    Trace.WriteLine("  Endpoint contract operation sync method : \"{0}\"", operation.SyncMethod != null ? operation.SyncMethod.Name : "null");
                    Trace.WriteLine("  Endpoint contract operation sync method : \"{0}\"", operation.TaskMethod != null ? operation.TaskMethod.Name : "null");
                    foreach (var message in operation.Messages)
                    {
                        Trace.WriteLine("  Endpoint contract operation message action : \"{0}\"", message.Action);
                        Trace.WriteLine("  Endpoint contract operation message body : \"{0}\"", message.Body);
                        Trace.WriteLine("  Endpoint contract operation message direction : \"{0}\"", message.Direction);
                    }
                    foreach (var operationBehavior in operation.OperationBehaviors)
                    {
                        Trace.WriteLine("  Endpoint contract operation behavior : \"{0}\"", operationBehavior.GetType().FullName);
                    }
                }
                
                
            }
        }
    }

    //public class ExtendedHostFactory : WebServiceHostFactory
    //{
    //    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    //    {
    //        System.ServiceModel.ServiceHost host = base.CreateServiceHost(serviceType, baseAddresses) as WebServiceHost;
    //        //host.Description.Behaviors.Add(new ValidateApiKey()); // ValidateApiKey is an IServiceBehavior
    //        host.Description.Behaviors.Add(new ValidateApiKey()); // ValidateApiKey is an IServiceBehavior
    //    }
    //}
}
