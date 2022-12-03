using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using IpInfoViewer.Libs.Abstractions;
using IpInfoViewer.Libs.Models;
using Npgsql;
using NpgsqlTypes;

namespace IpInfoViewer.Libs.Implementation
{
    public class IpInfoViewerDbRepository : IIpInfoViewerDbRepository
    {
        private readonly string _connectionString;

        public IpInfoViewerDbRepository(string connectionString)
        {
            _connectionString = connectionString;
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
        }

        private Task CreateIpTable(NpgsqlConnection connection)
        {
            var command = connection.CreateCommand();
            string sql = "CREATE TABLE IF NOT EXISTS IpAddresses (" +
                         "Id SERIAL PRIMARY KEY," +
                         "IpValue cidr," +
                         "CountryCode varchar(2)," +
                         "City varchar(80)," +
                         "Latitude float," +
                         "Longitude float)";
            command.CommandText = sql;
            return command.ExecuteNonQueryAsync();
        }

        #endregion

        public async Task SaveIpAddressInfo(string csvLine)
        {
            var fields = csvLine.Split(',');
            await using var connection = CreateConnection();
            connection.Open();
            string sql = "INSERT INTO IpAddresses (IpValue, CountryCode, City, Latitude, Longitude) " +
                         "VALUES (@IpValue, @CountryCode, @City, @Latitude, @Longitude)";
            var command = connection.CreateCommand();
            command.Parameters.Add("@IpValue", NpgsqlDbType.Cidr);
            command.Parameters["@IpValue"].Value = (IPAddress.Parse(fields[0]), 32);
            command.Parameters.Add("@CountryCode", NpgsqlDbType.Varchar);
            command.Parameters["@CountryCode"].Value = fields[3];
            command.Parameters.Add("@City", NpgsqlDbType.Varchar);
            command.Parameters["@City"].Value = RemoveDiacritics(fields[5]);
            command.Parameters.Add("@Latitude", NpgsqlDbType.Numeric);
            command.Parameters["@Latitude"].Value = Convert.ToDouble(fields[6], CultureInfo.InvariantCulture);
            command.Parameters.Add("@Longitude", NpgsqlDbType.Numeric);
            command.Parameters["@Longitude"].Value = Convert.ToDouble(fields[7], CultureInfo.InvariantCulture);
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<IpAddressInfo>> GetIpAddresses(int offset = 0, int limit = Int32.MaxValue)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<IpAddressInfo>("SELECT * FROM IpAddresses LIMIT @limit OFFSET @offset", new {limit, offset});
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
