
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
                Body = string.Empty // Assuming Body is required but not provided in the request
            };

            const string sql = "INSERT INTO BasicPages (Id, Name, Body) VALUES (@Id, @Name, @Body)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, basicPage);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return basicPage.Id.ToString();
        }

        public async Task<BasicPage> GetBasicPage(BasicPageRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM BasicPages WHERE Id = @Id";
            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(sql, new { request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return basicPage;
        }

        public async Task<string> UpdateBasicPage(UpdateBasicPageDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BasicPages WHERE Id = @Id";
            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(selectSql, new { request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            basicPage.Name = request.Name;

            const string updateSql = "UPDATE BasicPages SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { basicPage.Name, basicPage.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return basicPage.Id.ToString();
        }

        public async Task<string> DeleteBasicPage(DeleteBasicPageDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BasicPages WHERE Id = @Id";
            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(selectSql, new { request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM BasicPages WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return basicPage.Id.ToString();
        }

        public async Task<List<BasicPage>> GetListBasicPage(ListBasicPageRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM BasicPages ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var basicPages = await _dbConnection.QueryAsync<BasicPage>(sql, new { request.SortField, request.SortOrder, request.PageOffset, request.PageLimit });

            if (basicPages == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return basicPages.AsList();
        }
    }
}
