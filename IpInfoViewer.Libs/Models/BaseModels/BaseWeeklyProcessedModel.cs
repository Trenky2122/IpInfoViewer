namespace IpInfoViewer.Libs.Models.BaseModels
{
    public class BaseWeeklyProcessedModel: BaseModel
    {
        /// <summary>
        /// Week in HTML (ISO_8601) format e.g. (2023-W25)
        /// </summary>
        public string Week { get; set; }
    }
}
