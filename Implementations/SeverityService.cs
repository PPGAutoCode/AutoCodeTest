
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
    public class SeverityService : ISeverityService
    {
        private readonly IDbConnection _dbConnection;

        public SeverityService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSeverity(CreateSeverityDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var severity = new Severity
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
                INSERT INTO Severities (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, severity);
                return severity.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<Severity> GetSeverity(SeverityRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM Severities WHERE Id = @Id;
            ";

            try
            {
                var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(sql, new { request.Id });
                if (severity == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return severity;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateSeverity(UpdateSeverityDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM Severities WHERE Id = @Id;
            ";

            var existingSeverity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(selectSql, new { request.Id });
            if (existingSeverity == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingSeverity.Name = request.Name;
            existingSeverity.Version += 1;
            existingSeverity.Changed = DateTime.UtcNow;
            existingSeverity.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE Severities
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingSeverity);
                return existingSeverity.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteSeverity(DeleteSeverityDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM Severities WHERE Id = @Id;
            ";

            var existingSeverity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(selectSql, new { request.Id });
            if (existingSeverity == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM Severities WHERE Id = @Id;
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

        public async Task<List<Severity>> GetListSeverity(ListSeverityRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM Severities
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var severities = await _dbConnection.QueryAsync<Severity>(sql, new
                {
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });
                return severities.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
