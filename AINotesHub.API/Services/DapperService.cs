using System.Data;
using AINotesHub.Shared.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AINotesHub.API.Services
{
    public class DapperService
    {
        private readonly IConfiguration _config;

        public DapperService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Note>> SearchNotes(string keyword)
        {
            using var connection = CreateConnection();

            var query = @"SELECT * FROM Notes WHERE Title LIKE @Keyword";

            return await connection.QueryAsync<Note>(
                query,
                new { Keyword = "%" + keyword + "%" }
            );
        }
        public async Task<int> GetNextUntitledNumber(Guid userId)
        {
            using var connection = CreateConnection();

            var query = @"
        SELECT ISNULL(MAX(CAST(SUBSTRING(Title, 10, LEN(Title)) AS INT)), 0)
        FROM Notes
        WHERE UserId = @UserId
        AND Title LIKE 'Untitled [0-9]%'";

            var maxNumber = await connection.ExecuteScalarAsync<int>(
                query,
                new { UserId = userId }
            );

            return maxNumber + 1;
        }
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
        //public async Task<IEnumerable<Note>> GetJobs()
        //{
        //    using var connection = CreateConnection();

        //    string query = "SELECT * FROM Notes";

        //    return await connection.QueryAsync<Note>(query);
        //}

    }
}
