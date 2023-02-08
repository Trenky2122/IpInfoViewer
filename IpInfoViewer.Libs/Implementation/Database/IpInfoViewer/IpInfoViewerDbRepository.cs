﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        #region Seed Tables


        public async Task SeedTables()
        {

            await using var connection = CreateConnection();
            connection.Open();
            await CreateIpTable(connection);
            await CreateMapIpRepresentationTable(connection);
            await CreateChartsTable(connection);
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

        #endregion

        public async Task SaveIpAddressInfo(IpAddressInfo address)
        {
            await using var connection = CreateConnection();
            string sql = "INSERT INTO IpAddresses (IpValue, CountryCode, City, Latitude, Longitude) " +
                         "VALUES (@IpValue, @CountryCode, @City, @Latitude, @Longitude)";
            await connection.ExecuteAsync(sql, address);
        }

        public async Task SaveMapIpAddressRepresentation(MapIpAddressesRepresentation representation)
        {
            await using var connection = CreateConnection();
            string sql = "INSERT INTO MapIpRepresentation (Latitude, Longitude, IpAddressesCount, AveragePingRtT, ValidFrom, ValidTo) " +
                         "VALUES (@Latitude, @Longitude, @IpAddressesCount, @AveragePingRtT, @ValidFrom, @ValidTo)";
            await connection.ExecuteAsync(sql, representation);
        }

        public async Task<IEnumerable<IpAddressInfo>> GetIpAddresses(int offset = 0, int limit = int.MaxValue)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<IpAddressInfo>("SELECT * FROM IpAddresses LIMIT @limit OFFSET @offset", new { limit, offset });
        }

        public async Task<IEnumerable<MapIpAddressesRepresentation>> GetMapForWeek(Week week)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<MapIpAddressesRepresentation>("SELECT * FROM MapIpRepresentation WHERE @Tuesday BETWEEN ValidFrom AND ValidTo", new { tuesday = week.Tuesday });
        }

        public async Task<DateTime?> GetLastDateWhenMapIsProcessed()
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DateTime?>("SELECT ValidTo FROM MapIpRepresentation ORDER BY ValidTo DESC LIMIT 1");
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
