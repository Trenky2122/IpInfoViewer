using System.Net;
using Dapper;
using IpInfoViewer.Libs.Models.MFile;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IpInfoViewer.Libs.Implementation.Database.MFile
{
    public class MFileDbRepository : IMFileDbRepository

    {
        public readonly string _connectionString;

        public MFileDbRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MFileDbRepository(IConfiguration configuration)
        {
            _connectionString = configuration["MFileConnectionString"];
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<IEnumerable<Ping>> GetAllNotNullPings()
        {
            string sql = "SELECT ip_addr as IpAddr," +
                         " ping_rttmin as PingRTTMin," +
                         " ping_rttmax as PingRTTMax," +
                         " ping_rttavg as PingRTTAvg," +
                         " ping_rttmdev as PingRTTMDev," +
                         " ping_ploss as PingPLoss," +
                         " ping_date as PingDate" +
                         " FROM ping p" +
                         " WHERE ping_rttavg is not null";
            await using var connection = CreateConnection();
            return await connection.QueryAsync<Ping>(sql);
        }

        public async Task<IEnumerable<Ping>> GetPingsInRange(IPAddress start, IPAddress end)
        {
            string sql = "SELECT ip_addr as IpAddr," +
                         " ping_rttmin as PingRTTMin," +
                         " ping_rttmax as PingRTTMax," +
                         " ping_rttavg as PingRTTAvg," +
                         " ping_rttmdev as PingRTTMDev," +
                         " ping_ploss as PingPLoss," +
                         " ping_date as PingDate" +
                         " FROM ping p" +
                         $" WHERE ip_addr BETWEEN '{start}' AND '{end}'";
            await using var connection = CreateConnection();
            return await connection.QueryAsync<Ping>(sql);
        }

        public async Task<IEnumerable<Host>> GetHostsInRange(IPAddress start, IPAddress end)
        {
            string sql = "SELECT ip_addr as IpAddr," +
                         " rank_code as RankCode," +
                         " enter_date as EnterDate," +
                         " source as Source," +
                         " comment as Comment," +
                         " exclude as Exclude" +
                         " FROM hosts h" +
                         $" WHERE ip_addr BETWEEN '{start}' AND '{end}' AND exclude = 0" +
                         " and ip_addr NOT BETWEEN '10.0.0.0' AND '10.255.255.255'" +
                         " AND ip_addr NOT BETWEEN '172.16.0.0' AND '172.31.255.255'" +
                         " AND ip_addr NOT BETWEEN '192.168.0.0' AND '192.168.255.255'" +
                         " AND (EXISTS (SELECT * FROM topology t WHERE t.ip_addr=h.ip_addr AND t_status<>'E' LIMIT 1)" +
                         " OR EXISTS (SELECT * FROM ping p WHERE p.ip_addr = h.ip_addr AND ping_ploss BETWEEN 0 AND 100))";
            ;
            await using var connection = CreateConnection();
            return await connection.QueryAsync<Host>(sql);
        }

        public async Task<IEnumerable<Host>> GetAllValidHosts()
        {
            string sql = "SELECT ip_addr as IpAddr," +
                         " rank_code as RankCode," +
                         " enter_date as EnterDate," +
                         " source as Source," +
                         " comment as Comment," +
                         " exclude as Exclude" +
                         " FROM hosts h" +
                         " WHERE ip_addr NOT BETWEEN '10.0.0.0' AND '10.255.255.255'" +
                         " AND ip_addr NOT BETWEEN '172.16.0.0' AND '172.31.255.255'" +
                         " AND ip_addr NOT BETWEEN '192.168.0.0' AND '192.168.255.255'" +
                         " AND exclude = 0" +
                         " AND (EXISTS (SELECT * FROM topology t WHERE t.ip_addr=h.ip_addr AND t_status<>'E' LIMIT 1)" +
                         " OR EXISTS (SELECT * FROM ping p WHERE p.ip_addr = h.ip_addr AND ping_ploss BETWEEN 0 AND 100))";
            await using var connection = CreateConnection();
            return await connection.QueryAsync<Host>(sql, commandTimeout: 1800);
        }

        public async Task<IEnumerable<Tuple<(IPAddress, int), double>>> GetAverageRtTForIpForWeek(Week week)
        {
            string sql =
                $"SELECT ip_addr as Item1, AVG(ping_rttavg) as Item2 FROM ping WHERE ping_ploss BETWEEN 0 AND 100 AND ping_rttavg IS NOT NULL AND ping_date BETWEEN @from AND @to GROUP BY ip_addr";
            var parameters = new { from = week.Monday, to = week.Next().Monday.AddTicks(-1) };
            await using var connection = CreateConnection();
            return await connection.QueryAsync<Tuple<(IPAddress, int), double>>(sql, parameters, commandTimeout: 1800);
        }
    }
}
