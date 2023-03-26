﻿using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
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
        public IpAddressesController(IIpInfoViewerDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpAddressInfo>>> GetIpAddresses()
        {
            return Ok(await _dbRepository.GetIpAddresses(0, 500));
        }

    }
}
