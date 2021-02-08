using System;
using System.Net;

namespace OctoMedia.Api.Common.Exceptions.Entry
{
    public class EntryNotFoundException : EntryBaseException<Guid>
    {
        public EntryNotFoundException(Guid key) : base(key, "No entry found with the provided id", HttpStatusCode.NotFound)
        {
        }

        public EntryNotFoundException(Guid key, string message) : base(key, message, HttpStatusCode.NotFound)
        { 
        }
    }
}