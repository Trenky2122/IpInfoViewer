using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.IpInfo;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace IpInfoViewer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IpAddressesController: ControllerBase
    {
        private readonly IIpInfoViewerDbRepository _dbRepository;
        private readonly IMapFacade _mapFacade;
        public IpAddressesController(IIpInfoViewerDbRepository dbRepository, IMapFacade mapFacade)
        {
            _dbRepository = dbRepository;
            _mapFacade = mapFacade;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpAddressInfo>>> GetIpAddresses()
        {
            return Ok(await _dbRepository.GetIpAddresses(0, 500));
        }


        [HttpGet("ipInfoMapLegend")]
        public ContentResult GetLegendForIpInfoMap(
            [FromQuery]List<int> counts,
            [FromQuery]List<float> radii,
            int pingUpperBound)
        {
            var sizeInformation = new List<(float Radius, int Count)>();
            for (int i = 0; i < 5; i++)
            {
                sizeInformation.Add((radii[i], counts[i]));
            }
            return Content(_mapFacade.GetIpMapLegend(sizeInformation, pingUpperBound), "image/svg+xml");
        }
    }
}
