using System.Globalization;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace IpInfoViewer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MapController: ControllerBase
    {
        private readonly IIpInfoViewerDbRepository _dbRepository;
        public MapController(IIpInfoViewerDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        /// <summary>
        /// Gets map points for week of input date
        /// </summary>
        /// <param name="dayFromWeek">Date from wanted week</param>
        /// <returns>Map points</returns>
        [HttpGet("ForDayOfWeek/{dayFromWeek}")]
        public async Task<ActionResult<IEnumerable<MapIpAddressesRepresentation>>> GetMapForDayOfWeek(DateTime dayFromWeek)
        {
            return Ok(await _dbRepository.GetMapForWeek(new Week(dayFromWeek)));
        }

        /// <summary>
        /// Gets map points for week
        /// </summary>
        /// <param name="week">Week in HTML (ISO_8601) format e.g. (2023-W25)</param>
        /// <returns>Map points</returns>
        [HttpGet("ForWeek/{week}")]
        public async Task<ActionResult<IEnumerable<MapIpAddressesRepresentation>>> GetMapForWeek(string week)
        {
            return Ok(await _dbRepository.GetMapForWeek(new Week(week)));
        }
    }
}
