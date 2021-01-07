using Autofac;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.MongoDB.Factories;
using OctoMedia.Api.DataAccess.MongoDB.Repositories;

namespace OctoMedia.Api.DataAccess.MongoDB
{
    public class MongoDBModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MongoDBContextFactory>();

            builder.RegisterType<MongoDBMediaRepository>().As<IMediaRepository>().SingleInstance();
        }
    }
}