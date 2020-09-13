using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public class EntryNotFoundException : EntryBaseException
    {
        public EntryNotFoundException(int key) : base(key, "No entry found with the provided id", HttpStatusCode.NotFound)
        {
        }

        public EntryNotFoundException(int key, string message) : base(key, message, HttpStatusCode.NotFound)
        { 
        }
    }
}