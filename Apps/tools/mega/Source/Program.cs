using pb;
using pb.Data.Xml;
using pb.Web.Data;
using System;
using System.Collections.Generic;

namespace Mega
{
    public enum MegaCommand
    {
        Undefined = 0,
        SetLogin,           // login
        Cd,                 // cd
        Ls                  // ls
    }

    public static class Program
    {
        private static MegaCommand _megaCommand = MegaCommand.Undefined;
        private static List<string> _parameters = new List<string>();
        // ls options
        private static NodesOptions _lsOption1 = NodesOptions.Default;
        private static LsOptions _lsOption2 = LsOptions.None;
        private static bool _help = false;

        static void Main(string[] args)
        {
            TraceManager.Current.AddTrace(Trace.Current);
            TraceManager.Current.SetViewer(text => Console.Write(text));
            XmlConfig.CurrentConfig = new XmlConfig();
            try
            {
                if (ManageParameters(args))
                {
                    ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        private static void ExecuteCommand()
        {
            if (_help)
                Help();
            else
            {
                switch (_megaCommand)
                {
                    case MegaCommand.SetLogin:
                        MegaExecuteCommand.SetLogin(_parameters.ToArray());
                        break;
                    case MegaCommand.Cd:
                        MegaExecuteCommand.Cd(_parameters.ToArray());
                        break;
                    case MegaCommand.Ls:
                        MegaExecuteCommand.Ls(_parameters.ToArray(), _lsOption1, _lsOption2);
                        break;
                }
            }
        }

        private static bool ManageParameters(string[] args)
        {
            foreach (string arg in args)
            {
                bool r;
                if (arg.StartsWith("-"))
                    r = SetOption(arg);
                else if (_megaCommand == MegaCommand.Undefined)
                    r = SetCommand(arg);
                else
                    r = SetParameter(arg);
                if (!r)
                    return false;
            }
            if (_megaCommand == MegaCommand.Undefined)
                _help = true;
            return true;
        }

        private static bool SetOption(string option)
        {
            switch (option)
            {
                case "-?":
                case "-h":
                case "--help":
                    _help = true;
                    break;

                // ls options
                case "-R":
                    _lsOption1 |= NodesOptions.Recursive;
                    break;
                case "-l":
                    _lsOption2 |= LsOptions.LongView;
                    break;
                default:
                    Trace.WriteLine($"unknow option \"{option}\"");
                    return false;
            }
            return true;
        }

        private static bool SetCommand(string command)
        {
            switch (command.ToLower())
            {
                case "login":
                    _megaCommand = MegaCommand.SetLogin;
                    break;
                case "cd":
                    _megaCommand = MegaCommand.Cd;
                    break;
                case "ls":
                    _megaCommand = MegaCommand.Ls;
                    break;
                default:
                    Trace.WriteLine($"unknow command \"{command}\"");
                    return false;
            }
            return true;
        }

        private static bool SetParameter(string parameter)
        {
            _parameters.Add(parameter);
            return true;
        }

        private static void Help()
        {
            Trace.WriteLine($"mega [option] command [parameter ...]");
            Trace.WriteLine($"  option  : -? -h --help");
            Trace.WriteLine($"  command : login | cd | ls");
            Trace.WriteLine($"    login name");
            Trace.WriteLine($"    cd directory");
            Trace.WriteLine($"    ls [directory]");
        }
    }
}
