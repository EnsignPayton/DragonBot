using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace DragonBot.Utilities
{
    public class ServiceLocatorFactory
    {
        private readonly ContainerBuilder _builder;
        private IEnumerable<Type> _assemblyTypes;
        private IEnumerable<Type> _implementationTypes;
        private IEnumerable<Type> _singletonTypes;
        private IDictionary<Type, object> _instances;
        private Func<IServiceCollection, IServiceCollection> _serviceConfig;

        public ServiceLocatorFactory()
        {
            _builder = new ContainerBuilder();
            _assemblyTypes = Enumerable.Empty<Type>();
            _implementationTypes = Enumerable.Empty<Type>();
            _singletonTypes = Enumerable.Empty<Type>();
            _instances = new Dictionary<Type, object>();
        }

        #region Config

        public ServiceLocatorFactory WithAssemblyTypes(IEnumerable<Type> assemblyTypes)
        {
            _assemblyTypes = assemblyTypes ?? throw new ArgumentNullException(nameof(assemblyTypes));
            return this;
        }

        public ServiceLocatorFactory WithAssemblyTypes(params Type[] assemblyTypes) =>
            WithAssemblyTypes((IEnumerable<Type>)assemblyTypes);

        public ServiceLocatorFactory WithImplementationTypes(IEnumerable<Type> implementationTypes)
        {
            _implementationTypes = implementationTypes ?? throw new ArgumentNullException(nameof(implementationTypes));
            return this;
        }

        public ServiceLocatorFactory WithImplementationTypes(params Type[] implementationTypes) =>
            WithImplementationTypes((IEnumerable<Type>)implementationTypes);

        public ServiceLocatorFactory WithSingletonTypes(IEnumerable<Type> singletonTypes)
        {
            _singletonTypes = singletonTypes ?? throw new ArgumentNullException(nameof(singletonTypes));
            return this;
        }

        public ServiceLocatorFactory WithSingletonTypes(params Type[] singletonTypes) =>
            WithSingletonTypes((IEnumerable<Type>)singletonTypes);

        public ServiceLocatorFactory WithServiceConfig(Func<IServiceCollection, IServiceCollection> serviceConfig)
        {
            _serviceConfig = serviceConfig ?? throw new ArgumentNullException(nameof(serviceConfig));
            return this;
        }

        public ServiceLocatorFactory WithInstances(IDictionary<Type, object> instances)
        {
            _instances = instances ?? throw new ArgumentNullException(nameof(instances));
            return this;
        }

        public ServiceLocatorFactory WithInstances(params KeyValuePair<Type, object>[] instances) =>
            WithInstances(instances.ToDictionary(x => x.Key, x => x.Value));

        public ServiceLocatorFactory WithInstances(params (Type Key, object Value)[] instances) =>
            WithInstances(instances.ToDictionary(x => x.Key, x => x.Value));

        #endregion

        #region Registration

        public IServiceLocator Build()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                RegisterAssemblies();
                RegisterServices();
                RegisterConfiguration();
                RegisterImplementations();
                RegisterSingletons();
                RegisterInstances();

                var container = _builder.Build();
                var serviceLocator = new AutofacServiceLocator(container);

                ServiceLocator.SetLocatorProvider(() => serviceLocator);

                return serviceLocator;
            }
            catch (Exception ex)
            {
                logger.Error($"Error in Composition Root: {ex}");
                throw;
            }
        }

        private void RegisterAssemblies()
        {
            var assemblies = _assemblyTypes
                .Select(Assembly.GetAssembly)
                .ToList();

            assemblies.Add(Assembly.GetExecutingAssembly());

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                assemblies.Add(entryAssembly);

            _builder.RegisterAssemblyTypes(assemblies.ToArray()).AsSelf();
        }

        private void RegisterServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(x =>
            {
                x.AddNLog(); 
            });

            _serviceConfig?.Invoke(serviceCollection);

            _builder.Populate(serviceCollection);
        }

        private void RegisterConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true)
                .Build();
            var settings = new ApplicationSettings();
            config.GetSection("ApplicationSettings").Bind(settings);
            _builder.RegisterInstance(settings)
                .As<ApplicationSettings>()
                .SingleInstance();
        }

        private void RegisterImplementations()
        {
            var instanceTypes = _implementationTypes.ToArray();
            _builder.RegisterTypes(instanceTypes).AsImplementedInterfaces();
        }

        private void RegisterSingletons()
        {
            var singletonTypes = _singletonTypes.ToArray();
            _builder.RegisterTypes(singletonTypes).AsImplementedInterfaces().SingleInstance();
            _builder.RegisterTypes(singletonTypes).SingleInstance();
        }

        private void RegisterInstances()
        {
            foreach (var instance in _instances)
            {
                _builder.RegisterInstance(instance.Value).As(instance.Key);
            }
        }

        #endregion
    }
}
