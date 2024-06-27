
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support ticket states.
    /// </summary>
    public interface ISupportTicketStateService
    {
        /// <summary>
        /// Creates a new support ticket state.
        /// </summary>
        /// <param name="createSupportTicketStateDto">The data transfer object containing the details for the new support ticket state.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> CreateSupportTicketState(CreateSupportTicketStateDto createSupportTicketStateDto);

        /// <summary>
        /// Retrieves a support ticket state based on the provided request.
        /// </summary>
        /// <param name="supportTicketStateRequestDto">The data transfer object containing the request details.</param>
        /// <returns>A SupportTicketState object representing the retrieved support ticket state.</returns>
        Task<SupportTicketState> GetSupportTicketState(SupportTicketStateRequestDto supportTicketStateRequestDto);

        /// <summary>
        /// Updates an existing support ticket state.
        /// </summary>
        /// <param name="updateSupportTicketStateDto">The data transfer object containing the updated details for the support ticket state.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> UpdateSupportTicketState(UpdateSupportTicketStateDto updateSupportTicketStateDto);

        /// <summary>
        /// Deletes a support ticket state.
        /// </summary>
        /// <param name="deleteSupportTicketStateDto">The data transfer object containing the details for the support ticket state to be deleted.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DeleteSupportTicketState(DeleteSupportTicketStateDto deleteSupportTicketStateDto);

        /// <summary>
        /// Retrieves a list of support ticket states based on the provided request.
        /// </summary>
        /// <param name="listSupportTicketStateRequestDto">The data transfer object containing the request details.</param>
        /// <returns>A list of SupportTicketState objects representing the retrieved support ticket states.</returns>
        Task<List<SupportTicketState>> GetListSupportTicketState(ListSupportTicketStateRequestDto listSupportTicketStateRequestDto);
    }
}
