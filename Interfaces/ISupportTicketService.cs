
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support tickets.
    /// </summary>
    public interface ISupportTicketService
    {
        /// <summary>
        /// Creates a new support ticket.
        /// </summary>
        /// <param name="createSupportTicketDto">The data transfer object containing the details for creating a support ticket.</param>
        /// <returns>A string representing the unique identifier of the created support ticket.</returns>
        Task<string> CreateSupportTicketAsync(CreateSupportTicketDto createSupportTicketDto);

        /// <summary>
        /// Retrieves a support ticket by its unique identifier.
        /// </summary>
        /// <param name="requestSupportTicketDto">The data transfer object containing the details for requesting a support ticket.</param>
        /// <returns>A SupportTicket object representing the requested support ticket.</returns>
        Task<SupportTicket> GetSupportTicketAsync(RequestSupportTicketDto requestSupportTicketDto);

        /// <summary>
        /// Updates an existing support ticket.
        /// </summary>
        /// <param name="updateSupportTicketDto">The data transfer object containing the details for updating a support ticket.</param>
        /// <returns>A string representing the unique identifier of the updated support ticket.</returns>
        Task<string> UpdateSupportTicketAsync(UpdateSupportTicketDto updateSupportTicketDto);

        /// <summary>
        /// Deletes a support ticket by its unique identifier.
        /// </summary>
        /// <param name="deleteSupportTicketDto">The data transfer object containing the details for deleting a support ticket.</param>
        /// <returns>A boolean indicating whether the support ticket was successfully deleted.</returns>
        Task<bool> DeleteSupportTicketAsync(DeleteSupportTicketDto deleteSupportTicketDto);

        /// <summary>
        /// Retrieves a list of support tickets based on the provided criteria.
        /// </summary>
        /// <param name="listSupportTicketRequestDto">The data transfer object containing the criteria for listing support tickets.</param>
        /// <returns>A list of SupportTicket objects representing the requested support tickets.</returns>
        Task<List<SupportTicket>> GetListSupportTicketAsync(ListSupportTicketRequestDto listSupportTicketRequestDto);
    }
}
