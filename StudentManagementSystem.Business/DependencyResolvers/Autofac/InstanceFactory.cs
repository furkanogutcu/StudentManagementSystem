using Autofac;

namespace StudentManagementSystem.Business.DependencyResolvers.Autofac
{
    public class InstanceFactory
    {
        public static T GetInstance<T>(params Module[] modules)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacBusinessModule());
            IContainer container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }
    }
}
