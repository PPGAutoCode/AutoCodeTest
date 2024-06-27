
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
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            const string sql = @"
                INSERT INTO ProductDomains (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, productDomain);
                return productDomain.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ProductDomain> GetProductDomain(ProductDomainRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM ProductDomains WHERE Id = @Id;
            ";

            try
            {
                var productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(sql, new { request.Id });
                if (productDomain == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return productDomain;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateProductDomain(UpdateProductDomainDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM ProductDomains WHERE Id = @Id;
            ";

            var existingProductDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { request.Id });
            if (existingProductDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingProductDomain.Name = request.Name;
            existingProductDomain.Version += 1;
            existingProductDomain.Changed = DateTime.UtcNow;
            existingProductDomain.ChangedUser = request.ChangedUser;

            const string updateSql = @"
                UPDATE ProductDomains
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingProductDomain);
                return existingProductDomain.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteProductDomain(DeleteProductDomainDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM ProductDomains WHERE Id = @Id;
            ";

            var existingProductDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { request.Id });
            if (existingProductDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM ProductDomains WHERE Id = @Id;
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

        public async Task<List<ProductDomain>> GetListProductDomain(ListProductDomainRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0 || string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM ProductDomains
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS
                FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var productDomains = await _dbConnection.QueryAsync<ProductDomain>(sql, new { request.PageLimit, request.PageOffset, request.SortField, request.SortOrder });
                return productDomains.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
