
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
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public ProductCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductCategory(CreateProductCategoryDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (request.Parent.HasValue)
            {
                var parentExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM ProductCategory WHERE Id = @Parent",
                    new { Parent = request.Parent.Value });

                if (!parentExists)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
            }

            var productCategory = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                UserQuestionnaire = request.UserQuestionnaire,
                Description = request.Description,
                Parent = request.Parent,
                UrlAlias = request.UrlAlias,
                Weight = request.Weight
            };

            var sql = @"
                INSERT INTO ProductCategory (Id, Name, UserQuestionnaire, Description, Parent, UrlAlias, Weight)
                VALUES (@Id, @Name, @UserQuestionnaire, @Description, @Parent, @UrlAlias, @Weight)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, productCategory);
                return productCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ProductCategory> GetProductCategory(ProductCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM ProductCategory WHERE Id = @Id";
            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(sql, new { Id = request.Id });

            if (productCategory == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            return productCategory;
        }

        public async Task<string> UpdateProductCategory(UpdateProductCategoryDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategory WHERE Id = @Id",
                new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            existingCategory.Name = request.Name;
            existingCategory.UserQuestionnaire = request.UserQuestionnaire;
            existingCategory.Description = request.Description;
            existingCategory.Parent = request.Parent;
            existingCategory.UrlAlias = request.UrlAlias;
            existingCategory.Weight = request.Weight;

            var sql = @"
                UPDATE ProductCategory 
                SET Name = @Name, UserQuestionnaire = @UserQuestionnaire, Description = @Description, 
                    Parent = @Parent, UrlAlias = @UrlAlias, Weight = @Weight 
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, existingCategory);
                return existingCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteProductCategory(DeleteProductCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategory WHERE Id = @Id",
                new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            var sql = "DELETE FROM ProductCategory WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<ProductCategory>> GetListProductCategory(ListProductCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM ProductCategory 
                ORDER BY @SortField @SortOrder 
                OFFSET @PageOffset ROWS 
                FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var productCategories = await _dbConnection.QueryAsync<ProductCategory>(sql, new
                {
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder
                });

                return productCategories.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
