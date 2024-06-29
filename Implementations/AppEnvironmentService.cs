
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class AppEnvironmentService : IAppEnvironmentService
    {
        private readonly IDbConnection _dbConnection;

        public AppEnvironmentService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppEnvironment(CreateAppEnvironmentDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appEnvironment = new AppEnvironment
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
                INSERT INTO AppEnvironments (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, appEnvironment);
                return appEnvironment.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<AppEnvironment> GetAppEnvironment(AppEnvironmentRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM AppEnvironments WHERE Id = @Id";

            try
            {
                var result = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(sql, new { request.Id });
                if (result == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return result;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateAppEnvironment(UpdateAppEnvironmentDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppEnvironments WHERE Id = @Id";
            var existingEnvironment = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(selectSql, new { request.Id });

            if (existingEnvironment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingEnvironment.Name = request.Name;
            existingEnvironment.Version += 1;
            existingEnvironment.Changed = DateTime.UtcNow;
            existingEnvironment.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE AppEnvironments 
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser 
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingEnvironment);
                return existingEnvironment.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAppEnvironment(DeleteAppEnvironmentDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppEnvironments WHERE Id = @Id";
            var existingEnvironment = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(selectSql, new { request.Id });

            if (existingEnvironment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM AppEnvironments WHERE Id = @Id";

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

        public async Task<List<AppEnvironment>> GetListAppEnvironment(ListAppEnvironmentRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM AppEnvironments 
                ORDER BY @SortField @SortOrder 
                OFFSET @PageOffset ROWS 
                FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var result = await _dbConnection.QueryAsync<AppEnvironment>(sql, new { request.PageLimit, request.PageOffset, request.SortField, request.SortOrder });
                return result.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
