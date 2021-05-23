using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.Media;
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
        public async Task<KeyedSource> GetSource(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                KeyedSource source = await _mediaRepository.GetSourceAsync(id, cancellationToken);

                return source;
            }
        }

        [HttpGet("sample")]
        public Task<KeyedSource[]> GetSample(int size = 20, CancellationToken cancellationToken = default)
        {
            return _mediaRepository.GetSourceSampleAsync(size, cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSource([FromBody] Source source, CancellationToken cancellationToken)
        {
            source.Deleted = false;

            Guid newId = await _mediaRepository.CreateSourceAsync(source, cancellationToken);

            return CreatedAtAction(nameof(GetSource), new { id = newId }, new CreatedResponse(newId));
        }

        [HttpGet("{id}/attachments")]
        public Task<SourceAttachment> GetAttachments(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                throw new NotImplementedException();
            }
        }

        [HttpGet("{id}/medias/ids")]
        public async Task<Guid[]> GetSourceMediaIds(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                Guid[] mediaIds = await _mediaRepository.GetSourceMediaIdsAsync(id, cancellationToken);

                return mediaIds;
            }
        }

        [HttpGet("{id}/medias")]
        public async Task<KeyedMedia[]> GetSourceMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("SourceId", id))
            {
                KeyedMedia[] medias = await _mediaRepository.GetSourceMediasAsync(id, cancellationToken);

                return medias;
            }
        }

        #region Reddit Attachment

        [HttpPost("{id}/attach/reddit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AttachRedditToSource(Guid id, [FromBody] RedditSource redditSource, CancellationToken cancellationToken)
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