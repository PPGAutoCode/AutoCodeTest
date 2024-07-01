
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
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newSelectTicketPriority = new SelectTicketPriority
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO SelectTicketPriority (Id, Name) VALUES (@Id, @Name)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, newSelectTicketPriority);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return newSelectTicketPriority.Id.ToString();
        }

        public async Task<SelectTicketPriority> GetSelectTicketPriority(SelectTicketPriorityRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT Id, Name FROM SelectTicketPriority WHERE Id = @Id";
            var selectTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SelectTicketPriority>(sql, new { Id = request.Id });

            if (selectTicketPriority == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return selectTicketPriority;
        }

        public async Task<string> UpdateSelectTicketPriority(UpdateSelectTicketPriorityDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT Id, Name FROM SelectTicketPriority WHERE Id = @Id";
            var existingSelectTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SelectTicketPriority>(selectSql, new { Id = request.Id });

            if (existingSelectTicketPriority == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingSelectTicketPriority.Name = request.Name;

            const string updateSql = "UPDATE SelectTicketPriority SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { Id = request.Id, Name = request.Name });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingSelectTicketPriority.Id.ToString();
        }

        public async Task<bool> DeleteSelectTicketPriority(DeleteSelectTicketPriorityDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT Id, Name FROM SelectTicketPriority WHERE Id = @Id";
            var existingSelectTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SelectTicketPriority>(selectSql, new { Id = request.Id });

            if (existingSelectTicketPriority == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM SelectTicketPriority WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<SelectTicketPriority>> GetListSelectTicketPriority(ListSelectTicketPriorityRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT Id, Name FROM SelectTicketPriority ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var selectTicketPriorities = await _dbConnection.QueryAsync<SelectTicketPriority>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (selectTicketPriorities == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return selectTicketPriorities.AsList();
        }
    }
}
