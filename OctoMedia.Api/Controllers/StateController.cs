using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.Responses;
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
        [ProducesResponseType(typeof(ListedStringState[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDateTimeStates(string keyPattern, CancellationToken cancellationToken)
        {
            IEnumerable<KeyedDateTimeState> states = await _stateRepository.GetStatesAsync<KeyedDateTimeState>(keyPattern, cancellationToken);

            return Ok(states);
        }

        [HttpPut("keyed/datetime/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveDateTimeState(string key, [FromBody] KeyedDateTimeState value, CancellationToken cancellationToken)
        {
            await _stateRepository.SaveStateAsync(key, value, cancellationToken);

            return Ok();
        }

        [HttpDelete("keyed/datetime/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDateTimeState(string key, CancellationToken cancellationToken)
        {
            await _stateRepository.DeleteStateAsync<KeyedDateTimeState>(key, cancellationToken);

            return Ok();
        }

        #endregion

        #region Keyed string

        [HttpGet("listed/string/{keyPattern}")]
        [ProducesResponseType(typeof(ListedStringState[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStringStates(string keyPattern, CancellationToken cancellationToken)
        {
            IEnumerable<ListedStringState> states = await _stateRepository.GetStatesAsync<ListedStringState>(keyPattern, cancellationToken);

            return Ok(states);
        }

        [HttpPut("listed/string/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveStringState(string key, [FromBody] ListedStringState value, CancellationToken cancellationToken)
        {
            await _stateRepository.SaveStateAsync(key, value, cancellationToken);

            return Ok();
        }

        [HttpDelete("listed/string/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStringState(string key, CancellationToken cancellationToken)
        {
            await _stateRepository.DeleteStateAsync<ListedStringState>(key, cancellationToken);

            return Ok();
        }

        #endregion
    }
}