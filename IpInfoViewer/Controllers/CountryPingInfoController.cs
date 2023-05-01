using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace IpInfoViewer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryPingInfoController: ControllerBase
    {
        private readonly ICountryPingInfoFacade _countryFacade;

        public CountryPingInfoController(ICountryPingInfoFacade countryFacade)
        {
            _countryFacade = countryFacade;
        }

        [HttpGet("ColoredMap/{week}")]
        public async Task<ContentResult> GetColoredSvgMapAsync(string week, bool fullScale)
        {
            return Content(await _countryFacade.GetColoredSvgMapForWeek(new Week(week), fullScale), "image/svg+xml");
        }

        [HttpGet("LastProcessedDate/")]
        public async Task<ActionResult<StringResponse?>> GetLatestProcessedWeekCountryPing()
        {
            return Ok(_countryFacade.GetLastProcessedWeek());
        }
    }
}
