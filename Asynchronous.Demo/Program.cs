using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Asynchronous.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler syncContextScheduler;
            if (SynchronizationContext.Current != null)
            {
                syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            else
            {
                // If there is no SyncContext for this thread (e.g. we are in a unit test
                // or console scenario instead of running in an app), then just use the
                // default scheduler because there is no UI thread to sync with.
                syncContextScheduler = TaskScheduler.Current;
            }

            // computation task
            Task T = new Task(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(800);
                        Console.WriteLine("=> Hello Task {0}", i);
                    }
                }
            );

            // simulated UI update thread
            Task T2 = T.ContinueWith((antecedent) =>
                {
                    Console.WriteLine("Hello Continue");
                },
                syncContextScheduler
            );
            T.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                Console.WriteLine("Hello Main {0}", i);
            }

            // console apps have limited support - simply Wait() for returned task to complete
            T.Wait();
        }
    }
}
