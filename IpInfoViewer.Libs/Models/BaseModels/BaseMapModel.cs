namespace IpInfoViewer.Libs.Models.BaseModels
{
    public class BaseMapModel: BaseWeeklyProcessedModel
    {
        public int IpAddressesCount { get; set; }
        public float AveragePingRtT { get; set; }
        public float MaximumPingRtT { get; set; }
        public float MinimumPingRtT { get; set; }
    }
}
