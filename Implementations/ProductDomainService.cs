
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class ProductDomainService : IProductDomainService
    {
        private readonly IDbConnection _dbConnection;

        public ProductDomainService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductDomain(CreateProductDomainDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var productDomain = new ProductDomain
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO ProductDomains (Id, Name) VALUES (@Id, @Name)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, productDomain);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return productDomain.Id.ToString();
        }

        public async Task<ProductDomain> GetProductDomain(ProductDomainRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM ProductDomains WHERE Id = @Id";
            var productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(sql, new { Id = request.Id });

            if (productDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return productDomain;
        }

        public async Task<string> UpdateProductDomain(UpdateProductDomainDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductDomains WHERE Id = @Id";
            var existingProductDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { Id = request.Id });

            if (existingProductDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingProductDomain.Name = request.Name;

            const string updateSql = "UPDATE ProductDomains SET Name = @Name WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, new { Id = existingProductDomain.Id, Name = existingProductDomain.Name });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingProductDomain.Id.ToString();
        }

        public async Task<bool> DeleteProductDomain(DeleteProductDomainDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductDomains WHERE Id = @Id";
            var existingProductDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { Id = request.Id });

            if (existingProductDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM ProductDomains WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<ProductDomain>> GetListProductDomain(ListProductDomainRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM ProductDomains ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var productDomains = await _dbConnection.QueryAsync<ProductDomain>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit, SortField = request.SortField, SortOrder = request.SortOrder });

            if (productDomains == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return productDomains.AsList();
        }
    }
}
