
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
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateUserQuestionnaire(CreateUserQuestionnaireDto createUserQuestionnaireDto);

        /// <summary>
        /// Updates an existing user questionnaire.
        /// </summary>
        /// <param name="updateUserQuestionnaireDto">The data transfer object containing the details for updating a user questionnaire.</param>
        /// <returns>A string representing the result of the update operation.</returns>
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
        /// <param name="listUserQuestionnaireRequestDto">The data transfer object containing the request details for listing user questionnaires.</param>
        /// <returns>A list of user questionnaires.</returns>
        Task<List<UserQuestionnaire>> GetUserQuestionnairesList(ListUserQuestionnaireRequestDto listUserQuestionnaireRequestDto);

        /// <summary>
        /// Retrieves a specific user questionnaire based on the provided request details.
        /// </summary>
        /// <param name="userQuestionnaireRequestDto">The data transfer object containing the request details for a specific user questionnaire.</param>
        /// <returns>A user questionnaire.</returns>
        Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDto userQuestionnaireRequestDto);
    }
}
