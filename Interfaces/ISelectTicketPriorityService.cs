
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing ticket priority selection.
    /// </summary>
    public interface ISelectTicketPriorityService
    {
        /// <summary>
        /// Creates a new ticket priority selection.
        /// </summary>
        /// <param name="createSelectTicketPriorityDto">The data transfer object for creating a ticket priority selection.</param>
        /// <returns>A string indicating the result of the creation operation.</returns>
        Task<string> CreateSelectTicketPriority(CreateSelectTicketPriorityDto createSelectTicketPriorityDto);

        /// <summary>
        /// Retrieves a ticket priority selection based on the provided request.
        /// </summary>
        /// <param name="selectTicketPriorityRequestDto">The data transfer object for requesting a ticket priority selection.</param>
        /// <returns>The ticket priority selection.</returns>
        Task<SelectTicketPriority> GetSelectTicketPriority(SelectTicketPriorityRequestDto selectTicketPriorityRequestDto);

        /// <summary>
        /// Updates an existing ticket priority selection.
        /// </summary>
        /// <param name="updateSelectTicketPriorityDto">The data transfer object for updating a ticket priority selection.</param>
        /// <returns>A string indicating the result of the update operation.</returns>
        Task<string> UpdateSelectTicketPriority(UpdateSelectTicketPriorityDto updateSelectTicketPriorityDto);

        /// <summary>
        /// Deletes a ticket priority selection.
        /// </summary>
        /// <param name="deleteSelectTicketPriorityDto">The data transfer object for deleting a ticket priority selection.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteSelectTicketPriority(DeleteSelectTicketPriorityDto deleteSelectTicketPriorityDto);

        /// <summary>
        /// Retrieves a list of ticket priority selections based on the provided request.
        /// </summary>
        /// <param name="listSelectTicketPriorityRequestDto">The data transfer object for requesting a list of ticket priority selections.</param>
        /// <returns>A list of ticket priority selections.</returns>
        Task<List<SelectTicketPriority>> GetGetListSelectTicketPriority(ListSelectTicketPriorityRequestDto listSelectTicketPriorityRequestDto);
    }
}
