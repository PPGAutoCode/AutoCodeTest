
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing the conditions under which getting started tasks are considered completed.
    /// </summary>
    public interface IGettingStartedCompletedConditionService
    {
        /// <summary>
        /// Creates a new getting started completed condition.
        /// </summary>
        /// <param name="dto">The data transfer object containing the information needed to create the condition.</param>
        /// <returns>A string representing the identifier of the newly created condition.</returns>
        Task<string> CreateGettingStartedCompletedCondition(CreateGettingStartedCompletedConditionDto dto);

        /// <summary>
        /// Retrieves a getting started completed condition based on the provided request data.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the request information.</param>
        /// <returns>A GettingStartedCompletedCondition object representing the retrieved condition.</returns>
        Task<GettingStartedCompletedCondition> GetGettingStartedCompletedCondition(GettingStartedCompletedConditionRequestDto requestDto);

        /// <summary>
        /// Updates an existing getting started completed condition.
        /// </summary>
        /// <param name="dto">The data transfer object containing the updated information.</param>
        /// <returns>A string representing the identifier of the updated condition.</returns>
        Task<string> UpdateGettingStartedCompletedCondition(UpdateGettingStartedCompletedConditionDto dto);

        /// <summary>
        /// Deletes a getting started completed condition.
        /// </summary>
        /// <param name="dto">The data transfer object containing the information needed to delete the condition.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteGettingStartedCompletedCondition(DeleteGettingStartedCompletedConditionDto dto);

        /// <summary>
        /// Retrieves a list of getting started completed conditions based on the provided request data.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the request information.</param>
        /// <returns>A list of GettingStartedCompletedCondition objects representing the retrieved conditions.</returns>
        Task<List<GettingStartedCompletedCondition>> GetListGettingStartedCompletedCondition(ListGettingStartedCompletedConditionRequestDto requestDto);
    }
}
