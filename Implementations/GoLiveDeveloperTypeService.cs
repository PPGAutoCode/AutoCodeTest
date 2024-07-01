
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
    public class GoLiveDeveloperTypeService : IGoLiveDeveloperTypeService
    {
        private readonly IDbConnection _dbConnection;

        public GoLiveDeveloperTypeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateGoLiveDeveloperType(CreateGoLiveDeveloperTypeDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newGoLiveDeveloperType = new GoLiveDeveloperType
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO GoLiveDeveloperType (Id, Name) VALUES (@Id, @Name)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, newGoLiveDeveloperType);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return newGoLiveDeveloperType.Id.ToString();
        }

        public async Task<GoLiveDeveloperType> GetGoLiveDeveloperType(GoLiveDeveloperTypeRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT Id, Name FROM GoLiveDeveloperType WHERE Id = @Id";
            var goLiveDeveloperType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(sql, new { Id = request.Id });

            if (goLiveDeveloperType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return goLiveDeveloperType;
        }

        public async Task<string> UpdateGoLiveDeveloperType(UpdateGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT Id, Name FROM GoLiveDeveloperType WHERE Id = @Id";
            var existingGoLiveDeveloperType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(selectSql, new { Id = request.Id });

            if (existingGoLiveDeveloperType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingGoLiveDeveloperType.Name = request.Name;

            const string updateSql = "UPDATE GoLiveDeveloperType SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { Id = existingGoLiveDeveloperType.Id, Name = existingGoLiveDeveloperType.Name });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingGoLiveDeveloperType.Id.ToString();
        }

        public async Task<bool> DeleteGoLiveDeveloperType(DeleteGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT Id, Name FROM GoLiveDeveloperType WHERE Id = @Id";
            var existingGoLiveDeveloperType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(selectSql, new { Id = request.Id });

            if (existingGoLiveDeveloperType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM GoLiveDeveloperType WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<GoLiveDeveloperType>> GetListGoLiveDeveloperType(ListGoLiveDeveloperTypeRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT Id, Name FROM GoLiveDeveloperType ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var goLiveDeveloperTypes = await _dbConnection.QueryAsync<GoLiveDeveloperType>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (goLiveDeveloperTypes == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return goLiveDeveloperTypes.AsList();
        }
    }
}
