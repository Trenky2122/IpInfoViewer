﻿using System.Globalization;
using System.Net;
using System.Net.WebSockets;
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

        private static async Task<NpgsqlTransaction> CreateTransaction(NpgsqlConnection conn)
        {
            await conn.OpenAsync();
            return await conn.BeginTransactionAsync();
        }

        #endregion

        #region Create Tables


        public async Task SeedTablesAsync()
        {
            await using var connection = CreateConnection();
            
            connection.Open();
            await CreateIpTableAsync(connection);
            await CreateMapIpRepresentationTableAsync(connection);
            await CreateCountryPingInfoTableAsync(connection);
        }

        private Task CreateIpTableAsync(NpgsqlConnection connection)
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

        private Task CreateMapIpRepresentationTableAsync(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS MapIpRepresentation (" +
                         "Id SERIAL PRIMARY KEY," +
                         "Latitude float," +
                         "Longitude float," +
                         "IpAddressesCount int, " +
                         "AveragePingRtT int, " +
                         "MaximumPingRtT int, " +
                         "MinimumPingRtT int, " +
                         "Week varchar(8));" +
                         "CREATE UNIQUE INDEX IF NOT EXISTS index2 ON MapIpRepresentation (Latitude, Longitude, Week); " +
                         "CREATE INDEX IF NOT EXISTS index4 ON MapIpRepresentation (Week);";
            return connection.ExecuteAsync(sql);
        }

        private Task CreateCountryPingInfoTableAsync(NpgsqlConnection connection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS CountryPingInfo (" +
                         "Id SERIAL PRIMARY KEY," +
                         "CountryCode varchar(2)," +
                         "IpAddressesCount int, " +
                         "AveragePingRtT int, " +
                         "MaximumPingRtT int, " +
                         "MinimumPingRtT int, " +
                         "Week varchar(8));" +
                         "CREATE UNIQUE INDEX IF NOT EXISTS index3 ON CountryPingInfo (CountryCode, Week); " +
                         "CREATE INDEX IF NOT EXISTS index5 ON CountryPingInfo (Week);";
            return connection.ExecuteAsync(sql);
        }

        #endregion

        public async Task SaveIpAddressInfoAsync(IpAddressInfo address)
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

        public async Task SaveMapIpAddressRepresentationsAsync(IEnumerable<MapPoint> representations)
        {
            await using var connection = CreateConnection();
            var transaction = await CreateTransaction(connection);
            foreach (var representation in representations)
            {
                string sql = "INSERT INTO MapIpRepresentation (Latitude, Longitude, IpAddressesCount, AveragePingRtT, MinimumPingRtT, MaximumPingRtT, Week) " +
                             "VALUES (@Latitude, @Longitude, @IpAddressesCount, @AveragePingRtT, @MinimumPingRtT, @MaximumPingRtT, @Week)";
                await connection.ExecuteAsync(sql, representation, transaction);
            }
            await transaction.CommitAsync();
        }

        public async Task SaveCountryPingInfosAsync(IEnumerable<CountryPingInfo> countryPingInfos)
        {
            await using var connection = CreateConnection();
            var transaction = await CreateTransaction(connection);
            foreach (var countryPingInfo in countryPingInfos)
            {
                string sql = "INSERT INTO CountryPingInfo (CountryCode, IpAddressesCount, AveragePingRtT, MinimumPingRtT, MaximumPingRtT, Week) " +
                             "VALUES (@CountryCode, @IpAddressesCount, @AveragePingRtT, @MinimumPingRtT, @MaximumPingRtT, @Week)";
                await connection.ExecuteAsync(sql, countryPingInfo, transaction);
            }
            await transaction.CommitAsync();
        }

        public async Task<IEnumerable<IpAddressInfo>> GetIpAddressesAsync(int offset = 0, int limit = int.MaxValue)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<IpAddressInfo>("SELECT * FROM IpAddresses LIMIT @limit OFFSET @offset", new { limit, offset });
        }
        
        public async Task<IEnumerable<MapPoint>> GetMapForWeekAsync(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<MapPoint>("SELECT * FROM MapIpRepresentation WHERE Week = @Week", new { week = week.ToString() });
        }

        public async Task<string?> GetLastDateWhenMapIsProcessedAsync()
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>("SELECT Week FROM MapIpRepresentation ORDER BY Week DESC LIMIT 1");
        }

        public async Task<string?> GetLastDateWhenCountriesAreProcessedAsync()
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>("SELECT Week FROM CountryPingInfo ORDER BY Week DESC LIMIT 1");
        }

        public async Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeekAsync(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<CountryPingInfo>("SELECT * FROM CountryPingInfo WHERE Week = @Week", new { week = week.ToString() });
        }

    }
}
