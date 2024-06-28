
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing change logs.
    /// </summary>
    public interface IChangeLogService
    {
        /// <summary>
        /// Creates a new change log entry.
        /// </summary>
        /// <param name="createChangeLogDto">The data transfer object containing the information for the new change log entry.</param>
        /// <returns>A string representing the identifier of the newly created change log entry.</returns>
        Task<string> CreateChangeLog(CreateChangeLogDto createChangeLogDto);

        /// <summary>
        /// Retrieves a change log entry based on the provided request data.
        /// </summary>
        /// <param name="changeLogRequestDto">The data transfer object containing the request information for the change log entry.</param>
        /// <returns>A ChangeLog object representing the retrieved change log entry.</returns>
        Task<ChangeLog> GetChangeLog(ChangeLogRequestDto changeLogRequestDto);

        /// <summary>
        /// Updates an existing change log entry.
        /// </summary>
        /// <param name="updateChangeLogDto">The data transfer object containing the updated information for the change log entry.</param>
        /// <returns>A string representing the identifier of the updated change log entry.</returns>
        Task<string> UpdateChangeLog(UpdateChangeLogDto updateChangeLogDto);

        /// <summary>
        /// Deletes a change log entry based on the provided request data.
        /// </summary>
        /// <param name="deleteChangeLogDto">The data transfer object containing the information for the change log entry to be deleted.</param>
        /// <returns>A boolean indicating whether the change log entry was successfully deleted.</returns>
        Task<bool> DeleteChangeLog(DeleteChangeLogDto deleteChangeLogDto);

        /// <summary>
        /// Retrieves a list of change log entries based on the provided request data.
        /// </summary>
        /// <param name="listChangeLogRequestDto">The data transfer object containing the request information for the list of change log entries.</param>
        /// <returns>A list of ChangeLog objects representing the retrieved change log entries.</returns>
        Task<List<ChangeLog>> GetListChangeLog(ListChangeLogRequestDto listChangeLogRequestDto);
    }
}
