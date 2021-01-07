using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.Mssql.Models;

namespace OctoMedia.Api.DataAccess.Mssql.Repositories.Interfaces
{
    internal interface IMssqlMediaRepository : IMediaRepository
    {
        public Task<IDictionary<Guid, CacheMedia>> GetIdToExtensionMappingAsync(CancellationToken cancellationToken);
    }
}