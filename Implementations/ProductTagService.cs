
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
    public class ProductTagService : IProductTagService
    {
        private readonly IDbConnection _dbConnection;

        public ProductTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductTag(CreateProductTagDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var productTag = new ProductTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            const string sql = "INSERT INTO ProductTags (Id, Name) VALUES (@Id, @Name)";
            try
            {
                await _dbConnection.ExecuteAsync(sql, productTag);
                return productTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ProductTag> GetProductTag(ProductTagRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM ProductTags WHERE Id = @Id";
            var productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sql, new { Id = request.Id });

            if (productTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return productTag;
        }

        public async Task<string> UpdateProductTag(UpdateProductTagDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductTags WHERE Id = @Id";
            var existingProductTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(selectSql, new { Id = request.Id });

            if (existingProductTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingProductTag.Name = request.Name;

            const string updateSql = "UPDATE ProductTags SET Name = @Name WHERE Id = @Id";
            try
            {
                await _dbConnection.ExecuteAsync(updateSql, new { Id = existingProductTag.Id, Name = existingProductTag.Name });
                return existingProductTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteProductTag(DeleteProductTagDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductTags WHERE Id = @Id";
            var existingProductTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(selectSql, new { Id = request.Id });

            if (existingProductTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM ProductTags WHERE Id = @Id";
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

        public async Task<List<ProductTag>> GetListProductTag(ListProductTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM ProductTags";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var productTags = await _dbConnection.QueryAsync<ProductTag>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });
                return productTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
