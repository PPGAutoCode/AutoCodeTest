
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
    public class SelectTicketPriorityService : ISelectTicketPriorityService
    {
        private readonly IDbConnection _dbConnection;

        public SelectTicketPriorityService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSelectTicketPriority(CreateSelectTicketPriorityDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new SelectTicketPriority object
            var selectTicketPriority = new SelectTicketPriority
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            // Step 3: Insert the newly created SelectTicketPriority object to the database
            const string sql = @"
                INSERT INTO SelectTicketPriority (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, selectTicketPriority);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 4: Return the ID of the newly created SelectTicketPriority
            return selectTicketPriority.Id.ToString();
        }

        public async Task<SelectTicketPriority> GetSelectTicketPriority(SelectTicketPriorityRequestDto request)
        {
            const string sql = @"
                SELECT Id, Name, Version, Created, Changed, CreatorId, ChangedUser
                FROM SelectTicketPriority
                WHERE Id = @Id;
            ";

            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<SelectTicketPriority>(sql, new { request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateSelectTicketPriority(UpdateSelectTicketPriorityDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Update the SelectTicketPriority object in the database
            const string sql = @"
                UPDATE SelectTicketPriority
                SET Name = @Name, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new
                {
                    request.Id,
                    request.Name,
                    Changed = DateTime.UtcNow,
                    ChangedUser = request.ChangedUser
                });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 3: Return the ID of the updated SelectTicketPriority
            return request.Id.ToString();
        }

        public async Task<bool> DeleteSelectTicketPriority(DeleteSelectTicketPriorityDto request)
        {
            const string sql = @"
                DELETE FROM SelectTicketPriority
                WHERE Id = @Id;
            ";

            try
            {
                var result = await _dbConnection.ExecuteAsync(sql, new { request.Id });
                return result > 0;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<SelectTicketPriority>> GetListSelectTicketPriority(ListSelectTicketPriorityRequestDto request)
        {
            const string sql = @"
                SELECT Id, Name, Version, Created, Changed, CreatorId, ChangedUser
                FROM SelectTicketPriority
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var result = await _dbConnection.QueryAsync<SelectTicketPriority>(sql, new
                {
                    request.PageLimit,
                    request.PageOffset,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });
                return result.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
