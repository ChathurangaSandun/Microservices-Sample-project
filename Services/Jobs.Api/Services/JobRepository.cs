﻿

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Jobs.Api.Models;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Jobs.Api.Services
{
    public class JobRepository : IJobRepository
    {
        private readonly string _connectionString;
        //private readonly ILogger<JobRepository> _logger;

        public JobRepository(string connectionString/*, ILogger<JobRepository> logger*/ )
        {
            _connectionString = connectionString;
            //this._logger = logger;
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Job>> GetAll()
        {
            //this._logger.LogInformation("get jobs repo");
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<Job>("SELECT * FROM Jobs");
            }
        }

        public async Task<Job> Get(int jobId)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs where JobId=@JobId", new{JobId=jobId});
            }
        }

        public async Task<int> AddApplicant(JobApplicant jobApplicant)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.ExecuteAsync(
                    "insert JobApplicants values(@jobId,@applicantId,@name,@email,getutcdate(),1)",
                    new
                    {
                        jobId = jobApplicant.JobId,
                        applicantId = jobApplicant.ApplicantId,
                        name = jobApplicant.Name,
                        email = jobApplicant.Email
                    });
            }
        }
    }
}