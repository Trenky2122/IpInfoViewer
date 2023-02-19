using IpInfoViewer.CountryPingInfoService;
using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IIpInfoViewerDbRepository, IpInfoViewerDbRepository>();
        services.AddSingleton<IMFileDbRepository, MFileDbRepository>();
        services.AddSingleton<ICountryPingInfoFacade, CountryPingInfoFacade>();
        services.AddHostedService<CountryPingInfoWorker>();
    })
    .Build();

await host.RunAsync();
