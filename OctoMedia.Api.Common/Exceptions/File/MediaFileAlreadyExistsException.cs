using System.Net;
using OctoMedia.Api.Common.Exceptions.Entry;

namespace OctoMedia.Api.Common.Exceptions.File
{
    public class MediaFileAlreadyExistsException : EntryBaseException<int>
    {
        public MediaFileAlreadyExistsException(int key) : base(key, "A media file already exists", HttpStatusCode.Conflict)
        {
        }

        public MediaFileAlreadyExistsException(int key, string message) : base(key, message, HttpStatusCode.Conflict)
        {
        }

        public MediaFileAlreadyExistsException(string message) : base(HttpStatusCode.Conflict, message)
        {
        }

        public MediaFileAlreadyExistsException() : base(HttpStatusCode.Conflict, "A media file already exists")
        {
        }
    }
}