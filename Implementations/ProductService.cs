
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
            if (string.IsNullOrEmpty(createProductDto.Name) ||
                string.IsNullOrEmpty(createProductDto.Description) ||
                createProductDto.Comparison == null ||
                string.IsNullOrEmpty(createProductDto.ProductCategoryId) ||
                createProductDto.Version == 0 ||
                !createProductDto.Enabled ||
                string.IsNullOrEmpty(createProductDto.ApicHostname) ||
                string.IsNullOrEmpty(createProductDto.Attachments) ||
                string.IsNullOrEmpty(createProductDto.EnvironmentId) ||
                string.IsNullOrEmpty(createProductDto.HeaderImage) ||
                string.IsNullOrEmpty(createProductDto.Label) ||
                !createProductDto.OverviewDisplay ||
                string.IsNullOrEmpty(createProductDto.Domain) ||
                string.IsNullOrEmpty(createProductDto.ImageId) ||
                !createProductDto.Visible ||
                createProductDto.Weight == null ||
                string.IsNullOrEmpty(createProductDto.Langcode) ||
                !createProductDto.Sticky ||
                !createProductDto.Status ||
                !createProductDto.Promote ||
                !createProductDto.CommercialProduct)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ProductCategory from the database by ProductCategoryId.
            var productCategory = await _dbConnection.QueryFirstOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategories WHERE Id = @ProductCategoryId",
                new { ProductCategoryId = createProductDto.ProductCategoryId });

            if (productCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Process each item in the Advantages list.
            var advantages = new List<Advantage>();
            foreach (var advantageId in createProductDto.Advantages)
            {
                var advantage = await _dbConnection.QueryFirstOrDefaultAsync<Advantage>(
                    "SELECT * FROM Advantages WHERE Id = @AdvantageId",
                    new { AdvantageId = advantageId });

                if (advantage == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                advantages.Add(advantage);
            }

            // Step 4: Process each item in the Features list.
            var features = new List<Feature>();
            foreach (var featureId in createProductDto.Features)
            {
                var feature = await _dbConnection.QueryFirstOrDefaultAsync<Feature>(
                    "SELECT * FROM Features WHERE Id = @FeatureId",
                    new { FeatureId = featureId });

                if (feature == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                features.Add(feature);
            }

            // Step 5: Process each item in the RelatedProducts list.
            var relatedProducts = new List<RelatedProduct>();
            foreach (var relatedProductId in createProductDto.RelatedProducts)
            {
                var relatedProduct = await _dbConnection.QueryFirstOrDefaultAsync<RelatedProduct>(
                    "SELECT * FROM RelatedProducts WHERE Id = @RelatedProductId",
                    new { RelatedProductId = relatedProductId });

                if (relatedProduct == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                relatedProducts.Add(relatedProduct);
            }

            // Step 6: Process each item in the APIEndpoint list.
            var apiEndpoints = new List<APIEndpoint>();
            foreach (var apiEndpointId in createProductDto.APIEndpoint)
            {
                var apiEndpoint = await _dbConnection.QueryFirstOrDefaultAsync<APIEndpoint>(
                    "SELECT * FROM APIEndpoints WHERE Id = @APIEndpointId",
                    new { APIEndpointId = apiEndpointId });

                if (apiEndpoint == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                apiEndpoints.Add(apiEndpoint);
            }

            // Step 7: Process each item in the Tags list.
            var tags = new List<Tag>();
            foreach (var tagName in createProductDto.Tags)
            {
                var tag = await _dbConnection.QueryFirstOrDefaultAsync<Tag>(
                    "SELECT * FROM Tags WHERE Name = @TagName",
                    new { TagName = tagName });

                if (tag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                tags.Add(tag);
            }

            // Step 8: Process each item in the Subscribers list.
            var subscribers = new List<User>();
            foreach (var subscriberId in createProductDto.Subscribers)
            {
                var subscriber = await _dbConnection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE Id = @SubscriberId",
                    new { SubscriberId = subscriberId });

                if (subscriber == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                subscribers.Add(subscriber);
            }

            // Step 9: Create a new Product object (product) from the arguments.
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Comparison = createProductDto.Comparison,
                ProductCategoryId = createProductDto.ProductCategoryId,
                Version = createProductDto.Version,
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
                Changed = DateTime.UtcNow
            };

            // Step 10: Create new lists of related objects.
            var productAdvantages = advantages.Select(a => new ProductAdvantage { ProductId = product.Id, AdvantageId = a.Id }).ToList();
            var productFeatures = features.Select(f => new ProductFeature { ProductId = product.Id, FeatureId = f.Id }).ToList();
            var productAPIEndpoints = apiEndpoints.Select(ae => new ProductAPIEndpoint { ProductId = product.Id, APIEndpointId = ae.Id }).ToList();
            var productTags = tags.Select(t => new ProductTag { ProductId = product.Id, TagId = t.Id }).ToList();
            var relatedProductsList = relatedProducts.Select(rp => new RelatedProduct { ProductId = product.Id, RelatedProductId = rp.Id }).ToList();
            var productSubscribers = subscribers.Select(s => new ProductSubscriber { ProductId = product.Id, SubscriberId = s.Id }).ToList();

            // Step 11: In a single SQL transaction.
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert product in the database table Product.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO Products (Id, Name, Description, Comparison, ProductCategoryId, Version, Enabled, ApicHostname, Attachments, EnvironmentId, HeaderImage, Label, OverviewDisplay, Domain, ImageId, Weight, Langcode, Sticky, Status, Promote, CommercialProduct, Created, Changed) " +
                        "VALUES (@Id, @Name, @Description, @Comparison, @ProductCategoryId, @Version, @Enabled, @ApicHostname, @Attachments, @EnvironmentId, @HeaderImage, @Label, @OverviewDisplay, @Domain, @ImageId, @Weight, @Langcode, @Sticky, @Status, @Promote, @CommercialProduct, @Created, @Changed)",
                        product, transaction);

                    // Insert productAdvantages in the database table ProductAdvantages.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductAdvantages (ProductId, AdvantageId) VALUES (@ProductId, @AdvantageId)",
                        productAdvantages, transaction);

                    // Insert productFeatures in the database table ProductFeatures.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductFeatures (ProductId, FeatureId) VALUES (@ProductId, @FeatureId)",
                        productFeatures, transaction);

                    // Insert relatedProducts in the database table RelatedProducts.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO RelatedProducts (ProductId, RelatedProductId) VALUES (@ProductId, @RelatedProductId)",
                        relatedProductsList, transaction);

                    // Insert productApiEndpoints in the database table ProductApiEndpoints.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductApiEndpoints (ProductId, APIEndpointId) VALUES (@ProductId, @APIEndpointId)",
                        productAPIEndpoints, transaction);

                    // Insert productTags in the database table ProductTags.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductTags (ProductId, TagId) VALUES (@ProductId, @TagId)",
                        productTags, transaction);

                    // Insert productSubscribers in the database table ProductSubscribers.
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO ProductSubscribers (ProductId, SubscriberId) VALUES (@ProductId, @SubscriberId)",
                        productSubscribers, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 12: Return ProductId from the database.
            return product.Id.ToString();
        }
    }
}
