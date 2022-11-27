using IpInfoViewer.Libs.Implementation;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;

IpInfoViewerDbRepository repository = new IpInfoViewerDbRepository("Server=127.0.0.1;Port=5432;Database=ipinfoviewerprocesseddb;User Id=postgres;Password=0000;Include Error Detail=true;");
await repository.SeedTables();
foreach (var line in File.ReadLines(@"C:\Users\Peter\Downloads\dbip-city-lite-2022-11.csv\dbip-city-lite-2022-11.csv"))
{
    if (line.Contains("\"")) continue;
    await repository.SaveIpAddressInfo(line);
}