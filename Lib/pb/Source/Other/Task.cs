using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using pb;

namespace PB_Library
{
    public delegate void TaskEventHandler(ITask task);

    public interface ITask
    {
        string TaskName { get; }
        event TaskEventHandler TaskEnded;
        //Trace TaskTrace { get; }
        //ITrace TaskTrace { get; }
        Progress TaskProgress { get; }
        Progress TaskProgressDetail { get; }
        void ExecuteTask();
        void AbortTask();
        void SuspendTask();
        void ResumeTask();
    }

    #region class ApplicationThread
    public class ApplicationThread
    {
        public Thread Thread;
        public ITask Task;

        public ApplicationThread(Thread thread, ITask task)
        {
            Thread = thread;
            Task = task;
            task.TaskEnded += new TaskEventHandler(TaskEnded);
        }

        #region TaskEnded
        public void TaskEnded(ITask task)
        {
            ApplicationThreads.Remove(this);
        }
        #endregion
    }
    #endregion

    #region class ApplicationThreads
    public static class ApplicationThreads
    {
        private static SortedList<int, ApplicationThread> gThreadList = new SortedList<int, ApplicationThread>();

        #region Add
        public static void Add(Thread thread, ITask task)
        {
            gThreadList.Add(thread.ManagedThreadId, new ApplicationThread(thread, task));
        }
        #endregion

        #region Remove
        public static void Remove(ApplicationThread applicationThread)
        {
            int i = gThreadList.IndexOfKey(applicationThread.Thread.ManagedThreadId);
            if (i != -1) gThreadList.RemoveAt(i);
        }
        #endregion

        #region ExitAllThreads
        public static bool ExitAllThreads()
        {
            return true;
        }
        #endregion
    }
    #endregion

    #region TaskScheduler
    // Error	7	'PB_Library.TaskScheduler' does not implement interface member 'PB_Library.ITask.TaskTrace'. 'PB_Library.TaskScheduler.TaskTrace' cannot implement 'PB_Library.ITask.TaskTrace' because it does not have the matching return type of 'pb.old.Trace'.	C:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Library\Other\Task.cs	82	18	Source_01
    public class TaskScheduler : ITask
    {
        #region variable
        private const string gTaskName = "TaskScheduler";
        private bool gAbortTask = false;
        //private bool gSuspendTask = false;

        private List<ITask> gTaskList = new List<ITask>();
        private ITask gCurrentTask = null;
        private Thread gThread = null;

        public event TaskEventHandler TaskEnded;
        #endregion

        #region property
        public string TaskName { get { return gTaskName; } }
        //public cTraced TaskTrace { get { return null; } }
        //public ITrace TaskTrace { get { return null; } }
        public Progress TaskProgress { get { return null; } }
        public Progress TaskProgressDetail { get { return null; } }
        public Thread Thread
        {
            get { return gThread; }
        }
        #endregion

        #region IsRunning
        public bool IsRunning()
        {
            return gThread != null;
        }
        #endregion

        #region AddTask
        public void AddTask(ITask task)
        {
            gTaskList.Add(task);
            ExecuteTask();
        }
        #endregion

        #region ExecuteTask
        public void ExecuteTask()
        {
            if (gCurrentTask != null || gTaskList.Count == 0) return;
            //gCurrentTask = gTaskList[0];
            //gTaskList.RemoveAt(0);
            //////////////////////////////gCurrentTask.EndFunction += new TaskEndFunction(EndTask);
            //gCurrentTask.TaskEnded += new TaskEventHandler(EndTask);
            //ThreadStart ts = new ThreadStart(gCurrentTask.ExecuteTask);
            //gCurrentTaskThread = new Thread(ts);
            //gCurrentTaskThread.Start();
            ThreadStart ts = new ThreadStart(ExecuteTaskThread);
            gThread = new Thread(ts);
            ApplicationThreads.Add(gThread, this);
            gThread.Start();
        }
        #endregion

        #region ExecuteTaskThread
        private void ExecuteTaskThread()
        {
            if (gCurrentTask != null) return;
            while (gTaskList.Count != 0)
            {
                if (gAbortTask) break;
                gCurrentTask = gTaskList[0];
                gTaskList.RemoveAt(0);
                ////////////////////////////gCurrentTask.EndFunction += new TaskEndFunction(EndTask);
                //gCurrentTask.TaskEnded += new TaskEventHandler(EndTask);
                //ThreadStart ts = new ThreadStart(gCurrentTask.ExecuteTask);
                //gCurrentTaskThread = new Thread(ts);
                //gCurrentTaskThread.Start();
                gCurrentTask.ExecuteTask();
                ///////////////////gCurrentTask.EndFunction -= new TaskEndFunction(EndTask);
                gCurrentTask = null;
            }
            if (TaskEnded != null) TaskEnded(this);
            gThread = null;
        }
        #endregion

        #region //EndTask
        //private void EndTask(ITask task)
        //{
        //    ///////////////////gCurrentTask.EndFunction -= new TaskEndFunction(EndTask);
        //    gCurrentTask.TaskEnded -= new TaskEventHandler(EndTask);
        //    gCurrentTask = null;
        //    ExecuteTasks();
        //}
        #endregion

        #region KillTask
        public void KillTask()
        {
            if (gThread == null) return;
            string s = "unknow";
            if (gCurrentTask != null) s = gCurrentTask.TaskName;
            Trace.WriteLine("Abort tast {0}", s);
            gThread.Abort();
        }
        #endregion

        #region AbortTask
        public void AbortTask()
        {
            gAbortTask = true;
            if (gCurrentTask != null)
                gCurrentTask.AbortTask();
        }
        #endregion

        #region SuspendTask
        public void SuspendTask()
        {
            //gSuspendTask = true;
            if (gCurrentTask != null)
                gCurrentTask.SuspendTask();
        }
        #endregion

        #region ResumeTask
        public void ResumeTask()
        {
            //gSuspendTask = false;
            if (gCurrentTask != null)
                gCurrentTask.ResumeTask();
        }
        #endregion
    }
    #endregion
}
