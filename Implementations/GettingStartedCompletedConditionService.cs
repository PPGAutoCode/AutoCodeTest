
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
    public class GettingStartedCompletedConditionService : IGettingStartedCompletedConditionService
    {
        private readonly IDbConnection _dbConnection;

        public GettingStartedCompletedConditionService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateGettingStartedCompletedCondition(CreateGettingStartedCompletedConditionDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newCondition = new GettingStartedCompletedCondition
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO GettingStartedCompletedCondition (Id, Name) VALUES (@Id, @Name)";
            int rowsAffected = await _dbConnection.ExecuteAsync(sql, newCondition);

            if (rowsAffected == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return newCondition.Id.ToString();
        }

        public async Task<GettingStartedCompletedCondition> GetGettingStartedCompletedCondition(GettingStartedCompletedConditionRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id";
            var condition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(sql, new { Id = request.Id });

            if (condition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return condition;
        }

        public async Task<string> UpdateGettingStartedCompletedCondition(UpdateGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id";
            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { Id = request.Id });

            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCondition.Name = request.Name;

            const string updateSql = "UPDATE GettingStartedCompletedCondition SET Name = @Name WHERE Id = @Id";
            int rowsAffected = await _dbConnection.ExecuteAsync(updateSql, new { Id = existingCondition.Id, Name = existingCondition.Name });

            if (rowsAffected == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingCondition.Id.ToString();
        }

        public async Task<bool> DeleteGettingStartedCompletedCondition(DeleteGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id";
            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { Id = request.Id });

            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM GettingStartedCompletedCondition WHERE Id = @Id";
            int rowsAffected = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (rowsAffected == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<GettingStartedCompletedCondition>> GetListGettingStartedCompletedCondition(ListGettingStartedCompletedConditionRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM GettingStartedCompletedCondition ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var conditions = await _dbConnection.QueryAsync<GettingStartedCompletedCondition>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (conditions == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return conditions.AsList();
        }
    }
}
