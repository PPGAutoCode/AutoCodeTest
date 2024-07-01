
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
            if (request == null || !ValidateCreateUserQuestionnaireDto(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Process each item in the ProductCategories list
            foreach (var categoryId in request.ProductCategoriesId)
            {
                var category = await _dbConnection.QueryFirstOrDefaultAsync<ProductCategory>("SELECT * FROM ProductCategories WHERE Id = @Id", new { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
            }

            // Step 3: Create UserQuestionnaire Object
            var userQuestionnaire = new UserQuestionnaire
            {
                Id = Guid.NewGuid(),
                CompanyErpSolutionName = request.CompanyErpSolutionName,
                CompanyErpSolutionVersion = request.CompanyErpSolutionVersion,
                CompanyKad = request.CompanyKad,
                CompanyLegalName = request.CompanyLegalName,
                CompanyOwnsBankAccount = request.CompanyOwnsBankAccount,
                CompanyReprEmail = request.CompanyReprEmail,
                CompanyRepFullName = request.CompanyRepFullName,
                CompanyRepNumber = request.CompanyRepNumber,
                CompanyTaxId = request.CompanyTaxId,
                CompanyUsesErp = request.CompanyUsesErp,
                CompanyWebsite = request.CompanyWebsite,
                CorporateUserId = request.CorporateUserId,
                ErpBankingActivities = request.ErpBankingActivities,
                ProductCategoriesId = request.ProductCategoriesId
            };

            // Step 4: Create new lists of related objects
            var productProductCategories = request.ProductCategoriesId.Select(categoryId => new ProductProductCategory
            {
                Id = Guid.NewGuid(),
                ProductId = userQuestionnaire.Id,
                ProductCategoryId = categoryId
            }).ToList();

            // Step 5: Insert UserQuestionnaire into Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO UserQuestionnaire (Id, CompanyErpSolutionName, CompanyErpSolutionVersion, CompanyKad, CompanyLegalName, CompanyOwnsBankAccount, CompanyReprEmail, CompanyRepFullName, CompanyRepNumber, CompanyTaxId, CompanyUsesErp, CompanyWebsite, CorporateUserId, ErpBankingActivities) VALUES (@Id, @CompanyErpSolutionName, @CompanyErpSolutionVersion, @CompanyKad, @CompanyLegalName, @CompanyOwnsBankAccount, @CompanyReprEmail, @CompanyRepFullName, @CompanyRepNumber, @CompanyTaxId, @CompanyUsesErp, @CompanyWebsite, @CorporateUserId, @ErpBankingActivities)", userQuestionnaire, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ProductProductCategories (Id, ProductId, ProductCategoryId) VALUES (@Id, @ProductId, @ProductCategoryId)", productProductCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return the UserQuestionnaire.Id from the database
            return userQuestionnaire.Id.ToString();
        }

        public async Task<List<UserQuestionnaire>> GetUserQuestionnairesList(ListUserQuestionnaireRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary pagination parameters
            if (request == null || request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of UserQuestionnaires from the database based on the provided pagination parameters and optional sorting
            var query = "SELECT * FROM UserQuestionnaire";
            if (request.PageLimit > 0 && request.PageOffset >= 0)
            {
                query += " ORDER BY Id OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            }

            var userQuestionnaires = await _dbConnection.QueryAsync<UserQuestionnaire>(query, new { PageLimit = request.PageLimit, PageOffset = request.PageOffset });

            // Step 3: If PageLimit and PageOffset are both equal to 0, return an empty array
            if (request.PageLimit == 0 && request.PageOffset == 0)
            {
                return new List<UserQuestionnaire>();
            }

            // Step 4: Return the list of UserQuestionnaires
            return userQuestionnaires.ToList();
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch UserQuestionnaire from database by id
            var userQuestionnaire = await _dbConnection.QueryFirstOrDefaultAsync<UserQuestionnaire>("SELECT * FROM UserQuestionnaire WHERE Id = @Id", new { Id = request.Id });

            // Step 3: If the UserQuestionnaire exists, return it
            if (userQuestionnaire != null)
            {
                return userQuestionnaire;
            }

            // Step 4: If the UserQuestionnaire does not exist, throw an exception
            throw new BusinessException("DP-404", "Technical Error");
        }

        public async Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request == null || !ValidateUpdateUserQuestionnaireDto(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing UserQuestionnaire
            var existingUserQuestionnaire = await _dbConnection.QueryFirstOrDefaultAsync<UserQuestionnaire>("SELECT * FROM UserQuestionnaire WHERE Id = @Id", new { Id = request.Id });
            if (existingUserQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Validate Related Entities
            foreach (var categoryId in request.ProductCategoriesId)
            {
                var category = await _dbConnection.QueryFirstOrDefaultAsync<ProductCategory>("SELECT * FROM ProductCategories WHERE Id = @Id", new { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Update UserQuestionnaire Object
            existingUserQuestionnaire.CompanyErpSolutionName = request.CompanyErpSolutionName;
            existingUserQuestionnaire.CompanyErpSolutionVersion = request.CompanyErpSolutionVersion;
            existingUserQuestionnaire.CompanyKad = request.CompanyKad;
            existingUserQuestionnaire.CompanyLegalName = request.CompanyLegalName;
            existingUserQuestionnaire.CompanyOwnsBankAccount = request.CompanyOwnsBankAccount;
            existingUserQuestionnaire.CompanyReprEmail = request.CompanyReprEmail;
            existingUserQuestionnaire.CompanyRepFullName = request.CompanyRepFullName;
            existingUserQuestionnaire.CompanyRepNumber = request.CompanyRepNumber;
            existingUserQuestionnaire.CompanyTaxId = request.CompanyTaxId;
            existingUserQuestionnaire.CompanyUsesErp = request.CompanyUsesErp;
            existingUserQuestionnaire.CompanyWebsite = request.CompanyWebsite;
            existingUserQuestionnaire.CorporateUserId = request.CorporateUserId;
            existingUserQuestionnaire.ErpBankingActivities = request.ErpBankingActivities;
            existingUserQuestionnaire.ProductCategoriesId = request.ProductCategoriesId;

            // Step 5: Update Associated Lists
            var productProductCategories = request.ProductCategoriesId.Select(categoryId => new ProductProductCategory
            {
                Id = Guid.NewGuid(),
                ProductId = existingUserQuestionnaire.Id,
                ProductCategoryId = categoryId
            }).ToList();

            // Step 6: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE UserQuestionnaire SET CompanyErpSolutionName = @CompanyErpSolutionName, CompanyErpSolutionVersion = @CompanyErpSolutionVersion, CompanyKad = @CompanyKad, CompanyLegalName = @CompanyLegalName, CompanyOwnsBankAccount = @CompanyOwnsBankAccount, CompanyReprEmail = @CompanyReprEmail, CompanyRepFullName = @CompanyRepFullName, CompanyRepNumber = @CompanyRepNumber, CompanyTaxId = @CompanyTaxId, CompanyUsesErp = @CompanyUsesErp, CompanyWebsite = @CompanyWebsite, CorporateUserId = @CorporateUserId, ErpBankingActivities = @ErpBankingActivities WHERE Id = @Id", existingUserQuestionnaire, transaction);

                    await _dbConnection.ExecuteAsync("DELETE FROM ProductProductCategories WHERE ProductId = @ProductId", new { ProductId = existingUserQuestionnaire.Id }, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ProductProductCategories (Id, ProductId, ProductCategoryId) VALUES (@Id, @ProductId, @ProductCategoryId)", productProductCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 7: Return the UserQuestionnaire.Id
            return existingUserQuestionnaire.Id.ToString();
        }

        public async Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDto request)
        {
            // Step 1: Validate that the request.payload.id contains the necessary parameter
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Get the existing UserQuestionnaire object from the database based on the provided UserQuestionnaire ID
            var existingUserQuestionnaire = await _dbConnection.QueryFirstOrDefaultAsync<UserQuestionnaire>("SELECT * FROM UserQuestionnaire WHERE Id = @Id", new { Id = request.Id });
            if (existingUserQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Delete the UserQuestionnaire object from the database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM ProductProductCategories WHERE ProductId = @ProductId", new { ProductId = existingUserQuestionnaire.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM UserQuestionnaire WHERE Id = @Id", new { Id = existingUserQuestionnaire.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 4: Return true if the transaction is successful
            return true;
        }

        private bool ValidateCreateUserQuestionnaireDto(CreateUserQuestionnaireDto request)
        {
            return !string.IsNullOrEmpty(request.CompanyErpSolutionName) &&
                   !string.IsNullOrEmpty(request.CompanyErpSolutionVersion) &&
                   !string.IsNullOrEmpty(request.CompanyKad) &&
                   !string.IsNullOrEmpty(request.CompanyLegalName) &&
                   !string.IsNullOrEmpty(request.CompanyReprEmail) &&
                   !string.IsNullOrEmpty(request.CompanyRepFullName) &&
                   !string.IsNullOrEmpty(request.CompanyRepNumber) &&
                   !string.IsNullOrEmpty(request.CompanyTaxId) &&
                   !string.IsNullOrEmpty(request.CompanyWebsite) &&
                   request.CorporateUserId != Guid.Empty &&
                   !string.IsNullOrEmpty(request.ErpBankingActivities) &&
                   request.ProductCategoriesId != null &&
                   request.ProductCategoriesId.All(id => id != Guid.Empty);
        }

        private bool ValidateUpdateUserQuestionnaireDto(UpdateUserQuestionnaireDto request)
        {
            return !string.IsNullOrEmpty(request.CompanyErpSolutionName) &&
                   !string.IsNullOrEmpty(request.CompanyErpSolutionVersion) &&
                   !string.IsNullOrEmpty(request.CompanyKad) &&
                   !string.IsNullOrEmpty(request.CompanyLegalName) &&
                   !string.IsNullOrEmpty(request.CompanyReprEmail) &&
                   !string.IsNullOrEmpty(request.CompanyRepFullName) &&
                   !string.IsNullOrEmpty(request.CompanyRepNumber) &&
                   !string.IsNullOrEmpty(request.CompanyTaxId) &&
                   !string.IsNullOrEmpty(request.CompanyWebsite) &&
                   request.CorporateUserId != Guid.Empty &&
                   !string.IsNullOrEmpty(request.ErpBankingActivities) &&
                   request.ProductCategoriesId != null &&
                   request.ProductCategoriesId.All(id => id != Guid.Empty);
        }
    }
}
