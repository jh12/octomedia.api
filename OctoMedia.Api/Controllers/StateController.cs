using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.State;

namespace OctoMedia.Api.Controllers
{
    [ApiController]
    [Route("v1/state")]
    public class StateController : ControllerBase
    {
        private readonly IStateRepository _stateRepository;

        public StateController(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }


        #region Keyed datetime

        [HttpGet("keyed/datetime/{keyPattern}")]
        public async IAsyncEnumerable<KeyedDateTimeState> GetDateTimeStates(string keyPattern, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (KeyedDateTimeState state in _stateRepository.GetStatesAsync<KeyedDateTimeState>(keyPattern, cancellationToken))
            {
                yield return state;
            }
        }

        [HttpPut("keyed/datetime/{key}")]
        public async Task<IActionResult> SaveDateTimeState(string key, [FromBody] KeyedDateTimeState value, CancellationToken cancellationToken)
        {
            await _stateRepository.SaveStateAsync(key, value, cancellationToken);

            return Ok();
        }

        [HttpDelete("keyed/datetime/{key}")]
        public async Task<IActionResult> DeleteDateTimeState(string key, CancellationToken cancellationToken)
        {
            await _stateRepository.DeleteStateAsync<KeyedDateTimeState>(key, cancellationToken);

            return Ok();
        }

        #endregion

        #region Keyed string

        [HttpGet("listed/string/{keyPattern}")]
        public async IAsyncEnumerable<ListedStringState> GetStringStates(string keyPattern, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (ListedStringState state in _stateRepository.GetStatesAsync<ListedStringState>(keyPattern, cancellationToken))
            {
                yield return state;
            }
        }

        [HttpPut("listed/string/{key}")]
        public async Task<IActionResult> SaveStringState(string key, [FromBody] ListedStringState value, CancellationToken cancellationToken)
        {
            await _stateRepository.SaveStateAsync(key, value, cancellationToken);

            return Ok();
        }

        [HttpDelete("listed/string/{key}")]
        public async Task<IActionResult> DeleteStringState(string key, CancellationToken cancellationToken)
        {
            await _stateRepository.DeleteStateAsync<ListedStringState>(key, cancellationToken);

            return Ok();
        }

        #endregion
    }
}