
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IDbConnection _dbConnection;

        public EnvironmentService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateEnvironment(CreateEnvironmentDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var environment = new Environment
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            const string sql = @"
                INSERT INTO Environments (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, environment);
                return environment.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<Environment> GetEnvironment(EnvironmentRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM Environments WHERE Id = @Id";

            try
            {
                var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>(sql, new { request.Id });
                if (environment == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return environment;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateEnvironment(UpdateEnvironmentDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Environments WHERE Id = @Id";
            var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>(selectSql, new { request.Id });

            if (environment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            environment.Name = request.Name;
            environment.Version += 1;
            environment.Changed = DateTime.UtcNow;
            environment.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE Environments
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, environment);
                return environment.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteEnvironment(DeleteEnvironmentDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Environments WHERE Id = @Id";
            var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>(selectSql, new { request.Id });

            if (environment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM Environments WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<Environment>> GetListEnvironment(ListEnvironmentRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM Environments
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var environments = await _dbConnection.QueryAsync<Environment>(sql, new
                {
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });
                return environments.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
