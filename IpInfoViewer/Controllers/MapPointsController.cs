using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using Microsoft.AspNetCore.Mvc;

namespace IpInfoViewer.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MapPointsController: ControllerBase
    {
        private readonly IMapPointsFacade _mapPointsFacade;

        public MapPointsController(IMapPointsFacade mapPointsFacade)
        {
            _mapPointsFacade = mapPointsFacade;
        }
        /// <summary>
        /// Gets map points for week of input date
        /// </summary>
        /// <param name="dayFromWeek">Date from wanted week</param>
        /// <returns>Map points</returns>
        [HttpGet("ForDayOfWeek/{dayFromWeek}")]
        public async Task<ActionResult<IEnumerable<MapPoint>>> GetMapPointsForDayOfWeek(DateTime dayFromWeek)
        {
            return Ok(await _mapPointsFacade.GetMapPointsForDayOfWeek(dayFromWeek));
        }

        /// <summary>
        /// Gets map points for week
        /// </summary>
        /// <param name="week">Week in HTML (ISO_8601) format e.g. (2023-W25)</param>
        /// <returns>Map points</returns>
        [HttpGet("ForWeek/{week}")]
        public async Task<ActionResult<IEnumerable<MapPoint>>> GetMapPoinsForWeek(string week)
        {
            return Ok(await _mapPointsFacade.GetMapPoinsForWeek(week));
        }

        /// <summary>
        /// Gets last date when map points are processed
        /// </summary>
        /// <returns></returns>
        [HttpGet("LastProcessedDate")]
        public async Task<ActionResult<StringResponse?>> GetLatestProcessedWeekIpInfo()
        {
            return Ok(new StringResponse(await _mapPointsFacade.GetLastProcessedWeek()));
        }



        [HttpGet("MapPointsMapLegend")]
        public ContentResult GetLegendForIpInfoMap(
            [FromQuery] List<int> counts,
            [FromQuery] List<float> radii,
            int pingUpperBound)
        {
            var sizeInformation = new List<(float Radius, int Count)>();
            for (int i = 0; i < 5; i++)
            {
                sizeInformation.Add((radii[i], counts[i]));
            }
            return Content(_mapPointsFacade.GetIpMapLegend(sizeInformation, pingUpperBound), "image/svg+xml");
        }
    }
}
