using System.Runtime.CompilerServices;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.MapPointsService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMFileDbRepository, MFileDbRepository>();
        services.AddSingleton<IIpInfoViewerDbRepository, IpInfoViewerDbRepository>();
        services.AddSingleton<IMapFacade, MapFacade>();
        services.AddHostedService<MapPointsServiceWorker>();
    })
    .Build();
await host.RunAsync();
