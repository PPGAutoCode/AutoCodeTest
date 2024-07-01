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

        public async Task<string> CreateProduct(CreateProductDto createProductDto)
        {
            // Step 1: Validate that all fields except [Advantages, Features, RelatedProducts, APIEndpoint, Tags] are not null.
            ValidateCreateProductDto(createProductDto);

            // Step 3: Fetch the ProductCategory from the database by ProductCategoryId.
            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategories WHERE Id = @ProductCategoryId",
                new { createProductDto.ProductCategoryId });

            if (productCategory == null)
            {
                throw new TechnicalException("DP-404", "Product category not found.");
            }

            // Step 5: Process each item in the Advantages list.
            var advantages = new List<Advantage>();
            foreach (var advantageId in createProductDto.Advantages)
            {
                var advantage = await _dbConnection.QuerySingleOrDefaultAsync<Advantage>(
                    "SELECT * FROM Advantages WHERE Id = @AdvantageId",
                    new { AdvantageId = advantageId });

                if (advantage == null)
                {
                    throw new TechnicalException("DP-404", "Advantage not found.");
                }
                advantages.Add(advantage);
            }

            // Step 8: Process each item in the Features list.
            var features = new List<Feature>();
            foreach (var featureId in createProductDto.Features)
            {
                var feature = await _dbConnection.QuerySingleOrDefaultAsync<Feature>(
                    "SELECT * FROM Features WHERE Id = @FeatureId",
                    new { FeatureId = featureId });

                if (feature == null)
                {
                    throw new TechnicalException("DP-404", "Feature not found.");
                }
                features.Add(feature);
            }

            // Step 11: Process each item in the RelatedProducts list.
            var relatedProducts = new List<RelatedProduct>();
            foreach (var relatedProductId in createProductDto.RelatedProducts)
            {
                var relatedProduct = await _dbConnection.QuerySingleOrDefaultAsync<RelatedProduct>(
                    "SELECT * FROM RelatedProducts WHERE Id = @RelatedProductId",
                    new { RelatedProductId = relatedProductId });

                if (relatedProduct == null)
                {
                    throw new TechnicalException("DP-404", "Related product not found.");
                }
                relatedProducts.Add(relatedProduct);
            }

            // Step 14: Process each item in the APIEndpoint list.
            var apiEndpoints = new List<APIEndpoint>();
            foreach (var apiEndpointId in createProductDto.APIEndpoint)
            {
                var apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>(
                    "SELECT * FROM APIEndpoints WHERE Id = @APIEndpointId",
                    new { APIEndpointId = apiEndpointId });

                if (apiEndpoint == null)
                {
                    throw new TechnicalException("DP-404", "API endpoint not found.");
                }
                apiEndpoints.Add(apiEndpoint);
            }

            // Step 17: Process each item in the ProductTags list.
            var tags = new List<ProductTag>();
            foreach (var tagName in createProductDto.Tags)
            {
                var tag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(
                    "SELECT * FROM ProductTags WHERE Name = @TagName",
                    new { TagName = tagName });

                if (tag == null)
                {
                    throw new TechnicalException("DP-404", "Tag not found.");
                }
                tags.Add(tag);
            }

            // Step 23: Create a new Product object (product) from the arguments.
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Comparison = createProductDto.Comparison,
                ProductCategoryId = createProductDto.ProductCategoryId,
                ProductVersion = createProductDto.ProductVersion,
                Enabled = createProductDto.Enabled,
                ApicHostname = createProductDto.ApicHostname,
                Attachments = createProductDto.Attachments,
                EnvironmentId = createProductDto.EnvironmentId,
                HeaderImage = createProductDto.HeaderImage,
                Label = createProductDto.Label,
                OverviewDisplay = createProductDto.OverviewDisplay,
                Domain = createProductDto.Domain,
                ImageId = createProductDto.ImageId,
                Weight = createProductDto.Weight,
                Langcode = createProductDto.Langcode,
                Sticky = createProductDto.Sticky,
                Status = createProductDto.Status,
                Promote = createProductDto.Promote,
                CommercialProduct = createProductDto.CommercialProduct,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                Version = 1
            };

            // Step 48: Create new lists of related objects.
            var productAdvantages = advantages.Select(a => new ProductAdvantage { ProductId = product.Id, AdvantageId = a.Id }).ToList();
            var productFeatures = features.Select(f => new ProductFeature { ProductId = product.Id, FeatureId = f.Id }).ToList();
            var productAPIEndpoints = apiEndpoints.Select(e => new ProductAPIEndpoint { ProductId = product.Id, APIEndpointId = e.Id }).ToList();
            var productTags = tags.Select(t => new ProductTag { ProductId = product.Id, TagId = t.Id }).ToList();
            var relatedProductsList = relatedProducts.Select(rp => new RelatedProduct { ProductId = product.Id, RelatedProductId = rp.Id }).ToList();

            // Step 57: In a single SQL transaction.
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Step 58: Insert product in the database table Product.
                    await _dbConnection.ExecuteAsync(
                        @"INSERT INTO Products (Id, Name, Description, Comparison, ProductCategoryId, ProductVersion, Enabled, ApicHostname, Attachments, EnvironmentId, HeaderImage, Label, OverviewDisplay, Domain, ImageId, Weight, Langcode, Sticky, Status, Promote, CommercialProduct, Created, Changed, Version)
                          VALUES (@Id, @Name, @Description, @Comparison, @ProductCategoryId, @ProductVersion, @Enabled, @ApicHostname, @Attachments, @EnvironmentId, @HeaderImage, @Label, @OverviewDisplay, @Domain, @ImageId, @Weight, @Langcode, @Sticky, @Status, @Promote, @CommercialProduct, @Created, @Changed, @Version)",
                        product, transaction);

                    // Step 59: Insert productAdvantages in the database table ProductAdvantages.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductAdvantages (ProductId, AdvantageId) VALUES (@ProductId, @AdvantageId)",
                        productAdvantages, transaction);

                    // Step 60: Insert productFeatures in the database table ProductFeatures.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductFeatures (ProductId, FeatureId) VALUES (@ProductId, @FeatureId)",
                        productFeatures, transaction);

                    // Step 61: Insert relatedProducts in the database table RelatedProducts.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO RelatedProducts (ProductId, RelatedProductId) VALUES (@ProductId, @RelatedProductId)",
                        relatedProductsList, transaction);

                    // Step 62: Insert productApiEndpoints in the database table ProductApiEndpoints.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductApiEndpoints (ProductId, APIEndpointId) VALUES (@ProductId, @APIEndpointId)",
                        productAPIEndpoints, transaction);

                    // Step 63: Insert productTags in the database table ProductTags.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductTags (ProductId, TagId) VALUES (@ProductId, @TagId)",
                        productTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            // Step 67: Return ProductId from the database.
            return product.Id.ToString();
        }

        private void ValidateCreateProductDto(CreateProductDto createProductDto)
        {
            if (string.IsNullOrEmpty(createProductDto.Name) ||
                string.IsNullOrEmpty(createProductDto.Description) ||
                createProductDto.ProductCategoryId == Guid.Empty ||
                string.IsNullOrEmpty(createProductDto.ProductVersion))
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }
    }
}