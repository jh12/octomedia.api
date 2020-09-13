using System;
using Autofac;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.FileSystem.Repositories;

namespace OctoMedia.Api.DataAccess.FileSystem
{
    public class FileSystemModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemFileRepository>().As<IFileRepository>();
            builder.RegisterType<FileSystemStateRepository>().As<IStateRepository>();
        }
    }
}
