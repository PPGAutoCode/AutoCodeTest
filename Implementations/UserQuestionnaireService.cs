
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

        public async Task<string> CreateUserQuestionnaire(CreateUserQuestionnaireDTO request)
        {
            // Step 1: Validate Fields
            if (string.IsNullOrEmpty(request.Label) || string.IsNullOrEmpty(request.HelpText) || string.IsNullOrEmpty(request.ReferenceMethod) || request.DefaultValue == null || !request.DefaultValue.Any())
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch ProductCategories
            foreach (var id in request.DefaultValue)
            {
                var exists = await _dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM ProductCategories WHERE Id = @Id", new { Id = id });
                if (!exists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 3: Fetch CorporateUser
            var corporateUserExists = await _dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM CorporateUser WHERE Id = @Id", new { Id = request.CorporateUserId });
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

            return userQuestionnaire.Id.ToString();
        }

        public async Task<List<UserQuestionnaire>> GetListUserQuestionnaire(ListUserQuestionnaireRequestDTO request)
        {
            // Step 1: Validate Pagination Parameters
            if (request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch UserQuestionnaires from Database
            var sql = @"
                SELECT * FROM UserQuestionnaire
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var userQuestionnaires = await _dbConnection.QueryAsync<UserQuestionnaire>(sql, new
                {
                    SortField = request.SortField,
                    SortOrder = request.SortOrder,
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit
                });

                return userQuestionnaires.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDTO request)
        {
            // Step 1: Validate Request Payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch UserQuestionnaire from Database
            var sql = "SELECT * FROM UserQuestionnaire WHERE Id = @Id";

            try
            {
                var userQuestionnaire = await _dbConnection.QueryFirstOrDefaultAsync<UserQuestionnaire>(sql, new { Id = request.Id });
                if (userQuestionnaire == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                return userQuestionnaire;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDTO request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Label) || string.IsNullOrEmpty(request.HelpText) || string.IsNullOrEmpty(request.ReferenceMethod) || request.DefaultValue == null || !request.DefaultValue.Any())
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing UserQuestionnaire
            var existingUserQuestionnaire = await GetUserQuestionnaire(new UserQuestionnaireRequestDTO { Id = request.Id });
            if (existingUserQuestionnaire == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update UserQuestionnaire Object
            existingUserQuestionnaire.Label = request.Label;
            existingUserQuestionnaire.HelpText = request.HelpText;
            existingUserQuestionnaire.ReferenceMethod = request.ReferenceMethod;
            existingUserQuestionnaire.DefaultValue = request.DefaultValue;
            existingUserQuestionnaire.Changed = DateTime.UtcNow;

            // Step 4: Save Changes to Database
            const string sql = @"
                UPDATE UserQuestionnaire
                SET Label = @Label, HelpText = @HelpText, ReferenceMethod = @ReferenceMethod, DefaultValue = @DefaultValue, Changed = @Changed
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, existingUserQuestionnaire);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingUserQuestionnaire.Id.ToString();
        }

        public async Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDTO request)
        {
            // Step 1: Validate Request Payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing UserQuestionnaire
            var existingUserQuestionnaire = await GetUserQuestionnaire(new UserQuestionnaireRequestDTO { Id = request.Id });
            if (existingUserQuestionnaire == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete UserQuestionnaire from Database
            const string sql = "DELETE FROM UserQuestionnaire WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }
    }
}
