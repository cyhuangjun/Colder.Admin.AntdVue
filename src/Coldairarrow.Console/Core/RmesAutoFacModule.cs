using Autofac; 

namespace Coldairarrow.Scheduler.Core
{
    public class RmesAutoFacModule : Autofac.Module
    {
        private static ILifetimeScope _container;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(container => _container = container);
        }

        public static ILifetimeScope GetContainer()
        {
            return _container;
        }
    }
}
