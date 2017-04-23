using System;
using System.Threading.Tasks;
using Foundation;

namespace NControl.Mvvm.iOS
{
    public class TouchTaskProvider: ITaskProvider
    {
        public Task ExecuteOnMainThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (NSThread.IsMain)
            {
                action();
                tcs.TrySetResult(true);
            }
            else
            {
                NSThread.MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        action();
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });
            }

            return tcs.Task;
        }

        public Task ExecuteOnMainThreadAsync(Func<Task> action)
        {
            if (NSThread.IsMain)
                return action();

            var tcs = new TaskCompletionSource<bool>();
            NSThread.MainThread.BeginInvokeOnMainThread(async () => { 
                try
                {
                    await action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
