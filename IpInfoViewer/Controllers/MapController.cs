using System.Globalization;
using System.Net.Mime;
using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Map;
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
        private readonly ICountryPingInfoFacade _countryFacade;
        public MapController(IIpInfoViewerDbRepository dbRepository, ICountryPingInfoFacade countryFacade)
        {
            _dbRepository = dbRepository;
            _countryFacade = countryFacade;
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

        [HttpGet("ColoredMap/{week}")]
        public async Task<ContentResult> GetColoredSvgMapAsync(string week)
        {
            return Content(await _countryFacade.GetColoredSvgMapForWeek(new Week(week)), "image/svg+xml");
        }

        [HttpGet("lastProcessedDate")]
        public async Task<ActionResult<string?>> GetLatestProcessedWeek()
        {
            DateTime? lastProcessedDate = await _dbRepository.GetLastDateWhenCountriesAreProcessed();
            if (!lastProcessedDate.HasValue)
                return Ok(null);
            return Ok(new Week(lastProcessedDate.Value).ToString());
        }
    }
}
