
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
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            const string sql = @"
                INSERT INTO GettingStartedCompletedCondition (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, newCondition);
                return newCondition.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<GettingStartedCompletedCondition> GetGettingStartedCompletedCondition(GettingStartedCompletedConditionRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id;
            ";

            try
            {
                var condition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(sql, new { request.Id });
                if (condition == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return condition;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateGettingStartedCompletedCondition(UpdateGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id;
            ";

            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { request.Id });
            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCondition.Name = request.Name;
            existingCondition.Version += 1;
            existingCondition.Changed = DateTime.UtcNow;
            existingCondition.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE GettingStartedCompletedCondition
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingCondition);
                return existingCondition.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteGettingStartedCompletedCondition(DeleteGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM GettingStartedCompletedCondition WHERE Id = @Id;
            ";

            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { request.Id });
            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM GettingStartedCompletedCondition WHERE Id = @Id;
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

        public async Task<List<GettingStartedCompletedCondition>> GetListGettingStartedCompletedCondition(ListGettingStartedCompletedConditionRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM GettingStartedCompletedCondition
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS
                FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var conditions = await _dbConnection.QueryAsync<GettingStartedCompletedCondition>(sql, new { request.PageLimit, request.PageOffset, request.SortField, request.SortOrder });
                return conditions.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
