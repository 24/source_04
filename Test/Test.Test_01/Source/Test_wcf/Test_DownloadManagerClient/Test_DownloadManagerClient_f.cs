using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using MyDownloader.Service;
using pb;

namespace Test.Test_DownloadManagerClient
{
    public static class Test_DownloadManagerClient
    {
        public static void Test_DownloadManagerClient_01()
        {
            Trace.WriteLine("Test_DownloadManagerClient_01");
            //string address = "http://localhost:8019/WCFDownloadManager";
            string address = "http://localhost:8019/DownloadManagerService";
            Trace.WriteLine("address : \"{0}\"", address);
            Trace.WriteLine("ContractDescription contractDescription = ContractDescription.GetContract(typeof(IDownloadManagerService))");
            ContractDescription contractDescription = ContractDescription.GetContract(typeof(IDownloadManagerService));
            WSHttpBinding binding = new WSHttpBinding();
            Trace.WriteLine("EndpointAddress endpointAddress = new EndpointAddress(new Uri(address))");
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));
            Trace.WriteLine("ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription, binding, endpointAddress)");
            ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription, binding, endpointAddress);
            Trace.WriteLine("ChannelFactory<IDownloadManagerService> channelFactory = new ChannelFactory<IDownloadManagerService>(endpoint)");
            ChannelFactory<IDownloadManagerService> channelFactory = new ChannelFactory<IDownloadManagerService>(endpoint);
            Trace.WriteLine("IDownloadManagerService service = channelFactory.CreateChannel()");
            IDownloadManagerService service = channelFactory.CreateChannel();
            int nb = service.GetDownloadCount();
            Trace.WriteLine("service.GetDownloadCount() : {0}", nb);
            Trace.WriteLine("channelFactory.Close()");
            channelFactory.Close();
        }
    }
}
