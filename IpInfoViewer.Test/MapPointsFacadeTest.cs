using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrapeCity.Documents.Text;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpInfoViewer.Test
{
    public class MapPointsFacadeTest
    {
        [Fact]
        public async Task Test_GetMapForWeekAsync()
        {
            Week week = new("2022-W01");
            List<MapPoint> points = new()
            {
                new MapPoint()
                {
                    Id = 1,
                    AveragePingRtT = 2,
                    IpAddressesCount = 3,
                    Latitude = 4,
                    Longitude = 5,
                    MaximumPingRtT = 6,
                    MinimumPingRtT = 7,
                    Week = week.ToString()
                },
                new MapPoint()
                {
                    Id = 8,
                    AveragePingRtT = 9,
                    IpAddressesCount = 10,
                    Latitude = 11,
                    Longitude = 12,
                    MaximumPingRtT = 13,
                    MinimumPingRtT = 14,
                    Week = week.Next().ToString()
                }
            };
            var localDbMock = new Mock<IIpInfoViewerDbRepository>();
            localDbMock.Setup(repo => repo.GetMapForWeekAsync(week))
                .ReturnsAsync(points);
            MapPointsFacade facade = new(localDbMock.Object, Mock.Of<IMFileDbRepository>(), Mock.Of<ILogger<MapPointsFacade>>());
            var result = (await facade.GetMapPointsForWeek(week.ToString())).ToList();
            Assert.Equal(2, result.Count());
            Assert.Equal(points[0], result.First());
            Assert.Equal(points[1], result.Last());
        }

        [Theory]
        [InlineData("2022-W01")]
        [InlineData("2022-W54")]
        [InlineData("asdkfjsa")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Test_LastProcessedDate(string? retval)
        {
            var localDbMock = new Mock<IIpInfoViewerDbRepository>();
            localDbMock.Setup(repo => repo.GetLastDateWhenMapIsProcessedAsync())
                .ReturnsAsync(retval);
            MapPointsFacade facade = new(localDbMock.Object, Mock.Of<IMFileDbRepository>(), Mock.Of<ILogger<MapPointsFacade>>());
            var result = await facade.GetLastProcessedWeek();
            Assert.Equal(retval, result);
        }
    }
}
