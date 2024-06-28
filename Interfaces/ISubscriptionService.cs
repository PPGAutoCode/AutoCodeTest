
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing subscription services.
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Creates a new subscription.
        /// </summary>
        /// <param name="createSubscriptionDTO">Data transfer object for creating a subscription.</param>
        /// <returns>A string representing the result of the subscription creation.</returns>
        Task<string> CreateSubscription(CreateSubscriptionDTO createSubscriptionDTO);

        /// <summary>
        /// Retrieves a subscription based on the provided request.
        /// </summary>
        /// <param name="requestSubscription">Request object for retrieving a subscription.</param>
        /// <returns>A Subscription object.</returns>
        Task<Subscription> GetSubscription(RequestSubscription requestSubscription);

        /// <summary>
        /// Updates an existing subscription.
        /// </summary>
        /// <param name="updateSubscriptionDTO">Data transfer object for updating a subscription.</param>
        /// <returns>A string representing the result of the subscription update.</returns>
        Task<string> UpdateSubscription(UpdateSubscriptionDTO updateSubscriptionDTO);

        /// <summary>
        /// Deletes a subscription.
        /// </summary>
        /// <param name="deleteSubscriptionDTO">Data transfer object for deleting a subscription.</param>
        /// <returns>A boolean indicating whether the subscription was successfully deleted.</returns>
        Task<bool> DeleteSubscription(DeleteSubscriptionDTO deleteSubscriptionDTO);

        /// <summary>
        /// Retrieves a list of subscriptions based on the provided request.
        /// </summary>
        /// <param name="listSubscriptionRequestDTO">Request object for retrieving a list of subscriptions.</param>
        /// <returns>A list of Subscription objects.</returns>
        Task<List<Subscription>> GetListSubscription(ListSubscriptionRequestDTO listSubscriptionRequestDTO);
    }
}
