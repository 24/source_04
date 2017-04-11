using CG.Web.MegaApiClient;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace pb.Web.Data.Test
{
    // https://github.com/gpailler/MegaApiClient
    // Install-Package MegaApiClient
    // MegaApiClient, Version=1.3.1.269, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
    // need Newtonsoft.Json, Version=9.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed

    public static class Test_MegaApiClient
    {
        public static void Test_01()
        {
            Trace.WriteLine("Test_MegaApiClient");
        }

        public static void Test_SaveNodes_01(string name)
        {
            MegaApiClient client = new MegaApiClient();
            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            client.Login(email, password);
            Trace.WriteLine("MegaApiClient : GetNodes()");
            var nodes = client.GetNodes();
            Trace.WriteLine("save nodes");
            nodes.zSave(@"c:\pib\_dl\nodes_01.json", jsonIndent: true);
        }

        public static void Test_LoadNodes_01()
        {
            var nodes = zMongo.BsonRead<INode>(@"c:\pib\_dl\nodes_01.json");
            TraceNodes(nodes);
        }

        public static void Test_TraceNodes_01(string name, int limit = 0, bool log = false)
        {
            MegaApiClient client = new MegaApiClient();

            Trace.WriteLine("MegaApiClient : login");

            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            client.Login(email, password);
            Trace.WriteLine("MegaApiClient : GetNodes()");
            var nodes = client.GetNodes();
            Trace.WriteLine($"MegaApiClient : nodes.Count() {nodes.Count()}");
            TraceNodes(nodes, limit: limit, log: log);

            //client.GetAccountInformation();
            //client.GetNodeFromLink(Uri uri);
            //CG.Web.MegaApiClient.NodeType
            //CG.Web.MegaApiClient.WebClient wc;

            //INode root = nodes.Single(n => n.Type == NodeType.Root);

            //INode myFolder = client.CreateFolder("Upload", root);

            //INode myFile = client.UploadFile("MyFile.ext", myFolder);

            //Uri downloadUrl = client.GetDownloadLink(myFile);
            //Console.WriteLine(downloadUrl);
        }

        public static void Test_TraceMegaNodes_01(string name, string directory = "/", NodesOptions options = NodesOptions.Default, int limit = 0, bool log = false)
        {
            MegaClient megaClient = new MegaClient();
            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            megaClient.Email = email;
            megaClient.Password = password;
            megaClient.Login();
            TraceNodes(megaClient.GetNodes(directory, options).Select(node => node.Node), limit: limit, log: log);
        }

        public static void Test_TraceMegaNodes_02(string name, string directory = "/", NodesOptions options = NodesOptions.Default, int limit = 0, bool log = false)
        {
            MegaClient megaClient = new MegaClient();
            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            megaClient.Email = email;
            megaClient.Password = password;
            megaClient.Login();
            TraceNodes(megaClient.GetNodes(directory, options), limit: limit, log: log);
        }

        public static void Test_03(string name)
        {
            MegaApiClient client = new MegaApiClient();
            Trace.WriteLine("MegaApiClient : login");
            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            client.Login(email, password);
            Trace.WriteLine("MegaApiClient : get root");
            var root = client.GetNodes().First(node => node.Type == NodeType.Root);
            Trace.WriteLine("MegaApiClient : GetNodes(root)");
            var nodes = client.GetNodes(root);
            Trace.WriteLine($"MegaApiClient : nodes.Count() {nodes.Count()}");
            TraceNodes(nodes);
        }

        public static void Test_GetMegaNodes_v2(string name)
        {
            MegaClient megaClient = new MegaClient();
            string email, password;
            if (!GetMegaLogin(name, out email, out password))
                return;
            megaClient.Email = email;
            megaClient.Password = password;
            megaClient.Login();
            // .zSave(@"c:\pib\_dl\meganode_01.json", jsonIndent: true)
            //megaClient.GetDictionaryNodes1_v2();
            megaClient.GetMegaNodes().zSave(@"c:\pib\_dl\meganode_02.json", jsonIndent: true);
        }

        //public static string GetNodeTypeShortName(NodeType type)
        //{
        //    switch (type)
        //    {
        //        case NodeType.Directory:
        //            return "D";
        //        case NodeType.File:
        //            return "F";
        //        case NodeType.Root:
        //            return "R";
        //        case NodeType.Inbox:
        //            return "I";
        //        case NodeType.Trash:
        //            return "T";
        //        default:
        //            throw new PBException($"unknow node type {type}");
        //    }
        //}

        public static void TraceNodes(IEnumerable<INode> nodes, int limit = 0, bool log = false)
        {
            try
            {
                if (log)
                {
                    //RunSourceCommand.TraceDisableViewer();
                    //RunSourceCommand.TraceSetWriter(GetLogWriter(), "log");
                    RunSourceCommand.TraceManager.DisableViewer();
                    RunSourceCommand.TraceManager.SetWriter(GetLogWriter(), "log");
                }
                if (limit != 0)
                    nodes = nodes.Take(limit);
                int fileCount = 0;
                int directoryCount = 0;
                Trace.WriteLine("Id       Owner       ParentId Type      LastModificationDate Size           Name");
                foreach (INode node in nodes)
                {
                    //Trace.WriteLine($"Id {node.Id} Name {(node.Name != null ? node.Name : "-null-")} Size {node.Size} LastModificationDate {node.LastModificationDate} Owner {node.Owner} ParentId {(node.ParentId != null ? node.ParentId : "-null-")} Type {node.Type}");
                    //Trace.WriteLine($"Id {node.Id} Owner {node.Owner} ParentId {node.ParentId,-8} Type {node.Type,-9} LastModificationDate {node.LastModificationDate} Size {node.Size,15:### ### ### ###} Name {(node.Name != null ? node.Name : "-null-")}");
                    Trace.WriteLine($"{node.Id} {node.Owner} {node.ParentId,-8} {node.Type,-9} {node.LastModificationDate} {node.Size,15:### ### ### ###} {(node.Name != null ? node.Name : "-null-")}");
                    if (node.Type == NodeType.File)
                        fileCount++;
                    else
                        directoryCount++;
                }
                Trace.WriteLine($"{fileCount} files, {directoryCount} directories, total {fileCount + directoryCount}");
            }
            finally
            {
                if (log)
                {
                    //RunSourceCommand.TraceRemoveWriter("log");
                    //RunSourceCommand.TraceEnableViewer();
                    RunSourceCommand.TraceManager.RemoveWriter("log");
                    RunSourceCommand.TraceManager.EnableViewer();
                }
            }
        }

        public static void TraceNodes(IEnumerable<MegaNode> nodes, int limit = 0, bool log = false)
        {
            try
            {
                if (log)
                {
                    //RunSourceCommand.TraceDisableViewer();
                    //RunSourceCommand.TraceSetWriter(GetLogWriter(), "log");
                    RunSourceCommand.TraceManager.DisableViewer();
                    RunSourceCommand.TraceManager.SetWriter(GetLogWriter(), "log");
                }
                if (limit != 0)
                    nodes = nodes.Take(limit);
                int fileCount = 0;
                int directoryCount = 0;
                Trace.WriteLine("LastModificationDate Size           Path");
                foreach (MegaNode node in nodes)
                {
                    Trace.WriteLine($"{node.Node.LastModificationDate} {node.Node.Size,15:### ### ### ###} {node.Path}");
                    if (node.Node.Type == NodeType.File)
                        fileCount++;
                    else
                        directoryCount++;
                }
                Trace.WriteLine($"{fileCount} files, {directoryCount} directories, total {fileCount + directoryCount}");
            }
            finally
            {
                if (log)
                {
                    //RunSourceCommand.TraceRemoveWriter("log");
                    //RunSourceCommand.TraceEnableViewer();
                    RunSourceCommand.TraceManager.RemoveWriter("log");
                    RunSourceCommand.TraceManager.EnableViewer();
                }
            }
        }

        public static bool GetMegaLogin(string name, out string email, out string password)
        {
            email = password = null;
            XmlConfig config = new XmlConfig(@"c:\pib\drive\google\dev_data\exe\runsource\mega\config\mega.config.local.xml");
            XElement xe = config.GetElement($"Login[@name = '{name}']");
            if (xe != null)
            {
                email = xe.Attribute("email").Value;
                password = xe.Attribute("password").Value;
                return true;
            }
            else
            {
                Trace.WriteLine($"mega login not found \"{name}\"");
                return false;
            }
        }

        public static WriteToFile GetLogWriter()
        {
            return WriteToFile.Create(@"c:\pib\dev_data\exe\runsource\test\_log\_log.txt", FileOption.IndexedFile);
        }
    }
}
