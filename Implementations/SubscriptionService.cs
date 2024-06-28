
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
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IDbConnection _dbConnection;

        public SubscriptionService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSubscription(CreateSubscriptionDto request)
        {
            // Step 1: Validate all fields of request.payload
            if (request.ApplicationsId == Guid.Empty || request.ProductsId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch application from the database by id from the ApplicationId argument
            var application = await _dbConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id = @Id", new { Id = request.ApplicationsId });
            if (application == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch product from the database by id, providing item.ProductId
            var product = await _dbConnection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = request.ProductsId });
            if (product == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Create a new Subscription object for each product as follows from the arguments
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                ApplicationsId = request.ApplicationsId,
                ProductsId = request.ProductsId,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a creator ID is needed
                ChangedUser = Guid.NewGuid() // Assuming a changed user ID is needed
            };

            // Step 5: In a single SQL transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO Subscriptions (Id, ApplicationsId, ProductsId, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ApplicationsId, @ProductsId, @Created, @Changed, @CreatorId, @ChangedUser)", subscription, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return SubscriptionId from database
            return subscription.Id.ToString();
        }

        public async Task<Subscription> GetSubscription(RequestSubscriptionDto request)
        {
            // Step 1: Validate input
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch subscription from the database by id
            var subscription = await _dbConnection.QueryFirstOrDefaultAsync<Subscription>("SELECT * FROM Subscriptions WHERE Id = @Id", new { Id = request.Id });
            if (subscription == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch the application from the database using ApplicationId from the subscription
            var application = await _dbConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id = @Id", new { Id = subscription.ApplicationsId });
            if (application == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Fetch the product from the database using ProductId from the subscription
            var product = await _dbConnection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = subscription.ProductsId });
            if (product == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 5: Map database objects to Subscription
            subscription.Application = application;
            subscription.Product = product;

            // Step 6: Return the fully populated subscription object including related application and product details
            return subscription;
        }

        public async Task<string> UpdateSubscription(UpdateSubscriptionDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || request.ApplicationsId == Guid.Empty || request.ProductsId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing Subscription
            var subscription = await _dbConnection.QueryFirstOrDefaultAsync<Subscription>("SELECT * FROM Subscriptions WHERE Id = @Id", new { Id = request.Id });
            if (subscription == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Validate Related Entities
            var application = await _dbConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id = @Id", new { Id = request.ApplicationsId });
            if (application == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var product = await _dbConnection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = request.ProductsId });
            if (product == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 4: Update the Subscription object with the provided changes
            subscription.ApplicationsId = request.ApplicationsId;
            subscription.ProductsId = request.ProductsId;
            subscription.Changed = DateTime.UtcNow;

            // Step 5: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE Subscriptions SET ApplicationsId = @ApplicationsId, ProductsId = @ProductsId, Changed = @Changed WHERE Id = @Id", subscription, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return SubscriptionId from database
            return subscription.Id.ToString();
        }

        public async Task<bool> DeleteSubscription(DeleteSubscriptionDto request)
        {
            // Step 1: Validate that the request.payload.id contains the necessary parameter (Id)
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing Subscription object from the database based on the provided subscription ID
            var subscription = await _dbConnection.QueryFirstOrDefaultAsync<Subscription>("SELECT * FROM Subscriptions WHERE Id = @Id", new { Id = request.Id });
            if (subscription == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the Subscription object from the database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM Subscriptions WHERE Id = @Id", new { Id = request.Id }, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 4: Return true if the transaction is successful
            return true;
        }

        public async Task<List<Subscription>> GetListSubscription(ListSubscriptionRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary pagination parameters (PageLimit and PageOffset)
            if (request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of Subscriptions from the database based on the provided pagination parameters and optional sorting
            var query = "SELECT * FROM Subscriptions";
            if (request.SortField != null && request.SortOrder != null)
            {
                query += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            query += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var subscriptions = await _dbConnection.QueryAsync<Subscription>(query, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            // Step 3: Return the list of Subscriptions as the response payload
            return subscriptions.ToList();
        }
    }
}
