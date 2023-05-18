using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrapeCity.Documents.Text;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpInfoViewer.Test
{
    public class MapPointsFacadeTest
    {
        //[Theory]
        //[InlineData(2023, 1, 1, "2022-W52")]
        //[InlineData(2023, 1, 2, "2023-W01")]
        //[InlineData(2024, 12, 31, "2025-W01")]
        //[InlineData(2027, 1, 1, "2026-W53")]
        //[InlineData(2027, 1, 4, "2027-W01")]
        //public async Task Test_GetLastDateWhenMapIsProcessed_Ok(int year, int month, int day, string expectedResult)
        //{
        //    var mockDb = new Mock<IIpInfoViewerDbRepository>();
        //    mockDb.Setup(repository => repository.GetLastDateWhenMapIsProcessed()).ReturnsAsync(new DateTime(year, month, day));
        //    var facade = new MapPointsFacade(mockDb.Object, new Mock<IMFileDbRepository>().Object, Mock.Of<ILogger<MapPointsFacade>>());
        //    var result = await facade.GetLastProcessedWeek();
        //    Assert.NotNull(result);
        //    Assert.Equal(expectedResult, result);
        //}
    }
}
