﻿using System;
using System.Collections.Concurrent;

namespace pb
{
    public class zTask
    {
        public string name;
        public string description;
        public Action task;
    }

    public class TaskManager : AsyncManager
    {
        private static TaskManager _currentTaskManager = new TaskManager();
        //private Queue<Task> _tasks = new Queue<Task>();
        private ConcurrentQueue<zTask> _tasks = new ConcurrentQueue<zTask>();

        public static TaskManager CurrentTaskManager { get { return _currentTaskManager; } set { _currentTaskManager = value; } }
        public int Count { get { return _tasks.Count; } }

        public static void StartTaskManager()
        {
            _currentTaskManager.Start();
        }

        public static void StopTaskManager()
        {
            _currentTaskManager.Stop();
        }

        public static bool IsTaskManagerStarted()
        {
            return _currentTaskManager.IsStarted();
        }

        public static void AddTask(zTask task)
        {
            _currentTaskManager.Add(task);
        }

        public void Add(zTask task)
        {
            _tasks.Enqueue(task);
        }

        protected override void ThreadExecute()
        {
            while (_tasks.Count != 0)
            {
                try
                {
                    //_tasks.Dequeue().task();
                    zTask task;
                    if (_tasks.TryDequeue(out task))
                        task.task();
                }
                catch (Exception exception)
                {
                    if (_onError != null)
                        _onError(exception);
                }
            }
        }
    }
}
