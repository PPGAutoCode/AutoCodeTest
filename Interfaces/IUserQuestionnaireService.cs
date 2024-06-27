
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
        /// <param name="createUserQuestionnaireDTO">Data transfer object for creating a user questionnaire.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateUserQuestionnaire(CreateUserQuestionnaireDTO createUserQuestionnaireDTO);

        /// <summary>
        /// Updates an existing user questionnaire.
        /// </summary>
        /// <param name="updateUserQuestionnaireDTO">Data transfer object for updating a user questionnaire.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDTO updateUserQuestionnaireDTO);

        /// <summary>
        /// Deletes a user questionnaire.
        /// </summary>
        /// <param name="deleteUserQuestionnaireDTO">Data transfer object for deleting a user questionnaire.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDTO deleteUserQuestionnaireDTO);

        /// <summary>
        /// Retrieves a list of user questionnaires based on the provided request.
        /// </summary>
        /// <param name="userQuestionnaireRequestDTO">Data transfer object for requesting user questionnaires.</param>
        /// <returns>A list of user questionnaires.</returns>
        Task<List<UserQuestionnaire>> GetListUserQuestionnaire(UserQuestionnaireRequestDTO userQuestionnaireRequestDTO);

        /// <summary>
        /// Retrieves a single user questionnaire based on the provided request.
        /// </summary>
        /// <param name="userQuestionnaireRequestDTO">Data transfer object for requesting a user questionnaire.</param>
        /// <returns>A single user questionnaire.</returns>
        Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDTO userQuestionnaireRequestDTO);
    }
}
