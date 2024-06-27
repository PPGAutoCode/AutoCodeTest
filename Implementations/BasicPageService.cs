
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
    public class BasicPageService : IBasicPageService
    {
        private readonly IDbConnection _dbConnection;

        public BasicPageService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateBasicPage(CreateBasicPageDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var basicPage = new BasicPage
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
                INSERT INTO BasicPages (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, basicPage);
                return basicPage.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<BasicPage> GetBasicPage(BasicPageRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM BasicPages WHERE Id = @Id";

            try
            {
                var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(sql, new { request.Id });
                if (basicPage == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return basicPage;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateBasicPage(UpdateBasicPageDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BasicPages WHERE Id = @Id";
            var existingBasicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(selectSql, new { request.Id });

            if (existingBasicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingBasicPage.Name = request.Name;
            existingBasicPage.Version += 1;
            existingBasicPage.Changed = DateTime.UtcNow;
            existingBasicPage.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE BasicPages
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingBasicPage);
                return existingBasicPage.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> DeleteBasicPage(DeleteBasicPageDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BasicPages WHERE Id = @Id";
            var existingBasicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(selectSql, new { request.Id });

            if (existingBasicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM BasicPages WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { request.Id });
                return request.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<BasicPage>> GetListBasicPage(ListBasicPageRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM BasicPages
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS
                FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var basicPages = await _dbConnection.QueryAsync<BasicPage>(sql, new { request.SortField, request.SortOrder, request.PageOffset, request.PageLimit });
                return basicPages.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
