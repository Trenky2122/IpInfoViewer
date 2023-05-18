using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Models.Enums;
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
        public async Task<ContentResult> GetColoredSvgMapAsync(string week, RequestedDataEnum requestedData, ScaleMode scaleMode)
        {
            return Content(await _countryFacade.GetColoredSvgMapForWeek(week, requestedData, scaleMode), "image/svg+xml");
        }

        [HttpGet("LastProcessedDate/")]
        public async Task<ActionResult<StringResponse?>> GetLatestProcessedWeekCountryPing()
        {
            return Ok(new StringResponse(await _countryFacade.GetLastProcessedWeek()));
        }
    }
}
