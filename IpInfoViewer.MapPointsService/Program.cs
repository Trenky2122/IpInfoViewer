using System.Runtime.CompilerServices;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.MapPointsService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IIpInfoViewerDbRepository, IpInfoViewerDbRepository>();
        services.AddHostedService<MapPointsServiceWorker>();
    })
    .Build();

await host.RunAsync();
