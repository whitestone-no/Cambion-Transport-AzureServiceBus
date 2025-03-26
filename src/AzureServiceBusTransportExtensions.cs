using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Whitestone.Cambion.Interfaces;

namespace Whitestone.Cambion.Transport.AzureSericeBus
{
    public static class AzureServiceBusTransportExtensions
    {
        public static ICambionSerializerBuilder UseAzureServiceBusTransport(this ICambionTransportBuilder builder, Action<AzureServiceBusConfig> configure)
        {
            return UseAzureServiceBusTransport(builder, null, configure, null);
        }

        public static ICambionSerializerBuilder UseAzureServiceBusTransport(this ICambionTransportBuilder builder, IConfiguration configuration, string cambionConfigurationKey = "Cambion")
        {
            return UseAzureServiceBusTransport(builder, configuration, null, cambionConfigurationKey);
        }

        public static ICambionSerializerBuilder UseAzureServiceBusTransport(this ICambionTransportBuilder builder, IConfiguration configuration, Action<AzureServiceBusConfig> configure, string cambionConfigurationKey = "Cambion")
        {
            builder.Services.Replace(new ServiceDescriptor(typeof(ITransport), typeof(AzureServiceBusTransport), ServiceLifetime.Singleton));

            if (configuration != null)
            {
                string assemblyName = typeof(AzureServiceBusTransport).Assembly.GetName().Name;

                IConfigurationSection config = configuration.GetSection(cambionConfigurationKey).GetSection("Transport").GetSection(assemblyName);

                if (config.Exists())
                {
                    builder.Services.Configure<AzureServiceBusConfig>(config);
                }
            }

            builder.Services.AddOptions<AzureServiceBusConfig>()
                .Configure(conf =>
                {
                    configure?.Invoke(conf);
                });

            return (ICambionSerializerBuilder)builder;
        }
    }
}
