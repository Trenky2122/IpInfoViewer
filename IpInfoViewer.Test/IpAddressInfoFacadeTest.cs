using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.IpInfo;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Models.MFile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpInfoViewer.Test
{
    public class IpAddressInfoFacadeTest
    {
        [Fact]
        public async Task Test_ProcessLine()
        {
            string line = "1.0.1.0,1.0.3.255,AS,CN,Fujian,Qingzhou,26.4837,117.925";
            var mfileMock = new Mock<IMFileDbRepository>();
            mfileMock.Setup(repo => repo.GetHostsInRange(IPAddress.Parse("1.0.1.0"), IPAddress.Parse("1.0.3.255")))
                .ReturnsAsync(new List<Host>()
                {
                    new()
                    {
                        IpAddr = (IPAddress.Parse("1.0.1.1"), 32)
                    },
                    new()
                    {
                        IpAddr = (IPAddress.Parse("1.0.1.2"), 32)
                    },
                    new()
                    {
                        IpAddr = (IPAddress.Parse("1.0.1.3"), 32)
                    }
                });
            var localMock = new Mock<IIpInfoViewerDbRepository>();
            localMock.Setup(repo => repo.SaveIpAddressInfoAsync(It.IsAny<IpAddressInfo>())).Callback<IpAddressInfo>(
                info =>
                {
                    Assert.Equal("CN", info.CountryCode);
                    Assert.Equal(26.4837, info.Latitude);
                    Assert.Equal(117.925, info.Longitude);
                    Assert.Equal("Qingzhou", info.City);
                });
            var facade = new IpAddressInfoFacade(mfileMock.Object, localMock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<IpAddressInfoFacade>>());
            var result = await facade.ProcessLine(line);
            Assert.Equal(3, result);
            localMock.Verify(repo => repo.SaveIpAddressInfoAsync(It.IsAny<IpAddressInfo>()), Times.Exactly(3));
        }
    }
}
