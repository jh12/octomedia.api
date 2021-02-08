using System.IO;

namespace OctoMedia.Api.Common.Models
{
    public class MediaStreamMetadata
    {
        public int Id { get; }
        public string Extension { get; }
        public Stream Content { get; }

        public MediaStreamMetadata(int id, string extension, Stream content)
        {
            Id = id;
            Extension = extension;
            Content = content;
        }
    }
}