
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
    public class AppTagService : IAppTagService
    {
        private readonly IDbConnection _dbConnection;

        public AppTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppTag(CreateAppTagDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appTag = new AppTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a default creator ID generation
                ChangedUser = Guid.NewGuid() // Assuming a default changed user ID generation
            };

            const string sql = @"
                INSERT INTO AppTags (Id, Name, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, appTag);
                return appTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<AppTag> GetAppTag(AppTagRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM AppTags WHERE Id = @Id";

            try
            {
                var appTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(sql, new { request.Id });
                if (appTag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return appTag;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateAppTag(UpdateAppTagDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM AppTags WHERE Id = @Id";

            var existingAppTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(selectSql, new { request.Id });
            if (existingAppTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingAppTag.Name = request.Name;
            existingAppTag.Changed = DateTime.UtcNow;

            const string updateSql = @"
                UPDATE AppTags SET Name = @Name, Changed = @Changed WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingAppTag);
                return existingAppTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAppTag(DeleteAppTagDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM AppTags WHERE Id = @Id";

            var existingAppTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(selectSql, new { request.Id });
            if (existingAppTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM AppTags WHERE Id = @Id";

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

        public async Task<List<AppTag>> GetListAppTag(ListAppTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM AppTags";

            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }

            sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var appTags = await _dbConnection.QueryAsync<AppTag>(sql, new { request.PageOffset, request.PageLimit });
                return appTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
