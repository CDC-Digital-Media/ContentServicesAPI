// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gov.Hhs.Cdc.MediaValidation
{
    public class MultiThreadedQueue<T> : IDisposable where T : class
    {
        object theLock = new object();
        List<Thread> workers;
        Queue<T> taskQueue = new Queue<T>();
        Action<T> dequeueAction;

        bool hasDisposeTimeout = false;
        TimeSpan disposeTimeout;

        bool isDisposing = false;
        DateTime timeToForceStop;

        public MultiThreadedQueue(int workerCount, Action<T> dequeueAction, TimeSpan disposeTimeout)
        {
            this.disposeTimeout = disposeTimeout;
            hasDisposeTimeout = true;
            Initialize(workerCount, dequeueAction);
        }
        /// <summary>       
        /// Initializes a new instance of the <see cref="MultiThreadedQueue{T}"/> class.     
        /// </summary>     
        /// <param name="workerCount">The worker count.</param>     
        /// <param name="dequeueAction">The dequeue action.</param>     
        public MultiThreadedQueue(int workerCount, Action<T> dequeueAction)
        {
            Initialize(workerCount, dequeueAction);
        }

        private void Initialize(int workerCount, Action<T> dequeueAction)
        {
            try
            {
                this.dequeueAction = dequeueAction;
                workers = new List<Thread>(workerCount);

                // Create and start a separate thread for each worker         
                for (int i = 0; i < workerCount; i++)
                {
                    Thread t = new Thread(Consume)
                    {
                        IsBackground = true,
                        Name = string.Format("MultiThreadedQueue worker {0}", i)
                    };
                    workers.Add(t);
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                //ErrorLogger.GetExceptionAndLogError(ex, "MultiThreadedQueue.Constructor()", "");
                throw;
            }
        }

        /// <summary>     
        /// Enqueues the task.     
        /// </summary>     
        /// <param name="task">The task.</param>     
        public void EnqueueTask(T task)
        {
            try
            {
                lock (theLock)
                {
                    taskQueue.Enqueue(task);
                    Monitor.PulseAll(theLock);
                }
            }
            catch (Exception ex)
            {
                //ErrorLogger.GetExceptionAndLogError(ex, "MultiThreadedQueue.EnqueueTask", "");
                throw;
            }
        }

        /// <summary>     
        /// Consumes this instance.     
        /// </summary>     
        void Consume()
        {
            try
            {
                while (true)
                {
                    T item;
                    lock (theLock)
                    {
                        while (taskQueue.Count == 0)
                        {
                            if (isDisposing)
                                return;
                            Monitor.Wait(theLock);
                        }
                        item = taskQueue.Dequeue();
                        if (isDisposing)
                            if (!hasDisposeTimeout)
                                return;
                            else if (DateTime.Now >= timeToForceStop)
                                return;
                    }

                    // run actual method             
                    dequeueAction(item);
                }
            }
            catch (Exception ex)
            {
                //ErrorLogger.GetExceptionAndLogError(ex, "MultiThreadedQueue.Consume", "");
                throw;
            }
        }
        /// <summary>     
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.     
        /// </summary>     
        public void Dispose()
        {

            try
            {

                lock (theLock)
                {        
                    isDisposing = true;
                    if (hasDisposeTimeout)
                        timeToForceStop = DateTime.Now.Add(disposeTimeout);
                    Monitor.PulseAll(theLock);
                }
                workers.ForEach(thread => thread.Join());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
