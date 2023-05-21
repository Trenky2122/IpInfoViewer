using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpInfoViewer.Test
{
    public class DateTimeUtilitiesTest
    {
        [Theory]
        [InlineData(2023, 1, 1, "2022-W52")]
        [InlineData(2023, 1, 2, "2023-W01")]
        [InlineData(2024, 12, 31, "2025-W01")]
        [InlineData(2027, 1, 1, "2026-W53")]
        [InlineData(2027, 1, 4, "2027-W01")]
        public void Test_Week(int year, int month, int day, string expectedResult)
        {
            DateTime date = new(year, month, day);
            Week week = new(date);
            string result = week.ToString();
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(2023, 1, 1, 2023, 1, 2, 2)]
        [InlineData(2023, 1, 2, 2023, 1, 3, 1)]
        [InlineData(2023, 1, 2, 2023, 2, 1, 5)]
        [InlineData(2022, 1, 1, 2023, 1, 1, 53)]
        [InlineData(2022, 1, 1, 2023, 1, 2, 54)]
        public void Test_GetWeeksFromTo(int yearFrom, int monthFrom, int dayFrom, int yearTo, int monthTo, int dayTo, int expectedCount)
        {
            DateTime dateFrom = new(yearFrom, monthFrom, dayFrom);
            DateTime dateTo = new(yearTo, monthTo, dayTo);
            var result = DateTimeUtilities.GetWeeksFromTo(dateFrom, dateTo).ToList();
            Assert.Equal(expectedCount, result.Count);
        }

        [Fact]
        public void Test_GetWeeksFromTo_Empty()
        {
            var result = DateTimeUtilities.GetWeeksFromTo(DateTime.Today, DateTime.Today.AddDays(-8));
            Assert.Empty(result);
        }
    }
}
