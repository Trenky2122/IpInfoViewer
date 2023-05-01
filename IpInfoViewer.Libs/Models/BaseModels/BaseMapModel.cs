namespace IpInfoViewer.Libs.Models.BaseModels
{
    public class BaseMapModel: BaseWeeklyProcessedModel
    {
        public int IpAddressesCount { get; set; }
        public float AveragePingRtT { get; set; }
    }
}
