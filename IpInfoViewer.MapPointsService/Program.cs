using System.Runtime.CompilerServices;
using IpInfoViewer.Libs.Abstractions;
using IpInfoViewer.Libs.Implementation;
using IpInfoViewer.MapPointsService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IIpInfoViewerDbRepository, IpInfoViewerDbRepository>();
        services.AddHostedService<MapPointsServiceWorker>();
    })
    .Build();

await host.RunAsync();
