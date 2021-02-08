using Autofac;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // LinQ queries seems broken if this obsolete property is not set
#pragma warning disable 618
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore 618
        }
    }
}