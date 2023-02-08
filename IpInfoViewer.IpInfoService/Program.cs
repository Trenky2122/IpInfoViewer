using IpInfoViewer.IpInfoService;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.IpInfo;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IIpInfoViewerDbRepository, IpInfoViewerDbRepository>();
        services.AddSingleton<IMFileDbRepository, MFileDbRepository>();
        services.AddSingleton<IIpAddressInfoFacade, IpAddressInfoFacade>();
        services.AddHostedService<IpInfoServiceWorker>();
    })
    .Build();

await host.RunAsync();
