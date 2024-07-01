
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
            ValidateCreateUserQuestionnaire(request);

            var productCategories = await FetchProductCategories(request.ProductCategoriesId);
            var corporateUser = await FetchCorporateUser(request.CorporateUserId);

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
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            var productProductCategories = productCategories.Select(pc => new ProductProductCategory
            {
                Id = Guid.NewGuid(),
                ProductId = userQuestionnaire.Id,
                ProductCategoryId = pc.Id
            }).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string insertUserQuestionnaireQuery = @"
                        INSERT INTO UserQuestionnaire (Id, CompanyErpSolutionName, CompanyErpSolutionVersion, CompanyKad, CompanyLegalName, CompanyOwnsBankAccount, CompanyReprEmail, CompanyRepFullName, CompanyRepNumber, CompanyTaxId, CompanyUsesErp, CompanyWebsite, CorporateUserId, ErpBankingActivities, Created, Changed)
                        VALUES (@Id, @CompanyErpSolutionName, @CompanyErpSolutionVersion, @CompanyKad, @CompanyLegalName, @CompanyOwnsBankAccount, @CompanyReprEmail, @CompanyRepFullName, @CompanyRepNumber, @CompanyTaxId, @CompanyUsesErp, @CompanyWebsite, @CorporateUserId, @ErpBankingActivities, @Created, @Changed)";
                    await _dbConnection.ExecuteAsync(insertUserQuestionnaireQuery, userQuestionnaire, transaction);

                    const string insertProductProductCategoriesQuery = @"
                        INSERT INTO ProductProductCategories (Id, ProductId, ProductCategoryId)
                        VALUES (@Id, @ProductId, @ProductCategoryId)";
                    await _dbConnection.ExecuteAsync(insertProductProductCategoriesQuery, productProductCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return userQuestionnaire.Id.ToString();
        }

        public async Task<List<UserQuestionnaire>> GetListUserQuestionnaire(ListUserQuestionnaireRequestDto request)
        {
            ValidateListUserQuestionnaire(request);

            const string query = @"
                SELECT * FROM UserQuestionnaire
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var parameters = new
            {
                PageOffset = request.PageOffset,
                PageLimit = request.PageLimit,
                SortField = request.SortField,
                SortOrder = request.SortOrder
            };

            var userQuestionnaires = await _dbConnection.QueryAsync<UserQuestionnaire>(query, parameters);

            if (request.PageLimit == 0 && request.PageOffset == 0)
            {
                return new List<UserQuestionnaire>();
            }

            return userQuestionnaires.ToList();
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDto request)
        {
            ValidateUserQuestionnaireRequest(request);

            const string query = @"
                SELECT * FROM UserQuestionnaire WHERE Id = @Id";

            var userQuestionnaire = await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(query, new { Id = request.Id });

            if (userQuestionnaire == null)
            {
                throw new TechnicalException("DP-404", "UserQuestionnaire not found");
            }

            return userQuestionnaire;
        }

        public async Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDto request)
        {
            ValidateUpdateUserQuestionnaire(request);

            var existingUserQuestionnaire = await FetchUserQuestionnaire(request.Id);

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
            existingUserQuestionnaire.Changed = DateTime.UtcNow;

            const string updateQuery = @"
                UPDATE UserQuestionnaire
                SET CompanyErpSolutionName = @CompanyErpSolutionName,
                    CompanyErpSolutionVersion = @CompanyErpSolutionVersion,
                    CompanyKad = @CompanyKad,
                    CompanyLegalName = @CompanyLegalName,
                    CompanyOwnsBankAccount = @CompanyOwnsBankAccount,
                    CompanyReprEmail = @CompanyReprEmail,
                    CompanyRepFullName = @CompanyRepFullName,
                    CompanyRepNumber = @CompanyRepNumber,
                    CompanyTaxId = @CompanyTaxId,
                    CompanyUsesErp = @CompanyUsesErp,
                    CompanyWebsite = @CompanyWebsite,
                    CorporateUserId = @CorporateUserId,
                    ErpBankingActivities = @ErpBankingActivities,
                    Changed = @Changed
                WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(updateQuery, existingUserQuestionnaire);

            return existingUserQuestionnaire.Id.ToString();
        }

        public async Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDto request)
        {
            ValidateDeleteUserQuestionnaire(request);

            var existingUserQuestionnaire = await FetchUserQuestionnaire(request.Id);

            const string deleteQuery = @"
                DELETE FROM UserQuestionnaire WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(deleteQuery, new { Id = request.Id });

            return true;
        }

        private void ValidateCreateUserQuestionnaire(CreateUserQuestionnaireDto request)
        {
            if (string.IsNullOrEmpty(request.CompanyErpSolutionName) ||
                string.IsNullOrEmpty(request.CompanyErpSolutionVersion) ||
                string.IsNullOrEmpty(request.CompanyKad) ||
                string.IsNullOrEmpty(request.CompanyLegalName) ||
                string.IsNullOrEmpty(request.CompanyReprEmail) ||
                string.IsNullOrEmpty(request.CompanyRepFullName) ||
                string.IsNullOrEmpty(request.CompanyRepNumber) ||
                string.IsNullOrEmpty(request.CompanyTaxId) ||
                string.IsNullOrEmpty(request.CompanyWebsite) ||
                request.CorporateUserId == Guid.Empty ||
                request.ProductCategoriesId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private void ValidateListUserQuestionnaire(ListUserQuestionnaireRequestDto request)
        {
            if (request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private void ValidateUserQuestionnaireRequest(UserQuestionnaireRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private void ValidateUpdateUserQuestionnaire(UpdateUserQuestionnaireDto request)
        {
            if (string.IsNullOrEmpty(request.CompanyErpSolutionName) ||
                string.IsNullOrEmpty(request.CompanyErpSolutionVersion) ||
                string.IsNullOrEmpty(request.CompanyKad) ||
                string.IsNullOrEmpty(request.CompanyLegalName) ||
                string.IsNullOrEmpty(request.CompanyReprEmail) ||
                string.IsNullOrEmpty(request.CompanyRepFullName) ||
                string.IsNullOrEmpty(request.CompanyRepNumber) ||
                string.IsNullOrEmpty(request.CompanyTaxId) ||
                string.IsNullOrEmpty(request.CompanyWebsite) ||
                request.CorporateUserId == Guid.Empty ||
                request.ProductCategoriesId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private void ValidateDeleteUserQuestionnaire(DeleteUserQuestionnaireDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private async Task<List<ProductCategory>> FetchProductCategories(Guid productCategoriesId)
        {
            const string query = @"
                SELECT * FROM ProductCategories WHERE Id = @Id";

            var productCategories = await _dbConnection.QueryAsync<ProductCategory>(query, new { Id = productCategoriesId });

            if (!productCategories.Any())
            {
                throw new TechnicalException("DP-404", "ProductCategory not found");
            }

            return productCategories.ToList();
        }

        private async Task<UserQuestionnaire> FetchUserQuestionnaire(Guid id)
        {
            const string query = @"
                SELECT * FROM UserQuestionnaire WHERE Id = @Id";

            var userQuestionnaire = await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(query, new { Id = id });

            if (userQuestionnaire == null)
            {
                throw new TechnicalException("DP-404", "UserQuestionnaire not found");
            }

            return userQuestionnaire;
        }

        private async Task<CorporateUser> FetchCorporateUser(Guid corporateUserId)
        {
            const string query = @"
                SELECT * FROM CorporateUser WHERE Id = @Id";

            var corporateUser = await _dbConnection.QuerySingleOrDefaultAsync<CorporateUser>(query, new { Id = corporateUserId });

            if (corporateUser == null)
            {
                throw new TechnicalException("DP-404", "CorporateUser not found");
            }

            return corporateUser;
        }
    }
}
