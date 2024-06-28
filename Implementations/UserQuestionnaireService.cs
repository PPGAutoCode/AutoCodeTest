
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class UserQuestionnaireService : IUserQuestionnaireService
    {
        private readonly IDbConnection _dbConnection;

        public UserQuestionnaireService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire(Guid userId)
        {
            // Step 1: Validate userId
            if (userId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user questionnaire from database
            var userQuestionnaire = await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(
                "SELECT * FROM UserQuestionnaires WHERE UserId = @UserId",
                new { UserId = userId });

            // Step 3: Handle user questionnaire not found
            if (userQuestionnaire == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return userQuestionnaire;
        }

        public async Task<string> UpdateUserQuestionnaire(UserQuestionnaire userQuestionnaire)
        {
            // Step 1: Validate user questionnaire
            if (userQuestionnaire == null || userQuestionnaire.UserId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch existing user questionnaire
            var existingUserQuestionnaire = await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(
                "SELECT * FROM UserQuestionnaires WHERE UserId = @UserId",
                new { UserId = userQuestionnaire.UserId });

            if (existingUserQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Update user questionnaire object
            existingUserQuestionnaire.Question1 = userQuestionnaire.Question1;
            existingUserQuestionnaire.Question2 = userQuestionnaire.Question2;
            existingUserQuestionnaire.Question3 = userQuestionnaire.Question3;
            existingUserQuestionnaire.Question4 = userQuestionnaire.Question4;
            existingUserQuestionnaire.Question5 = userQuestionnaire.Question5;
            existingUserQuestionnaire.LastUpdated = DateTime.UtcNow;

            // Step 4: Save changes to database
            try
            {
                await _dbConnection.ExecuteAsync(
                    "UPDATE UserQuestionnaires SET Question1 = @Question1, Question2 = @Question2, Question3 = @Question3, Question4 = @Question4, Question5 = @Question5, LastUpdated = @LastUpdated WHERE UserId = @UserId",
                    existingUserQuestionnaire);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingUserQuestionnaire.UserId.ToString();
        }
    }
}
