
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
        /// <param name="createSupportTicketDto">The data transfer object containing the details of the support ticket to be created.</param>
        /// <returns>A string representing the unique identifier of the newly created support ticket.</returns>
        Task<string> CreateSupportTicket(CreateSupportTicketDto createSupportTicketDto);
    }
}
