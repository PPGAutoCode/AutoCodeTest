
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

        public async Task<string> CreateUserQuestionnaire(CreateUserQuestionnaireDto request)
        {
            // Step 1: Validate Fields
            if (request == null || string.IsNullOrEmpty(request.Label) || string.IsNullOrEmpty(request.HelpText) ||
                string.IsNullOrEmpty(request.ReferenceMethod) || request.DefaultValue == null || !request.DefaultValue.Any())
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch ProductCategories
            foreach (var defaultValue in request.DefaultValue)
            {
                var defaultValueExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM DefaultValues WHERE Id = @Id", new { Id = defaultValue });
                if (!defaultValueExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 3: Fetch CorporateUser
            var corporateUserExists = await _dbConnection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM CorporateUsers WHERE Id = @Id", new { Id = request.CorporateUserId });
            if (!corporateUserExists)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Create UserQuestionnaire Object
            var userQuestionnaire = new UserQuestionnaire
            {
                Id = Guid.NewGuid(),
                Label = request.Label,
                HelpText = request.HelpText,
                ReferenceMethod = request.ReferenceMethod,
                DefaultValue = request.DefaultValue,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 5: Insert UserQuestionnaire into Database
            const string sql = @"
                INSERT INTO UserQuestionnaire (Id, Label, HelpText, ReferenceMethod, DefaultValue, Created, Changed)
                VALUES (@Id, @Label, @HelpText, @ReferenceMethod, @DefaultValue, @Created, @Changed)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, userQuestionnaire);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 6: Return the UserQuestionnaire.Id from the database
            return userQuestionnaire.Id.ToString();
        }

        // Implement other methods from IUserQuestionnaireService interface similarly
    }
}
