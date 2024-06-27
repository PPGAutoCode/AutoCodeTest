
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
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new AllowedGrantType object
            var allowedGrantType = new AllowedGrantType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            // Step 3: Insert the newly created AllowedGrantType object to the database
            const string sql = @"
                INSERT INTO AllowedGrantTypes (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, allowedGrantType);
                return allowedGrantType.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<AllowedGrantType> GetAllowedGrantType(AllowedGrantTypeRequestDto request)
        {
            // Step 1: Validate that request.payload.Id is not null
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the AllowedGrantType from the database based on the provided AllowedGrantType ID
            const string sql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(sql, new { Id = request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateAllowedGrantType(UpdateAllowedGrantTypeDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters ("Id" and "Name")
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the AllowedGrantType from the database by Id
            const string selectSql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var allowedGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { Id = request.Id });

            if (allowedGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the AllowedGrantType object with the provided changes
            allowedGrantType.Name = request.Name;
            allowedGrantType.Version += 1;
            allowedGrantType.Changed = DateTime.UtcNow;
            allowedGrantType.ChangedUser = request.ChangedUser;

            // Step 4: Save the updated AllowedGrantType object to the database
            const string updateSql = @"
                UPDATE AllowedGrantTypes
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, allowedGrantType);
                return allowedGrantType.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAllowedGrantType(DeleteAllowedGrantTypeDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter ("Id")
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the AllowedGrantType from the database by Id
            const string selectSql = "SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var allowedGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { Id = request.Id });

            if (allowedGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the AllowedGrantType object from the database
            const string deleteSql = "DELETE FROM AllowedGrantTypes WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<AllowedGrantType>> GetListAllowedGrantType(ListAllowedGrantTypeRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters ("PageNumber" and "PageSize")
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of AllowedGrantTypes from the database based on the provided pagination parameters
            const string sql = @"
                SELECT * FROM AllowedGrantTypes
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var results = await _dbConnection.QueryAsync<AllowedGrantType>(sql, new
                {
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });

                return results.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
