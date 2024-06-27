
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing severity levels.
    /// </summary>
    public interface ISeverityService
    {
        /// <summary>
        /// Creates a new severity level.
        /// </summary>
        /// <param name="createSeverityDto">Data transfer object for creating a severity level.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> CreateSeverity(CreateSeverityDto createSeverityDto);

        /// <summary>
        /// Retrieves a severity level based on the provided request.
        /// </summary>
        /// <param name="severityRequestDto">Data transfer object for requesting a severity level.</param>
        /// <returns>The requested severity level.</returns>
        Task<Severity> GetSeverity(SeverityRequestDto severityRequestDto);

        /// <summary>
        /// Updates an existing severity level.
        /// </summary>
        /// <param name="updateSeverityDto">Data transfer object for updating a severity level.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> UpdateSeverity(UpdateSeverityDto updateSeverityDto);

        /// <summary>
        /// Deletes a severity level.
        /// </summary>
        /// <param name="deleteSeverityDto">Data transfer object for deleting a severity level.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DeleteSeverity(DeleteSeverityDto deleteSeverityDto);

        /// <summary>
        /// Retrieves a list of severity levels based on the provided request.
        /// </summary>
        /// <param name="listSeverityRequestDto">Data transfer object for requesting a list of severity levels.</param>
        /// <returns>A list of severity levels.</returns>
        Task<List<Severity>> GetListSeverity(ListSeverityRequestDto listSeverityRequestDto);
    }
}
