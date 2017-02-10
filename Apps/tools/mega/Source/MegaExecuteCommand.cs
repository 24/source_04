using CG.Web.MegaApiClient;
using pb;
using pb.Data.Xml;
using pb.Web.Data;
using System;
using System.Collections.Generic;

namespace Mega
{
    [Flags]
    public enum LsOptions
    {
        None      = 0,
        LongView  = 0x0001
    }

    public static class MegaExecuteCommand
    {
        private static MegaEnvironment _environment = null;

        public static void SetLogin(string[] parameters)
        {
            if (parameters.Length != 1)
            {
                Trace.WriteLine("syntax error");
                return;
            }
            //string loginFile = XmlConfig.CurrentConfig.GetExplicit("LocalCurrentLogin");
            //XDocument loginDoc;
            //if (zFile.Exists(loginFile))
            //    loginDoc = XDocument.Load(loginFile);
            //else
            //    loginDoc = new XDocument();
            //loginDoc.zSetValue("Login", name);
            //loginDoc.Save(loginFile);
            GetMegaEnvironment().SetLogin(parameters[0]);
        }

        public static void Cd(string[] parameters)
        {
            if (parameters.Length > 1)
            {
                Trace.WriteLine("syntax error");
                return;
            }
            if (parameters.Length == 1)
            {
                MegaDirectory megaDirectory = new MegaDirectory(GetMegaEnvironment().GetDirectory());
                megaDirectory.SetDirectory(parameters[0]);
                GetMegaEnvironment().SetDirectory(megaDirectory.GetDirectory());
            }
            Trace.WriteLine(GetMegaEnvironment().GetDirectory());
        }

        public static void Ls(string[] parameters, NodesOptions option1, LsOptions option2)
        {
            if (parameters.Length > 1)
            {
                Trace.WriteLine("syntax error");
                return;
            }
            if (parameters.Length == 1)
                Cd(parameters);
            MegaClient megaClient = GetMegaClient();
            //NodesOptions options = NodesOptions.Directory | NodesOptions.File;
            TraceNodes(megaClient.GetNodes(GetMegaEnvironment().GetDirectory(), option1), option1, option2);
        }

        public static void TraceNodes(IEnumerable<MegaNode> nodes, NodesOptions option1, LsOptions option2)
        {
            int fileCount = 0;
            int directoryCount = 0;
            bool recursive = (option1 & NodesOptions.Recursive) == NodesOptions.Recursive;
            bool longView = (option2 & LsOptions.LongView) == LsOptions.LongView;
            if (longView)
                Trace.WriteLine("LastModificationDate Size           Path");
            foreach (MegaNode node in nodes)
            {
                if (longView)
                    Trace.WriteLine($"{node.Node.LastModificationDate} {node.Node.Size,15:### ### ### ###} {node.Path}");
                else if (recursive)
                    Trace.WriteLine(node.Path);
                else
                    Trace.WriteLine(node.Name);
                if (node.Node.Type == NodeType.File)
                    fileCount++;
                else
                    directoryCount++;
            }
            Trace.WriteLine($"{fileCount} files, {directoryCount} directories, total {fileCount + directoryCount}");
        }

        private static MegaClient GetMegaClient()
        {
            MegaClient megaClient = new MegaClient();
            //string environmentFile = XmlConfig.CurrentConfig.GetExplicit("LocalEnvironment");
            //if (!zFile.Exists(environmentFile))
            //{
            //    Trace.WriteLine("login not defined");
            //    return null;
            //}
            //string login = XDocument.Load(environmentFile).zXPathExplicitValue("Login");
            string login = GetMegaEnvironment().GetLogin();
            XmlConfig localConfig = XmlConfig.CurrentConfig.GetConfig("LocalConfig");
            XmlConfigElement configElement = localConfig.GetConfigElement($"Login[@name = '{login}']");
            if (configElement == null)
            {
                Trace.WriteLine($"unknow login \"{login}\"");
                return null;
            }
            megaClient.Email = configElement.GetExplicit("@email");
            megaClient.Password = configElement.GetExplicit("@password");
            megaClient.Login();
            return megaClient;
        }

        private static MegaEnvironment GetMegaEnvironment()
        {
            if (_environment == null)
                _environment = new MegaEnvironment();
            return _environment;
        }
    }
}
