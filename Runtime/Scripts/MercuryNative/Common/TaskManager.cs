/// Simple, really.  There is no need to initialize or even refer to TaskManager.  
/// When the first Task is created in an application, a "TaskManager" GameObject  
/// will automatically be added to the scene root with the TaskManager component  
/// attached.  This component will be responsible for dispatching all coroutines  
/// behind the scenes.  
///  
/// Task also provides an event that is triggered when the coroutine exits.  

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RayNeo.Native
{
    /// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.  
    /// It is an error to attempt to start a task that has been stopped or which has  
    /// naturally terminated.  
    public class Task
    {
        public static Task CreateTask(IEnumerator c, bool autoStart = true)
        {
            return new Task(c, autoStart);
        }

        /// Returns true if and only if the coroutine is running.  Paused tasks  
        /// are considered to be running.  
        public bool Running
        {
            get
            {
                return m_Task.Running;
            }
        }

        /// Returns true if and only if the coroutine is currently paused.  
        public bool Paused
        {
            get
            {
                return m_Task.Paused;
            }
        }

        /// Delegate for termination subscribers.  manual is true if and only if  
        /// the coroutine was stopped with an explicit call to Stop().  
        public delegate void FinishedHandler(Task task, bool manual);

        /// Termination event.  Triggered when the coroutine completes execution.  
        public event FinishedHandler Finished;

        /// Creates a new Task object for the given coroutine.  
        ///  
        /// If autoStart is true (default) the task is automatically started  
        /// upon construction.  
        public Task(IEnumerator c, bool autoStart = true)
        {
            m_Task = TaskManager.CreateTask(c);

            m_Task.Finished += TaskFinished;

            if (autoStart)
            {
                Start();
            }
        }

        /// Begins execution of the coroutine  
        public void Start()
        {
            m_Task.Start();
        }

        /// Discontinues execution of the coroutine at its next yield.  
        public void Stop()
        {
            m_Task.Stop();
        }

        public void Pause()
        {
            m_Task.Pause();
        }

        public void Unpause()
        {
            m_Task.Unpause();
        }

        public Coroutine Coroutine
        {
            get
            {
                return m_Task.Coroutine;
            }
        }

        void TaskFinished(bool manual)
        {
            FinishedHandler handler = Finished;
            if (handler != null)
            {
                handler(this, manual);
            }
        }

        internal static void CreateTask(object loadScene)
        {
            throw new NotImplementedException();
        }

        TaskManager.TaskState m_Task;
    }

    public class TaskManager : MonoBehaviour
    {
        public class TaskState
        {
            public bool Running
            {
                get
                {
                    return m_Running;
                }
            }

            public bool Paused
            {
                get
                {
                    return m_Paused;
                }
            }

            public Coroutine Coroutine
            {
                get
                {
                    return m_Coroutine;
                }
            }

            public delegate void FinishedHandler(bool manual);
            public event FinishedHandler Finished;

            IEnumerator m_Enumerator;
            Coroutine m_Coroutine;
            bool m_Running;
            bool m_Paused;
            bool m_Stopped;

            public TaskState(IEnumerator c)
            {
                m_Enumerator = c;
            }

            public void Pause()
            {
                m_Paused = true;
            }

            public void Unpause()
            {
                m_Paused = false;
            }

            public void Start()
            {
                m_Running = true;

                m_Coroutine = m_Singleton.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                m_Stopped = true;
                m_Running = false;
            }

            private IEnumerator CallWrapper()
            {
                //yield return null;

                IEnumerator e = m_Enumerator;

                while (m_Running)
                {
                    if (m_Paused)
                    {
                        yield return null;
                    }
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            m_Running = false;
                        }
                    }
                }

                FinishedHandler handler = Finished;

                if (handler != null)
                {
                    handler(m_Stopped);
                }
            }
        }

        private static TaskManager m_Singleton;

        public static TaskState CreateTask(IEnumerator coroutine)
        {
            if (m_Singleton == null)
            {
                GameObject go = new GameObject("TaskManager");
                m_Singleton = go.AddComponent<TaskManager>();
                DontDestroyOnLoad(go);
            }
            return new TaskState(coroutine);
        }
    }

    /// <summary>
    /// 任务队列
    /// 在某个时间点最多只有一个协程在执行，先加入队列中的先执行，后加入的后执行
    /// </summary>
    public class TaskQueue
    {
        static Dictionary<string, TaskQueue> m_TaskQueues = new Dictionary<string, TaskQueue>();
        public static TaskQueue GetTaskQueue(string id)
        {
            TaskQueue taskQueue;
            if (!m_TaskQueues.TryGetValue(id, out taskQueue))
            {
                taskQueue = new TaskQueue();
                m_TaskQueues[id] = taskQueue;
            }
            return taskQueue;
        }

        public static void RemoveTaskQueue(string id)
        {
            if (m_TaskQueues.ContainsKey(id))
            {
                m_TaskQueues.Remove(id);
            }
        }

        private List<TaskInfo> m_ListTask = new List<TaskInfo>();

        public delegate void DelegateVoid();
        public DelegateVoid OnTaskQueueFinished;

        public TaskInfo Add(IEnumerator c)
        {
            TaskInfo taskInfo = new TaskInfo();
            taskInfo.c = c;
            m_ListTask.Add(taskInfo);

            if (m_ListTask.Count == 1)
            {
                Run(m_ListTask[0]);
            }
            return taskInfo;
        }

        public void Remove(TaskInfo taskInfo)
        {
            if (m_ListTask.Contains(taskInfo))
            {
                bool removeTaskIsRuning = false;
                if (taskInfo.task != null)
                {
                    taskInfo.task.Stop();
                    removeTaskIsRuning = true;
                }
                m_ListTask.Remove(taskInfo);

                if (removeTaskIsRuning && m_ListTask.Count > 0)
                {
                    Run(m_ListTask[0]);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_ListTask.Count; i++)
            {
                if (m_ListTask[i].task != null)
                {
                    m_ListTask[i].task.Stop();
                }
            }
            m_ListTask.Clear();
        }

        void OnFinish(Task task, bool manual)
        {
            if (m_ListTask.Count > 0)
            {
                m_ListTask.RemoveAt(0);

                if (m_ListTask.Count > 0)
                {
                    Run(m_ListTask[0]);
                }
                else
                {
                    if (OnTaskQueueFinished != null)
                    {
                        OnTaskQueueFinished();
                    }
                }

            }
        }

        void Run(TaskInfo taskInfo)
        {
            taskInfo.task = new Task(taskInfo.c);
            taskInfo.task.Finished += OnFinish;
        }

        public class TaskInfo
        {
            public IEnumerator c;
            public Task task;
        }
    }
}