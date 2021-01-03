using Autofac;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.FileSystem.Repositories;
using OctoMedia.Api.DataAccess.Proxy.Repositories;

namespace OctoMedia.Api.Modules
{
    public class FileModule : Module
    {
        private readonly bool _shouldProxy;

        public FileModule(bool shouldProxy)
        {
            _shouldProxy = shouldProxy;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if(_shouldProxy)
                builder.RegisterType<ProxyFileRepository>().As<IFileRepository>();
            else
                builder.RegisterType<FileSystemFileRepository>().As<IFileRepository>();

            builder.RegisterType<FileSystemStateRepository>().As<IStateRepository>();
        }
    }
}