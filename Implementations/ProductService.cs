
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

        public async Task CreateProductAsync(Product product)
        {
            ValidateProduct(product);

            const string sql = @"
                INSERT INTO Products (
                    Id, Name, ApicHostname, Advantages, Features, RelatedProducts, ProductComparison, 
                    APIEndpoints, ProductTags, ProductCategories, ProductSubscribers, ProductDomainId, 
                    AttachmentsId, AppEnviroment, Deprecated, DisableDocumentation, HeaderImage, Label, 
                    OverviewDisplay, Description, Enabled, ImageId, Visible, Weight, Langcode, Sticky, 
                    Status, Promote, CommercialProduct, ProductVersion, Version, Created, Changed, 
                    CreatorId, ChangedUser
                ) VALUES (
                    @Id, @Name, @ApicHostname, @Advantages, @Features, @RelatedProducts, @ProductComparison, 
                    @APIEndpoints, @ProductTags, @ProductCategories, @ProductSubscribers, @ProductDomainId, 
                    @AttachmentsId, @AppEnviroment, @Deprecated, @DisableDocumentation, @HeaderImage, @Label, 
                    @OverviewDisplay, @Description, @Enabled, @ImageId, @Visible, @Weight, @Langcode, @Sticky, 
                    @Status, @Promote, @CommercialProduct, @ProductVersion, @Version, @Created, @Changed, 
                    @CreatorId, @ChangedUser
                )";

            await _dbConnection.ExecuteAsync(sql, product);
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
            {
                throw new BusinessException("1002", "Product cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new BusinessException("1003", "Product name cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(product.ProductVersion))
            {
                throw new BusinessException("1004", "Product version cannot be null or empty.");
            }

            if (product.Version <= 0)
            {
                throw new BusinessException("1005", "Product version must be greater than zero.");
            }

            if (product.Created == default)
            {
                throw new BusinessException("1006", "Product creation date cannot be default.");
            }

            if (product.Changed == default)
            {
                throw new BusinessException("1007", "Product change date cannot be default.");
            }

            if (product.CreatorId == Guid.Empty)
            {
                throw new BusinessException("1008", "Creator ID cannot be empty.");
            }
        }
    }
}
