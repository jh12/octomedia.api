using System.Net;
using OctoMedia.Api.Common.Exceptions.Entry;

namespace OctoMedia.Api.Common.Exceptions.File
{
    public class MediaFileNotFoundException : EntryBaseException<int>
    {
        public MediaFileNotFoundException(int key) : base(key, "No media file found with the provided id", HttpStatusCode.NotFound)
        {
        }

        public MediaFileNotFoundException(int key, string message) : base(key, message, HttpStatusCode.NotFound)
        { 
        }
    }
}