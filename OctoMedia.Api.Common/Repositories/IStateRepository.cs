using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.DTOs.V1.State;

namespace OctoMedia.Api.Common.Repositories
{
    public interface IStateRepository
    {
        IAsyncEnumerable<T> GetStatesAsync<T>(string keyPattern, CancellationToken cancellationToken) where T : State;
        Task SaveStateAsync<T>(string key, T value, CancellationToken cancellationToken) where T : State;
        Task DeleteStateAsync<T>(string key, CancellationToken cancellationToken) where T : State;
    }
}