
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
        /// <param name="createSubscriptionDto">The data transfer object containing the details for creating a subscription.</param>
        /// <returns>A string representing the result of the subscription creation.</returns>
        Task<string> CreateSubscription(CreateSubscriptionDto createSubscriptionDto);

        /// <summary>
        /// Retrieves a subscription based on the provided request details.
        /// </summary>
        /// <param name="requestSubscriptionDto">The data transfer object containing the details for requesting a subscription.</param>
        /// <returns>A Subscription object representing the requested subscription.</returns>
        Task<Subscription> GetSubscription(RequestSubscriptionDto requestSubscriptionDto);

        /// <summary>
        /// Updates an existing subscription.
        /// </summary>
        /// <param name="updateSubscriptionDto">The data transfer object containing the details for updating a subscription.</param>
        /// <returns>A string representing the result of the subscription update.</returns>
        Task<string> UpdateSubscription(UpdateSubscriptionDto updateSubscriptionDto);

        /// <summary>
        /// Deletes a subscription based on the provided details.
        /// </summary>
        /// <param name="deleteSubscriptionDto">The data transfer object containing the details for deleting a subscription.</param>
        /// <returns>A boolean indicating whether the subscription was successfully deleted.</returns>
        Task<bool> DeleteSubscription(DeleteSubscriptionDto deleteSubscriptionDto);

        /// <summary>
        /// Retrieves a list of subscriptions based on the provided request details.
        /// </summary>
        /// <param name="listSubscriptionRequestDto">The data transfer object containing the details for requesting a list of subscriptions.</param>
        /// <returns>A list of Subscription objects representing the requested subscriptions.</returns>
        Task<List<Subscription>> GetListSubscription(ListSubscriptionRequestDto listSubscriptionRequestDto);
    }
}
