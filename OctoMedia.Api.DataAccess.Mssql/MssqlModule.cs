using Autofac;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.Mssql.Factories;
using OctoMedia.Api.DataAccess.Mssql.Repositories;
using OctoMedia.Api.DataAccess.Mssql.Repositories.Interfaces;

namespace OctoMedia.Api.DataAccess.Mssql
{
    public class MssqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MssqlConnectionFactory>();

            builder.RegisterType<MssqlMediaRepository>().As<IMssqlMediaRepository>().As<IMediaRepository>();
        }
    }
}
