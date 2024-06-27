
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
    public class ApiTagService : IApiTagService
    {
        private readonly IDbConnection _dbConnection;

        public ApiTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateApiTag(CreateApiTagDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var apiTag = new ApiTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a default creator ID or fetch from context
                ChangedUser = Guid.NewGuid() // Assuming a default changed user ID or fetch from context
            };

            const string sql = @"
                INSERT INTO ApiTags (Id, Name, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, apiTag);
                return apiTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ApiTag> GetApiTag(ApiTagRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM ApiTags WHERE Id = @Id";

            try
            {
                var apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sql, new { request.Id });
                if (apiTag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return apiTag;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateApiTag(UpdateApiTagDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM ApiTags WHERE Id = @Id";

            var existingApiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(selectSql, new { request.Id });
            if (existingApiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingApiTag.Name = request.Name;
            existingApiTag.Changed = DateTime.UtcNow;

            const string updateSql = @"
                UPDATE ApiTags SET Name = @Name, Changed = @Changed WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingApiTag);
                return existingApiTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteApiTag(DeleteApiTagDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM ApiTags WHERE Id = @Id";

            var existingApiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(selectSql, new { request.Id });
            if (existingApiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM ApiTags WHERE Id = @Id";

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

        public async Task<List<ApiTag>> GetListApiTag(ListApiTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM ApiTags 
                ORDER BY @SortField @SortOrder 
                OFFSET @PageOffset ROWS 
                FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var apiTags = await _dbConnection.QueryAsync<ApiTag>(sql, new
                {
                    request.PageLimit,
                    request.PageOffset,
                    SortField = request.SortField ?? "Created",
                    SortOrder = request.SortOrder ?? "ASC"
                });
                return apiTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
