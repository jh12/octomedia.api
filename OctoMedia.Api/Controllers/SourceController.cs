using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;
using OctoMedia.Api.DTOs.V1.Responses;
using Serilog.Context;

namespace OctoMedia.Api.Controllers
{
    [ApiController]
    [Route("v1/source")]
    public class SourceController : ControllerBase
    {
        private readonly IMediaRepository _mediaRepository;

        public SourceController(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        [HttpGet("{id}")]
        public async Task<KeyedSource> GetSource(int id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                KeyedSource source = await _mediaRepository.GetSourceAsync(id, cancellationToken);

                return source;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSource([FromBody] Source source, CancellationToken cancellationToken)
        {
            int newId = await _mediaRepository.CreateSourceAsync(source, cancellationToken);

            return CreatedAtAction(nameof(GetSource), new { id = newId }, new CreatedResponse(newId));
        }

        [HttpGet("{id}/attachments")]
        public Task<SourceAttachments> GetAttachments(int id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                throw new NotImplementedException();
            }
        }

        #region Reddit Attachment

        [HttpPost("{id}/attach/reddit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AttachRedditToSource(int id, [FromBody] RedditSource redditSource, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                if (redditSource.PostedAt.Kind != DateTimeKind.Utc)
                    return BadRequest(new TextResponse("PostedAt must be in UTC"));

                await _mediaRepository.AttachRedditToSource(id, redditSource, cancellationToken);

                return CreatedAtAction(nameof(GetAttachments), new { id }, new TextResponse($"Attached to source with id {id}"));
            }
        }

        #endregion
    }
}