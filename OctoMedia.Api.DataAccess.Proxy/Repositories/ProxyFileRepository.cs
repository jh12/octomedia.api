using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeTypes;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.Common.Models;
using OctoMedia.Api.Common.Options;
using OctoMedia.Api.Common.Repositories;

namespace OctoMedia.Api.DataAccess.Proxy.Repositories
{
    public class ProxyFileRepository : IFileRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ProxyOptions _proxyOptions;

        public ProxyFileRepository(HttpClient httpClient, IOptions<ProxyOptions> proxyOptions)
        {
            // TODO: Configure HttpClient properly with baseaddress
            
            _httpClient = httpClient;
            _proxyOptions = proxyOptions.Value;
        }

        public async Task<MediaStreamMetadata> GetMediaAsync(int id, CancellationToken cancellationToken)
        {
            string baseUrl = _proxyOptions.BaseLocation;

            HttpResponseMessage response = await _httpClient.GetAsync($"{baseUrl}/v1/media/{id}/file");

            MediaTypeHeaderValue mediaTypeHeaderValue = response.Content.Headers.ContentType;

            if(string.IsNullOrEmpty(mediaTypeHeaderValue.MediaType))
                throw new HttpResponseException(HttpStatusCode.FailedDependency, "No content type received from proxy server");

            string extension = MimeTypeMap.GetExtension(mediaTypeHeaderValue.MediaType).Replace(".", string.Empty);

            return new MediaStreamMetadata(id, extension, response.Content.ReadAsStream(cancellationToken));
        }

        public async Task SaveMediaAsync(int id, string extension, Stream stream, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}