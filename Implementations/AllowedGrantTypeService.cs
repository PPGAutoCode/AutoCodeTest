
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
    public class AllowedGrantTypeService : IAllowedGrantTypeService
    {
        private readonly IDbConnection _dbConnection;

        public AllowedGrantTypeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAllowedGrantType(CreateAllowedGrantTypeDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newAllowedGrantType = new AllowedGrantType
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO AllowedGrantTypes (Id, Name) VALUES (@Id, @Name)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, newAllowedGrantType);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return newAllowedGrantType.Id.ToString();
        }

        public async Task<AllowedGrantType> GetAllowedGrantType(AllowedGrantTypeRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var allowedGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(sql, new { Id = request.Id });

            if (allowedGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return allowedGrantType;
        }

        public async Task<string> UpdateAllowedGrantType(UpdateAllowedGrantTypeDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var allowedGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { Id = request.Id });

            if (allowedGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            allowedGrantType.Name = request.Name;

            const string updateSql = "UPDATE AllowedGrantTypes SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { Id = allowedGrantType.Id, Name = allowedGrantType.Name });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return allowedGrantType.Id.ToString();
        }

        public async Task<bool> DeleteAllowedGrantType(DeleteAllowedGrantTypeDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var allowedGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { Id = request.Id });

            if (allowedGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM AllowedGrantTypes WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<AllowedGrantType>> GetListAllowedGrantType(ListAllowedGrantTypeRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM AllowedGrantTypes ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var allowedGrantTypes = await _dbConnection.QueryAsync<AllowedGrantType>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (allowedGrantTypes == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return allowedGrantTypes.AsList();
        }
    }
}
