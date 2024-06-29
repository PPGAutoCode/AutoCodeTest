
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
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            const string sql = @"
                INSERT INTO GoLiveDeveloperType (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, newGoLiveDeveloperType);
                return newGoLiveDeveloperType.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<GoLiveDeveloperType> GetGoLiveDeveloperType(GoLiveDeveloperTypeRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM GoLiveDeveloperType WHERE Id = @Id;
            ";

            try
            {
                var result = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(sql, new { request.Id });
                if (result == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return result;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateGoLiveDeveloperType(UpdateGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM GoLiveDeveloperType WHERE Id = @Id;
            ";

            var existingType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(selectSql, new { request.Id });
            if (existingType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingType.Name = request.Name;
            existingType.Version += 1;
            existingType.Changed = DateTime.UtcNow;
            existingType.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE GoLiveDeveloperType
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingType);
                return existingType.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteGoLiveDeveloperType(DeleteGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM GoLiveDeveloperType WHERE Id = @Id;
            ";

            var existingType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(selectSql, new { request.Id });
            if (existingType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM GoLiveDeveloperType WHERE Id = @Id;
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

        public async Task<List<GoLiveDeveloperType>> GetListGoLiveDeveloperType(ListGoLiveDeveloperTypeRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM GoLiveDeveloperType
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var results = await _dbConnection.QueryAsync<GoLiveDeveloperType>(sql, new { request.PageLimit, request.PageOffset, request.SortField, request.SortOrder });
                return results.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
