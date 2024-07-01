
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
                Name = request.Name
            };

            const string sql = "INSERT INTO SupportCategories (Id, Name) VALUES (@Id, @Name)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, supportCategory);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return supportCategory.Id.ToString();
        }

        public async Task<SupportCategory> GetSupportCategory(SupportCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var supportCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(sql, new { Id = request.Id });

            if (supportCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return supportCategory;
        }

        public async Task<string> UpdateSupportCategory(UpdateSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(selectSql, new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCategory.Name = request.Name;

            const string updateSql = "UPDATE SupportCategories SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { Id = existingCategory.Id, Name = existingCategory.Name });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingCategory.Id.ToString();
        }

        public async Task<bool> DeleteSupportCategory(DeleteSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(selectSql, new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM SupportCategories WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<SupportCategory>> GetListSupportCategory(ListSupportCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM SupportCategories ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (supportCategories == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return supportCategories.AsList();
        }
    }
}
