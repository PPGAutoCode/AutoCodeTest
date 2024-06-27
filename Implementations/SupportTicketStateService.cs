
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
    public class SupportTicketStateService : ISupportTicketStateService
    {
        private readonly IDbConnection _dbConnection;

        public SupportTicketStateService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportTicketState(CreateSupportTicketStateDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newSupportTicketState = new SupportTicketState
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
                INSERT INTO SupportTicketState (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, newSupportTicketState);
                return newSupportTicketState.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<SupportTicketState> GetSupportTicketState(SupportTicketStateRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM SupportTicketState WHERE Id = @Id;
            ";

            try
            {
                var supportTicketState = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(sql, new { request.Id });
                if (supportTicketState == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return supportTicketState;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateSupportTicketState(UpdateSupportTicketStateDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM SupportTicketState WHERE Id = @Id;
            ";

            var existingSupportTicketState = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(selectSql, new { request.Id });
            if (existingSupportTicketState == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingSupportTicketState.Name = request.Name;
            existingSupportTicketState.Version += 1;
            existingSupportTicketState.Changed = DateTime.UtcNow;
            existingSupportTicketState.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE SupportTicketState
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingSupportTicketState);
                return existingSupportTicketState.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteSupportTicketState(DeleteSupportTicketStateDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM SupportTicketState WHERE Id = @Id;
            ";

            var existingSupportTicketState = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(selectSql, new { request.Id });
            if (existingSupportTicketState == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM SupportTicketState WHERE Id = @Id;
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

        public async Task<List<SupportTicketState>> GetListSupportTicketState(ListSupportTicketStateRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM SupportTicketState
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS
                FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var supportTicketStates = await _dbConnection.QueryAsync<SupportTicketState>(sql, new { request.PageLimit, request.PageOffset, request.SortField, request.SortOrder });
                return supportTicketStates.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
