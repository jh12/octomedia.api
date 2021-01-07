using System;
using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public class EntryAlreadyExistsException : EntryBaseException<Guid>
    {


        public EntryAlreadyExistsException(Guid key) : base(key, "An entry already exists", HttpStatusCode.Conflict)
        {
        }

        public EntryAlreadyExistsException(Guid key, string message) : base(key, message, HttpStatusCode.Conflict)
        {
        }

        public EntryAlreadyExistsException(string message) : base(HttpStatusCode.Conflict, message)
        {
        }

        public EntryAlreadyExistsException() : base(HttpStatusCode.Conflict, "An entry already exists")
        {
        }
    }
}