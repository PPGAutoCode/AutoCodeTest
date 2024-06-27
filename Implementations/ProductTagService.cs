
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
                Name = request.Name,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a default creator ID or fetch from context
                ChangedUser = Guid.NewGuid() // Assuming a default changed user ID or fetch from context
            };

            const string sql = @"
                INSERT INTO ProductTags (Id, Name, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Created, @Changed, @CreatorId, @ChangedUser)";

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

            const string sql = @"
                SELECT * FROM ProductTags WHERE Id = @Id";

            try
            {
                var productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sql, new { request.Id });
                if (productTag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return productTag;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateProductTag(UpdateProductTagDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM ProductTags WHERE Id = @Id";

            var existingProductTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(selectSql, new { request.Id });
            if (existingProductTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingProductTag.Name = request.Name;
            existingProductTag.Changed = DateTime.UtcNow;

            const string updateSql = @"
                UPDATE ProductTags SET Name = @Name, Changed = @Changed WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingProductTag);
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

            const string selectSql = @"
                SELECT * FROM ProductTags WHERE Id = @Id";

            var existingProductTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(selectSql, new { request.Id });
            if (existingProductTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM ProductTags WHERE Id = @Id";

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

        public async Task<List<ProductTag>> GetListProductTag(ListProductTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM ProductTags 
                ORDER BY @SortField @SortOrder 
                OFFSET @PageOffset ROWS 
                FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var productTags = await _dbConnection.QueryAsync<ProductTag>(sql, new
                {
                    request.PageLimit,
                    request.PageOffset,
                    SortField = request.SortField ?? "Created",
                    SortOrder = request.SortOrder ?? "ASC"
                });
                return productTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
