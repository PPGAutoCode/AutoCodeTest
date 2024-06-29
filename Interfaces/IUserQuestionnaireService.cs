
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing user questionnaires.
    /// </summary>
    public interface IUserQuestionnaireService
    {
        /// <summary>
        /// Creates a new user questionnaire.
        /// </summary>
        /// <param name="createUserQuestionnaireDto">The data transfer object containing the details for creating a user questionnaire.</param>
        /// <returns>A string representing the unique identifier of the created user questionnaire.</returns>
        Task<string> CreateUserQuestionnaire(CreateUserQuestionnaireDto createUserQuestionnaireDto);

        /// <summary>
        /// Updates an existing user questionnaire.
        /// </summary>
        /// <param name="updateUserQuestionnaireDto">The data transfer object containing the details for updating a user questionnaire.</param>
        /// <returns>A string representing the unique identifier of the updated user questionnaire.</returns>
        Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDto updateUserQuestionnaireDto);

        /// <summary>
        /// Deletes a user questionnaire.
        /// </summary>
        /// <param name="deleteUserQuestionnaireDto">The data transfer object containing the details for deleting a user questionnaire.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDto deleteUserQuestionnaireDto);

        /// <summary>
        /// Retrieves a list of user questionnaires based on the provided request details.
        /// </summary>
        /// <param name="userQuestionnaireRequestDto">The data transfer object containing the request details for retrieving user questionnaires.</param>
        /// <returns>A list of user questionnaires.</returns>
        Task<List<UserQuestionnaire>> GetListUserQuestionnaire(UserQuestionnaireRequestDto userQuestionnaireRequestDto);

        /// <summary>
        /// Retrieves a single user questionnaire based on the provided request details.
        /// </summary>
        /// <param name="userQuestionnaireRequestDto">The data transfer object containing the request details for retrieving a user questionnaire.</param>
        /// <returns>A single user questionnaire.</returns>
        Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDto userQuestionnaireRequestDto);
    }
}
