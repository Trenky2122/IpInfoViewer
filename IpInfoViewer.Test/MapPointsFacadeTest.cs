using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [Fact]
        public async Task Test_ProcessWeekAsync()
        {
            Week week = new(DateTime.Now);
            var weekPingData = new List<WeekPingData>()
            {
                new()
                {
                    Average = 10,
                    IpAddress = (IPAddress.Parse("1.1.1.1"), 32),
                    Minimum = 5,
                    Maximum = 15
                },
                new()
                {
                    Average = 12,
                    IpAddress = (IPAddress.Parse("1.1.1.2"), 32),
                    Minimum = 6,
                    Maximum = 17
                },
                new()
                {
                    Average = 100,
                    IpAddress = (IPAddress.Parse("2.1.1.1"), 32),
                    Minimum = 50,
                    Maximum = 150
                },
                new()
                {
                    Average = 120,
                    IpAddress = (IPAddress.Parse("2.1.1.2"), 32),
                    Minimum = 60,
                    Maximum = 170
                },
                new()
                {
                    Average = 120,
                    IpAddress = (IPAddress.Parse("3.1.1.1"), 32),
                    Minimum = 60,
                    Maximum = 150
                }
            };
            var mfileMock = new Mock<IMFileDbRepository>();
            mfileMock.Setup(repo => repo.GetWeekPingData(week)).ReturnsAsync(weekPingData);
            var ipToCountry = new List<IpAddressInfo>()
            {
                new IpAddressInfo()
                {
                    Latitude = 5,
                    Longitude = 5,
                    CountryCode = "SK",
                    IpStringValue = "1.1.1.1"
                },
                new IpAddressInfo()
                {
                    Latitude = 4,
                    Longitude = 4,
                    CountryCode = "SK",
                    IpStringValue = "1.1.1.2"
                },
                new IpAddressInfo()
                {
                    Latitude = 14,
                    Longitude = 14,
                    CountryCode = "CZ",
                    IpStringValue = "2.1.1.1"
                },
                new IpAddressInfo()
                {
                    Latitude = 13,
                    Longitude = 13,
                    CountryCode = "CZ",
                    IpStringValue = "2.1.1.2"
                },
                new IpAddressInfo()
                {
                    Latitude = 52,
                    Longitude = 4,
                    CountryCode = "HU",
                    IpStringValue = "3.1.1.1"
                }
            };

            var addressesGroupedByLocation = ipToCountry.GroupBy(MapPointsFacade.GetApproximateLocation);
            var localMock = new Mock<IIpInfoViewerDbRepository>();
            localMock.Setup(repo => repo.SaveMapIpAddressRepresentationsAsync(It.IsAny<IEnumerable<MapPoint>>()))
                .Callback<IEnumerable<MapPoint>>(result =>
                {
                    var resultList = result.ToList();
                    Assert.Equal(3, resultList.Count);
                    Assert.Equal(4.5, resultList[0].Latitude);
                    Assert.Equal(4.5, resultList[0].Longitude);
                    Assert.Equal(2, resultList[0].IpAddressesCount);
                    Assert.Equal(11, resultList[0].AveragePingRtT);
                    Assert.Equal(5, resultList[0].MinimumPingRtT);
                    Assert.Equal(17, resultList[0].MaximumPingRtT);

                    Assert.Equal(13.5, resultList[1].Latitude);
                    Assert.Equal(13.5, resultList[1].Longitude);
                    Assert.Equal(2, resultList[1].IpAddressesCount);
                    Assert.Equal(110, resultList[1].AveragePingRtT);
                    Assert.Equal(50, resultList[1].MinimumPingRtT);
                    Assert.Equal(170, resultList[1].MaximumPingRtT);

                    Assert.Equal(52, resultList[2].Latitude);
                    Assert.Equal(4, resultList[2].Longitude);
                    Assert.Equal(1, resultList[2].IpAddressesCount);
                    Assert.Equal(120, resultList[2].AveragePingRtT);
                    Assert.Equal(60, resultList[2].MinimumPingRtT);
                    Assert.Equal(150, resultList[2].MaximumPingRtT);
                }).Returns(Task.CompletedTask);

            var facade = new MapPointsFacade(localMock.Object, mfileMock.Object, Mock.Of<ILogger<MapPointsFacade>>());
            await facade.ProcessWeekAsync(week, addressesGroupedByLocation);
            localMock.Verify(repo => repo.SaveMapIpAddressRepresentationsAsync(It.IsAny<IEnumerable<MapPoint>>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Test_ExecuteSeedingAsync()
        {
            var week = new Week(DateTime.Now.AddDays(-14));
            var weekPingData = new List<WeekPingData>()
            {
                new()
                {
                    Average = 10,
                    IpAddress = (IPAddress.Parse("1.1.1.1"), 32),
                    Minimum = 5,
                    Maximum = 15
                },
                new()
                {
                    Average = 12,
                    IpAddress = (IPAddress.Parse("1.1.1.2"), 32),
                    Minimum = 6,
                    Maximum = 17
                },
                new()
                {
                    Average = 100,
                    IpAddress = (IPAddress.Parse("2.1.1.1"), 32),
                    Minimum = 50,
                    Maximum = 150
                },
                new()
                {
                    Average = 120,
                    IpAddress = (IPAddress.Parse("2.1.1.2"), 32),
                    Minimum = 60,
                    Maximum = 170
                },
                new()
                {
                    Average = 120,
                    IpAddress = (IPAddress.Parse("3.1.1.1"), 32),
                    Minimum = 60,
                    Maximum = 150
                }
            };
            var ipToCountry = new List<IpAddressInfo>()
            {
                new IpAddressInfo()
                {
                    Latitude = 5,
                    Longitude = 5,
                    CountryCode = "SK",
                    IpStringValue = "1.1.1.1"
                },
                new IpAddressInfo()
                {
                    Latitude = 4,
                    Longitude = 4,
                    CountryCode = "SK",
                    IpStringValue = "1.1.1.2"
                },
                new IpAddressInfo()
                {
                    Latitude = 14,
                    Longitude = 14,
                    CountryCode = "CZ",
                    IpStringValue = "2.1.1.1"
                },
                new IpAddressInfo()
                {
                    Latitude = 13,
                    Longitude = 13,
                    CountryCode = "CZ",
                    IpStringValue = "2.1.1.2"
                },
                new IpAddressInfo()
                {
                    Latitude = 52,
                    Longitude = 4,
                    CountryCode = "HU",
                    IpStringValue = "3.1.1.1"
                }
            };
            var mfileMock = new Mock<IMFileDbRepository>();
            mfileMock.Setup(repo => repo.GetWeekPingData(week.Next())).ReturnsAsync(weekPingData);
            var localMock = new Mock<IIpInfoViewerDbRepository>();
            localMock.Setup(repo => repo.GetLastDateWhenMapIsProcessedAsync()).ReturnsAsync(week.ToString());
            localMock.Setup(repo => repo.GetIpAddressesAsync(0, int.MaxValue)).ReturnsAsync(ipToCountry);
            localMock.Setup(repo => repo.SaveMapIpAddressRepresentationsAsync(It.IsAny<IEnumerable<MapPoint>>()))
                .Callback<IEnumerable<MapPoint>>(result =>
                {
                    var resultList = result.ToList();
                    Assert.Equal(3, resultList.Count);
                    Assert.Equal(4.5, resultList[0].Latitude);
                    Assert.Equal(4.5, resultList[0].Longitude);
                    Assert.Equal(2, resultList[0].IpAddressesCount);
                    Assert.Equal(11, resultList[0].AveragePingRtT);
                    Assert.Equal(5, resultList[0].MinimumPingRtT);
                    Assert.Equal(17, resultList[0].MaximumPingRtT);

                    Assert.Equal(13.5, resultList[1].Latitude);
                    Assert.Equal(13.5, resultList[1].Longitude);
                    Assert.Equal(2, resultList[1].IpAddressesCount);
                    Assert.Equal(110, resultList[1].AveragePingRtT);
                    Assert.Equal(50, resultList[1].MinimumPingRtT);
                    Assert.Equal(170, resultList[1].MaximumPingRtT);

                    Assert.Equal(52, resultList[2].Latitude);
                    Assert.Equal(4, resultList[2].Longitude);
                    Assert.Equal(1, resultList[2].IpAddressesCount);
                    Assert.Equal(120, resultList[2].AveragePingRtT);
                    Assert.Equal(60, resultList[2].MinimumPingRtT);
                    Assert.Equal(150, resultList[2].MaximumPingRtT);
                }).Returns(Task.CompletedTask);
            var loggerMock = new Mock<ILogger<MapPointsFacade>>();
            var facade = new MapPointsFacade(localMock.Object, mfileMock.Object, loggerMock.Object);
            await facade.ExecuteSeedingAsync(CancellationToken.None);
            localMock.Verify(repo => repo.SaveMapIpAddressRepresentationsAsync(It.IsAny<IEnumerable<MapPoint>>()), Times.Exactly(1));
            loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);

        }
        
    }
}
