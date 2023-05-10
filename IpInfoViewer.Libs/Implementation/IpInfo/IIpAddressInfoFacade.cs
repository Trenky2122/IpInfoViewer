namespace IpInfoViewer.Libs.Implementation.IpInfo
{
    public interface IIpAddressInfoFacade
    {
        Task<int> ProcessLine(string line);
        Task ExecuteSeedingAsync(CancellationToken stoppingToken);
    }
}
