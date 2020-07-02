using System;
using System.Net.Http;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Client.Extensions.ServiceConfiguration
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly GrpcWebHandler GrpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());

        /// <summary>
        /// Configures a <see cref="GrpcChannel"/> which uses a HTTP web connection.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to configure.</param>
        /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddGrpcWebChannel(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddSingleton<GrpcChannel>(services =>
            {
                var grpcHttpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler()));
                //var grpcHttpClient = new HttpClient(GrpcWebHandler);
                var navigationManager = services.GetRequiredService<NavigationManager>();
                var channelUri = navigationManager.BaseUri;
                var grpcWebChannel = GrpcChannel.ForAddress(channelUri, new GrpcChannelOptions {HttpClient = grpcHttpClient});

                return grpcWebChannel;
            });

            return serviceCollection;
        }

        /// <inheritdoc cref="AddAuthorisedGrpcClientCore{TClient}"/>
        public static IServiceCollection AddAuthorisedGrpcClient<TClient>(this IServiceCollection serviceCollection)
            where TClient : class
        {
            serviceCollection
                .AddAuthorisedGrpcClientCore<TClient>((_, __) => {});

            return serviceCollection;
        }

        /// <inheritdoc cref="AddAuthorisedGrpcClientCore{TClient}"/>
        public static IServiceCollection AddAuthorisedGrpcClient<TClient>(this IServiceCollection serviceCollection, Action<IServiceProvider, GrpcClientFactoryOptions> configureClient)
            where TClient : class
        {
            serviceCollection
                .AddAuthorisedGrpcClientCore<TClient>(configureClient);

            return serviceCollection;
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to add an authorised gRPC client.
        /// </summary>
        /// <typeparam name="TClient"> The type of the gRPC client. The type specified will be registered in the service collection as a transient service. </typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureClient">A delegate that is used to configure the gRPC client.</param>
        /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
        /// <remarks>See <see cref="GrpcClientServiceExtensions.AddGrpcClient(IServiceCollection, Action{GrpcClientFactoryOptions})"/>.</remarks>
        private static IServiceCollection AddAuthorisedGrpcClientCore<TClient>(this IServiceCollection serviceCollection, Action<IServiceProvider, GrpcClientFactoryOptions> configureClient)
            where TClient : class
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (configureClient == null) throw new ArgumentNullException(nameof(configureClient));

            serviceCollection
                .AddGrpcClient<TClient>((services, options) =>
                {
                    ConfigureGrpcClientAddress(services, options);
                    configureClient(services, options);
                })
                .ConfigurePrimaryHttpMessageHandler(_ => GrpcWebHandler)
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            return serviceCollection;
        }

        /// <summary>
        /// Configures the address for a <see cref="GrpcClientFactoryOptions"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <param name="options">Options for the gRPC client.</param>
        private static void ConfigureGrpcClientAddress(IServiceProvider serviceProvider, GrpcClientFactoryOptions options)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            options.Address = new Uri(navigationManager.BaseUri);
        }
    }
}
