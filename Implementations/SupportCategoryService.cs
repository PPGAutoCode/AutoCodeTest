
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
    public class SupportCategoryService : ISupportCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public SupportCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportCategory(CreateSupportCategoryDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var supportCategory = new SupportCategory
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
                INSERT INTO SupportCategories (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, supportCategory);
                return supportCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<SupportCategory> GetSupportCategory(SupportCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM SupportCategories WHERE Id = @Id;
            ";

            try
            {
                var supportCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(sql, new { request.Id });
                if (supportCategory == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return supportCategory;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateSupportCategory(UpdateSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM SupportCategories WHERE Id = @Id;
            ";

            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(selectSql, new { request.Id });
            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCategory.Name = request.Name;
            existingCategory.Version += 1;
            existingCategory.Changed = DateTime.UtcNow;
            existingCategory.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE SupportCategories
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingCategory);
                return existingCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteSupportCategory(DeleteSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM SupportCategories WHERE Id = @Id;
            ";

            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(selectSql, new { request.Id });
            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM SupportCategories WHERE Id = @Id;
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

        public async Task<List<SupportCategory>> GetListSupportCategory(ListSupportCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM SupportCategories
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var supportCategories = await _dbConnection.QueryAsync<SupportCategory>(sql, new
                {
                    request.PageOffset,
                    request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });
                return supportCategories.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
