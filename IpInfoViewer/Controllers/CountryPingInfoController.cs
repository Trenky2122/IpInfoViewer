using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace IpInfoViewer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryPingInfoController: ControllerBase
    {
        private readonly IIpInfoViewerDbRepository _dbRepository;
        private readonly ICountryPingInfoFacade _countryFacade;

        public CountryPingInfoController(IIpInfoViewerDbRepository dbRepository, ICountryPingInfoFacade countryFacade)
        {
            _dbRepository = dbRepository;
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
            DateTime? lastProcessedDate = await _dbRepository.GetLastDateWhenCountriesAreProcessed();
            if (!lastProcessedDate.HasValue)
                return Ok(null);
            return Ok(new StringResponse(new Week(lastProcessedDate.Value).ToString()));
        }
    }
}
