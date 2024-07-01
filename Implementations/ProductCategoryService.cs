
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
            // Step 1: Validate that the request payload contains the necessary parameter ("Name").
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Validate that the provided parent category ID exists if it's included in the request payload.
            if (request.Parent.HasValue)
            {
                var parentExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM ProductCategory WHERE Id = @Parent",
                    new { Parent = request.Parent.Value });

                if (!parentExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 3: Create a new ProductCategory object with the provided details.
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

            // Step 4: Insert the newly created ProductCategory object to the database.
            var sql = @"
                INSERT INTO ProductCategory (Id, Name, UserQuestionnaire, Description, Parent, UrlAlias, Weight)
                VALUES (@Id, @Name, @UserQuestionnaire, @Description, @Parent, @UrlAlias, @Weight)";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, productCategory);

            // Step 5: If the transaction is successful, return the new ProductCategory ID.
            if (rowsAffected > 0)
            {
                return productCategory.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ProductCategory> GetProductCategory(ProductCategoryRequestDto request)
        {
            // Step 1: Validate that request.payload.Id is not null.
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the blog category from the database based on the provided category ID.
            var sql = "SELECT * FROM ProductCategory WHERE Id = @Id";
            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(sql, new { Id = request.Id });

            // Step 3: If the category exists, return it as the response payload.
            if (productCategory != null)
            {
                return productCategory;
            }
            else
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateProductCategory(UpdateProductCategoryDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters ("Id" and "Name").
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ProductCategory from the database by Id.
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategory WHERE Id = @Id",
                new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the ProductCategory object with the provided changes.
            existingCategory.Name = request.Name;
            existingCategory.UserQuestionnaire = request.UserQuestionnaire;
            existingCategory.Description = request.Description;
            existingCategory.Parent = request.Parent;
            existingCategory.UrlAlias = request.UrlAlias;
            existingCategory.Weight = request.Weight;

            // Step 4: Save the updated ProductCategory object to the database.
            var sql = @"
                UPDATE ProductCategory 
                SET Name = @Name, UserQuestionnaire = @UserQuestionnaire, Description = @Description, 
                    Parent = @Parent, UrlAlias = @UrlAlias, Weight = @Weight 
                WHERE Id = @Id";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, existingCategory);

            // Step 5: If the transaction is successful, return the ProductCategory ID.
            if (rowsAffected > 0)
            {
                return existingCategory.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteProductCategory(DeleteProductCategoryDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter ("Id").
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ProductCategory from the database by Id.
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategory WHERE Id = @Id",
                new { Id = request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the ProductCategory object from the database.
            var sql = "DELETE FROM ProductCategory WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });

            // Step 4: If the transaction is successful, return true.
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<ProductCategory>> GetListProductCategory(ListProductCategoryRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters ("PageNumber" and "PageSize").
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of ProductCategories from the database based on the provided pagination parameters.
            var sql = @"
                SELECT * FROM ProductCategory 
                ORDER BY @SortField @SortOrder 
                OFFSET @PageOffset ROWS 
                FETCH NEXT @PageLimit ROWS ONLY";

            var productCategories = await _dbConnection.QueryAsync<ProductCategory>(sql, new
            {
                PageOffset = request.PageOffset,
                PageLimit = request.PageLimit,
                SortField = request.SortField,
                SortOrder = request.SortOrder
            });

            // Step 3: If the transaction is successful, return the list of ProductCategories.
            return productCategories.ToList();
        }
    }
}
