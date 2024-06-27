
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
    public class AppStatusService : IAppStatusService
    {
        private readonly IDbConnection _dbConnection;

        public AppStatusService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppStatus(CreateAppStatusDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appStatus = new AppStatus
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
                INSERT INTO AppStatus (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, appStatus);
                return appStatus.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<AppStatus> GetAppStatus(AppStatusRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM AppStatus WHERE Id = @Id;
            ";

            try
            {
                var appStatus = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(sql, new { request.Id });
                if (appStatus == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return appStatus;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateAppStatus(UpdateAppStatusDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM AppStatus WHERE Id = @Id;
            ";

            var existingAppStatus = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(selectSql, new { request.Id });
            if (existingAppStatus == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingAppStatus.Name = request.Name;
            existingAppStatus.Version += 1;
            existingAppStatus.Changed = DateTime.UtcNow;
            existingAppStatus.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE AppStatus
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingAppStatus);
                return existingAppStatus.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAppStatus(DeleteAppStatusDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM AppStatus WHERE Id = @Id;
            ";

            var existingAppStatus = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(selectSql, new { request.Id });
            if (existingAppStatus == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM AppStatus WHERE Id = @Id;
            ";

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

        public async Task<List<AppStatus>> GetListAppStatus(ListAppStatusRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM AppStatus
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var appStatuses = await _dbConnection.QueryAsync<AppStatus>(sql, new
                {
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });
                return appStatuses.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
