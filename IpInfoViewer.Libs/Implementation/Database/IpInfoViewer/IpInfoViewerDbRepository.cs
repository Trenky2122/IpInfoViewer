using System.Globalization;
using System.Net;
using System.Text;
using Dapper;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace IpInfoViewer.Libs.Implementation.Database.IpInfoViewer
{
    public class IpInfoViewerDbRepository : IIpInfoViewerDbRepository
    {
        private readonly string _connectionString;

        public IpInfoViewerDbRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IpInfoViewerDbRepository(IConfiguration configuration)
        {
            _connectionString = configuration["IpInfoViewerProcessedConnectionString"];
        }

        #region Private helpers

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        #endregion

        #region Create Tables


        public async Task SeedTables()
        {
            await using var connection = CreateConnection();
            
            connection.Open();
            await CreateIpTable(connection);
            await CreateMapIpRepresentationTable(connection);
            await CreateChartsTable(connection);
            await CreateCountryPingInfoTable(connection);
        }

        private Task CreateIpTable(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS IpAddresses (" +
                         "Id SERIAL PRIMARY KEY," +
                         "IpValue cidr," +
                         "CountryCode varchar(2)," +
                         "City varchar(80)," +
                         "Latitude float," +
                         "Longitude float);" +
                         "CREATE UNIQUE INDEX IF NOT EXISTS index1 ON IpAddresses (IpValue);";
            return connection.ExecuteAsync(sql);
        }

        private Task CreateMapIpRepresentationTable(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS MapIpRepresentation (" +
                         "Id SERIAL PRIMARY KEY," +
                         "Latitude float," +
                         "Longitude float," +
                         "IpAddressesCount int, " +
                         "AveragePingRtT int, " +
                         "ValidFrom Date, " +
                         "ValidTo Date);" +
                         "CREATE UNIQUE INDEX IF NOT EXISTS index2 ON MapIpRepresentation (Latitude, Longitude, ValidFrom, ValidTo);";
            return connection.ExecuteAsync(sql);
        }

        private Task CreateChartsTable(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS Charts (" +
                         "Id SERIAL PRIMARY KEY," +
                         "Name Varchar(50)," +
                         "XAxis Varchar(50)," +
                         "YAxis Varchar(50)," +
                         "ChartType int);";
            return connection.ExecuteAsync(sql);
        }

        private Task CreateCountryPingInfoTable(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS CountryPingInfo (" +
                         "Id SERIAL PRIMARY KEY," +
                         "CountryCode varchar(2)," +
                         "IpAddressesCount int, " +
                         "AveragePingRtT int, " +
                         "ValidFrom Date, " +
                         "ValidTo Date);" +
                         "CREATE UNIQUE INDEX IF NOT EXISTS index3 ON CountryPingInfo (CountryCode, ValidFrom, ValidTo);";
            return connection.ExecuteAsync(sql);
        }

        #endregion

        public async Task SaveIpAddressInfo(IpAddressInfo address)
        {
            await using var connection = CreateConnection();
            connection.Open();
            string sql = "INSERT INTO IpAddresses (IpValue, CountryCode, City, Latitude, Longitude) " +
                         "VALUES (@IpValue, @CountryCode, @City, @Latitude, @Longitude)";
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("IpValue", NpgsqlDbType.Cidr, address.IpValue);
            command.Parameters.AddWithValue("CountryCode", NpgsqlDbType.Varchar, address.CountryCode);
            command.Parameters.AddWithValue("City", NpgsqlDbType.Varchar, address.City);
            command.Parameters.AddWithValue("Latitude", NpgsqlDbType.Double, address.Latitude);
            command.Parameters.AddWithValue("Longitude", NpgsqlDbType.Double, address.Longitude);
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }

        public async Task SaveMapIpAddressRepresentation(MapPoint representation)
        {
            await using var connection = CreateConnection();
            string sql = "INSERT INTO MapIpRepresentation (Latitude, Longitude, IpAddressesCount, AveragePingRtT, ValidFrom, ValidTo) " +
                         "VALUES (@Latitude, @Longitude, @IpAddressesCount, @AveragePingRtT, @ValidFrom, @ValidTo)";
            await connection.ExecuteAsync(sql, representation);
        }

        public async Task SaveCountryPingInfo(CountryPingInfo countryPingInfo)
        {
            await using var connection = CreateConnection();
            string sql = "INSERT INTO CountryPingInfo (CountryCode, IpAddressesCount, AveragePingRtT, ValidFrom, ValidTo) " +
                         "VALUES (@CountryCode, @IpAddressesCount, @AveragePingRtT, @ValidFrom, @ValidTo)";
            await connection.ExecuteAsync(sql, countryPingInfo);
        }

        public async Task<IEnumerable<IpAddressInfo>> GetIpAddresses(int offset = 0, int limit = int.MaxValue)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<IpAddressInfo>("SELECT * FROM IpAddresses LIMIT @limit OFFSET @offset", new { limit, offset });
        }
        
        public async Task<IEnumerable<MapPoint>> GetMapForWeek(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<MapPoint>("SELECT * FROM MapIpRepresentation WHERE @Tuesday BETWEEN ValidFrom AND ValidTo", new { tuesday = week.Tuesday });
        }

        public async Task<DateTime?> GetLastDateWhenMapIsProcessed()
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DateTime?>("SELECT ValidTo FROM MapIpRepresentation ORDER BY ValidTo DESC LIMIT 1");
        }

        public async Task<DateTime?> GetLastDateWhenCountriesAreProcessed()
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DateTime?>("SELECT ValidTo FROM CountryPingInfo ORDER BY ValidTo DESC LIMIT 1");
        }
        public async Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeek(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<CountryPingInfo>("SELECT * FROM CountryPingInfo WHERE @Tuesday BETWEEN ValidFrom AND ValidTo", new { tuesday = week.Tuesday });
        }

        public async Task<int> GetMaximumCountryPingForWeek(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QuerySingleAsync<int>("SELECT MAX(AveragePingRtT) FROM CountryPingInfo WHERE @Tuesday BETWEEN ValidFrom AND ValidTo", new { tuesday = week.Tuesday });
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
