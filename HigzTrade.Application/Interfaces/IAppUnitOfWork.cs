using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces
{
    public interface IAppUnitOfWork
    {
        Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct);
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct);
    }
}
