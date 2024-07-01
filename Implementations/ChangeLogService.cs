
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly IDbConnection _dbConnection;

        public ChangeLogService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateChangeLog(CreateChangeLogDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Notes) || request.ProductId == Guid.Empty || request.ReleaseDate == default || request.Version == null || string.IsNullOrEmpty(request.CreatorId))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new ChangeLog object
            var changeLog = new ChangeLog
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Notes = request.Notes,
                ProductId = request.ProductId,
                ReleaseDate = request.ReleaseDate,
                ChangeLogVersion = request.ChangeLogVersion,
                Version = request.Version,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.Parse(request.CreatorId),
                ChangedUser = Guid.Parse(request.CreatorId)
            };

            // Step 3: Save the new ChangeLog object to the database
            const string sql = @"
                INSERT INTO ChangeLogs (Id, Title, Notes, ProductId, ReleaseDate, ChangeLogVersion, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Title, @Notes, @ProductId, @ReleaseDate, @ChangeLogVersion, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, changeLog);
                return changeLog.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ChangeLog> GetChangeLog(ChangeLogRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty && string.IsNullOrEmpty(request.Title))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ChangeLog from the database
            ChangeLog changeLog;
            if (request.Id != Guid.Empty)
            {
                const string sql = "SELECT * FROM ChangeLogs WHERE Id = @Id";
                changeLog = await _dbConnection.QuerySingleOrDefaultAsync<ChangeLog>(sql, new { Id = request.Id });
            }
            else
            {
                const string sql = "SELECT * FROM ChangeLogs WHERE Title = @Title";
                changeLog = await _dbConnection.QuerySingleOrDefaultAsync<ChangeLog>(sql, new { Title = request.Title });
            }

            if (changeLog == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch the product and creator
            const string productSql = "SELECT * FROM Products WHERE Id = @ProductId";
            var product = await _dbConnection.QuerySingleOrDefaultAsync<Product>(productSql, new { ProductId = changeLog.ProductId });
            if (product == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string creatorSql = "SELECT * FROM Users WHERE Id = @CreatorId";
            var creator = await _dbConnection.QuerySingleOrDefaultAsync<User>(creatorSql, new { CreatorId = changeLog.CreatorId });
            if (creator == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return changeLog;
        }

        public async Task<string> UpdateChangeLog(UpdateChangeLogDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Notes) || request.ProductId == Guid.Empty || request.ReleaseDate == default || request.Version == null || request.ChangedUser == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing ChangeLog from the database
            const string fetchSql = "SELECT * FROM ChangeLogs WHERE Id = @Id";
            var changeLog = await _dbConnection.QuerySingleOrDefaultAsync<ChangeLog>(fetchSql, new { Id = request.Id });
            if (changeLog == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the ChangeLog object
            changeLog.Title = request.Title;
            changeLog.Notes = request.Notes;
            changeLog.ProductId = request.ProductId;
            changeLog.ReleaseDate = request.ReleaseDate;
            changeLog.Version = request.Version;
            changeLog.Changed = DateTime.UtcNow;
            changeLog.ChangedUser = request.ChangedUser;

            // Step 4: Save the updated ChangeLog object to the database
            const string updateSql = @"
                UPDATE ChangeLogs
                SET Title = @Title, Notes = @Notes, ProductId = @ProductId, ReleaseDate = @ReleaseDate, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, changeLog);
                return changeLog.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteChangeLog(DeleteChangeLogDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing ChangeLog from the database
            const string fetchSql = "SELECT * FROM ChangeLogs WHERE Id = @Id";
            var changeLog = await _dbConnection.QuerySingleOrDefaultAsync<ChangeLog>(fetchSql, new { Id = request.Id });
            if (changeLog == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the ChangeLog object from the database
            const string deleteSql = "DELETE FROM ChangeLogs WHERE Id = @Id";

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

        public async Task<List<ChangeLog>> GetListChangeLog(ListChangeLogRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Query the database to fetch the list of ChangeLogs
            string sql = "SELECT * FROM ChangeLogs";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            sql += " LIMIT @PageLimit OFFSET @PageOffset";

            var changeLogs = await _dbConnection.QueryAsync<ChangeLog>(sql, new { PageLimit = request.PageLimit, PageOffset = request.PageOffset });

            if (changeLogs == null || !changeLogs.Any())
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch the products and creators
            var productIds = changeLogs.Select(cl => cl.ProductId).Distinct().ToList();
            const string productsSql = "SELECT * FROM Products WHERE Id IN @ProductIds";
            var products = await _dbConnection.QueryAsync<Product>(productsSql, new { ProductIds = productIds });
            if (products == null || products.Count() != productIds.Count)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var creatorIds = changeLogs.Select(cl => cl.CreatorId).Distinct().ToList();
            const string creatorsSql = "SELECT * FROM Users WHERE Id IN @CreatorIds";
            var creators = await _dbConnection.QueryAsync<User>(creatorsSql, new { CreatorIds = creatorIds });
            if (creators == null || creators.Count() != creatorIds.Count)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return changeLogs.ToList();
        }
    }
}
