using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using pb.Windows.Win32;

namespace pb
{
    public static class zprocess
    {
        public static Process Execute(string sProgram)
        {
            return Execute(sProgram, null);
        }

        public static Process Execute(string sProgram, string sArguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = sProgram;
            process.StartInfo.Arguments = sArguments;
            process.Start();
            return process;
        }

        public static void CloseProcess(Process process)
        {
            bool b = process.CloseMainWindow();
            //if (b) cTrace.Trace("CloseMainWindow() : result {0} hWnd {1} title {2}", b, process.MainWindowHandle, process.MainWindowTitle);
            if (b) Trace.CurrentTrace.WriteLine("CloseMainWindow() : result {0} hWnd {1} title {2}", b, process.MainWindowHandle, process.MainWindowTitle);

            DateTime t0;
            if (!b)
            {
                WindowInfo[] windows = pb.Windows.Win32.Windows.GetProcessWindowsInfoList(process);
                foreach (WindowInfo window in windows)
                {
                    if ((window.wi.dwExStyle & User.Const.WS_EX_APPWINDOW) == User.Const.WS_EX_APPWINDOW)
                    {
                        //cTrace.Trace("Close window  : hWnd {0} title {1}", (int)window.hWnd, window.Title);
                        Trace.CurrentTrace.WriteLine("Close window  : hWnd {0} title {1}", (int)window.hWnd, window.Title);
                        User.SendMessage(window.hWnd, (uint)User.WM.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        b = true;
                        break;

                        //cTrace.Trace("Show window  : hWnd {0} title {1}", (int)window.hWnd, window.Title);
                        //User.ShowWindow(window.hWnd, User.Const.SW_SHOW);

                        //// boucle d'attente pour que la fenetre devienne visible
                        //t0 = DateTime.Now;
                        //while (true)
                        //{
                        //    if (DateTime.Now.Subtract(t0).TotalMilliseconds > 1000)
                        //        throw new pb.old.PBException("error impossible to show window {0} - {1}", window.Title, window.hWnd);
                        //    WindowInfo window2 = Windows.GetWindowInfo(window.hWnd);
                        //    //wr.Print("window  : {0:x} - {1} - {2}", (int)wi2.hWnd, wi2.Title, GetWindowStyle(wi2.wi.dwStyle));
                        //    if ((window2.wi.dwStyle & User.Const.WS_VISIBLE) == User.Const.WS_VISIBLE) break;
                        //    Thread.Sleep(10);
                        //}

                        //Process process2 = Process.GetProcessById(process.Id);
                        //b = process2.CloseMainWindow();
                        //cTrace.Trace("CloseMainWindow() : result {0} hWnd {1} title {2}", b, process2.MainWindowHandle, process2.MainWindowTitle);
                        //if (b) break;
                    }
                }
            }

            // boucle d'attente pour que le process se ferme
            if (b)
            {
                t0 = DateTime.Now;
                while (!process.HasExited)
                {
                    if (DateTime.Now.Subtract(t0).TotalMilliseconds > 10000)
                        //throw new pb.old.PBException("error impossible to close process {0} - {1}", process.ProcessName, process.Id);
                        break;
                    Thread.Sleep(10);
                }
                if (process.HasExited) return;
            }

            //cTrace.Trace("Kill process : {0}", process.ProcessName);
            Trace.CurrentTrace.WriteLine("Kill process : {0}", process.ProcessName);
            process.Kill();
            t0 = DateTime.Now;
            while (!process.HasExited)
            {
                if (DateTime.Now.Subtract(t0).TotalMilliseconds > 10000)
                    throw new PBException("error impossible to close process {0} - {1}", process.ProcessName, process.Id);
                Thread.Sleep(10);
            }
        }
    }
}
