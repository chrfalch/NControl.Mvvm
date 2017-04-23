using System;
using System.Threading.Tasks;

namespace NControl.Mvvm
{
    public interface ITaskProvider
    {
        Task ExecuteOnMainThreadAsync(Func<Task> action);
        Task ExecuteOnMainThreadAsync(Action action);
    }
}
