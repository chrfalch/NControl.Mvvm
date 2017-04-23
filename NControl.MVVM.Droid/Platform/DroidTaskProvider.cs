using System;
using System.Threading.Tasks;
using Android.OS;

namespace NControl.Mvvm.Droid
{
    public class DroidTaskProvider: ITaskProvider
    {
        public Task ExecuteOnMainThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (Looper.MyLooper() == Looper.MainLooper)
            {
                action();
                tcs.TrySetResult(true);
            }
            else
            {
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(() =>
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
            }

            return tcs.Task;
        }

        public Task ExecuteOnMainThreadAsync(Func<Task> action)
        {
            if (Looper.MyLooper() == Looper.MainLooper)
                return action();

            var tcs = new TaskCompletionSource<bool>();
            using (var h = new Handler(Looper.MainLooper))
                h.Post(async () => {
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
