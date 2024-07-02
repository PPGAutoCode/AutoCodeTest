
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
    public class ProductService : IProductService
    {
        private readonly IDbConnection _dbConnection;

        public ProductService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProduct(CreateProductDto productDto)
        {
            // Step 1: Validate that all fields except [Advantages, Features, RelatedProducts, APIEndpoint, Tags] are not null.
            if (string.IsNullOrEmpty(productDto.Name) || string.IsNullOrEmpty(productDto.ProductVersion))
            {
                throw new BusinessException("DP-422", "Mandatory fields are missing.");
            }

            // Step 2: Process each item in the ProductCategories list
            foreach (var categoryId in productDto.ProductCategories)
            {
                var category = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>("SELECT * FROM ProductCategories WHERE Id = @Id", new { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-404", $"ProductCategory with ID {categoryId} not found.");
                }
            }

            // Step 3: Process each item in the Advantages list
            foreach (var advantageId in productDto.Advantages)
            {
                var advantage = await _dbConnection.QuerySingleOrDefaultAsync<ProductAdvantage>("SELECT * FROM ProductAdvantages WHERE Id = @Id", new { Id = advantageId });
                if (advantage == null)
                {
                    throw new BusinessException("DP-404", $"Advantage with ID {advantageId} not found.");
                }
            }

            // Step 4: Process each item in the Features list
            foreach (var featureId in productDto.Features)
            {
                var feature = await _dbConnection.QuerySingleOrDefaultAsync<ProductFeature>("SELECT * FROM ProductFeatures WHERE Id = @Id", new { Id = featureId });
                if (feature == null)
                {
                    throw new BusinessException("DP-404", $"Feature with ID {featureId} not found.");
                }
            }

            // Step 5: Process each item in the RelatedProducts list
            foreach (var relatedProductId in productDto.RelatedProducts)
            {
                var relatedProduct = await _dbConnection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = relatedProductId });
                if (relatedProduct == null)
                {
                    throw new BusinessException("DP-404", $"Related Product with ID {relatedProductId} not found.");
                }
            }

            // Step 6: Process each item in the APIEndpoint list
            foreach (var apiEndpointId in productDto.APIEndpoints)
            {
                var apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = apiEndpointId });
                if (apiEndpoint == null)
                {
                    throw new BusinessException("DP-404", $"API Endpoint with ID {apiEndpointId} not found.");
                }
            }

            // Step 7: Process each item in the ProductTags list
            foreach (var tagName in productDto.ProductTags)
            {
                var tag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>("SELECT * FROM ProductTags WHERE Name = @Name", new { Name = tagName });
                if (tag == null)
                {
                    throw new BusinessException("DP-404", $"Tag with name {tagName} not found.");
                }
            }

            // Step 8: Process each item in the ProductSubscribers list
            foreach (var subscriberId in productDto.ProductSubscribers)
            {
                var subscriber = await _dbConnection.QuerySingleOrDefaultAsync<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = subscriberId });
                if (subscriber == null)
                {
                    throw new BusinessException("DP-404", $"Subscriber with ID {subscriberId} not found.");
                }
            }

            // Step 9: Process each item in the ProductDomain list
            foreach (var domainId in productDto.ProductDomain)
            {
                var domain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>("SELECT * FROM ProductDomains WHERE Id = @Id", new { Id = domainId });
                if (domain == null)
                {
                    throw new BusinessException("DP-404", $"ProductDomain with ID {domainId} not found.");
                }
            }

            // Insert the product into the database
            var productId = Guid.NewGuid();
            var query = @"
                INSERT INTO Products (Id, Name, Description, ProductVersion, Enabled, ApicHostname, AttachmentsId, AppEnviromentId, HeaderImage, Label, OverviewDisplay, Domain, ImageId, Weight, Langcode, Sticky, Status, Promote, CommercialProduct, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Description, @ProductVersion, @Enabled, @ApicHostname, @AttachmentsId, @AppEnviromentId, @HeaderImage, @Label, @OverviewDisplay, @Domain, @ImageId, @Weight, @Langcode, @Sticky, @Status, @Promote, @CommercialProduct, GETDATE(), GETDATE(), @CreatorId, @ChangedUser)";

            await _dbConnection.ExecuteAsync(query, new
            {
                Id = productId,
                productDto.Name,
                productDto.Description,
                productDto.ProductVersion,
                productDto.Enabled,
                productDto.ApicHostname,
                productDto.AttachmentsId,
                productDto.AppEnviromentId,
                productDto.HeaderImage,
                productDto.Label,
                productDto.OverviewDisplay,
                productDto.Domain,
                productDto.ImageId,
                productDto.Weight,
                productDto.Langcode,
                productDto.Sticky,
                productDto.Status,
                productDto.Promote,
                productDto.CommercialProduct,
                CreatorId = Guid.NewGuid(), // Assuming a new creator ID is generated
                ChangedUser = Guid.NewGuid() // Assuming a new changed user ID is generated
            });

            return productId.ToString();
        }
    }
}
