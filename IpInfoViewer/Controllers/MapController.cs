using IpInfoViewer.Libs.Abstractions;
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

        [HttpGet("mapForWeek/{dayFromWeek}")]
        public async Task<ActionResult<IEnumerable<MapIpAddressesRepresentation>>> GetMapForWeek(DateTime dayFromWeek)
        {
            return Ok(await _dbRepository.GetMapForWeek(new Week(dayFromWeek)));
        }
    }
}
