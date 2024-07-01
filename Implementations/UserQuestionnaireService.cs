
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
            ValidateCreateUserQuestionnaireRequest(request);

            var productCategories = await FetchProductCategories(request.ProductCategoriesId);
            if (productCategories.Count != request.ProductCategoriesId.Count)
            {
                throw new BusinessException("DP-404", "One or more ProductCategories do not exist.");
            }

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

            var productProductCategories = request.ProductCategoriesId.Select(pcId => new ProductProductCategory
            {
                Id = Guid.NewGuid(),
                ProductId = userQuestionnaire.Id,
                ProductCategoryId = pcId
            }).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string insertUserQuestionnaireQuery = @"
                        INSERT INTO UserQuestionnaire (Id, CompanyErpSolutionName, CompanyErpSolutionVersion, CompanyKad, CompanyLegalName, CompanyOwnsBankAccount, CompanyReprEmail, CompanyRepFullName, CompanyRepNumber, CompanyTaxId, CompanyUsesErp, CompanyWebsite, CorporateUserId, ErpBankingActivities, Created, Changed)
                        VALUES (@Id, @CompanyErpSolutionName, @CompanyErpSolutionVersion, @CompanyKad, @CompanyLegalName, @CompanyOwnsBankAccount, @CompanyReprEmail, @CompanyRepFullName, @CompanyRepNumber, @CompanyTaxId, @CompanyUsesErp, @CompanyWebsite, @CorporateUserId, @ErpBankingActivities, @Created, @Changed);
                    ";
                    await _dbConnection.ExecuteAsync(insertUserQuestionnaireQuery, userQuestionnaire, transaction);

                    const string insertProductProductCategoriesQuery = @"
                        INSERT INTO ProductProductCategories (Id, ProductId, ProductCategoryId)
                        VALUES (@Id, @ProductId, @ProductCategoryId);
                    ";
                    await _dbConnection.ExecuteAsync(insertProductProductCategoriesQuery, productProductCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            return userQuestionnaire.Id.ToString();
        }

        public async Task<List<UserQuestionnaire>> GetListUserQuestionnaire(ListUserQuestionnaireRequestDto request)
        {
            ValidateListUserQuestionnaireRequest(request);

            const string query = @"
                SELECT * FROM UserQuestionnaire
                ORDER BY {0} {1}
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            var sortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "ASC" : request.SortOrder;

            var userQuestionnaires = await _dbConnection.QueryAsync<UserQuestionnaire>(string.Format(query, sortField, sortOrder), new { request.PageOffset, request.PageLimit });

            return userQuestionnaires.ToList();
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire(UserQuestionnaireRequestDto request)
        {
            ValidateUserQuestionnaireRequest(request);

            const string query = @"
                SELECT * FROM UserQuestionnaire WHERE Id = @Id;
            ";

            var userQuestionnaire = await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(query, new { request.Id });

            if (userQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "UserQuestionnaire not found.");
            }

            return userQuestionnaire;
        }

        public async Task<string> UpdateUserQuestionnaire(UpdateUserQuestionnaireDto request)
        {
            ValidateUpdateUserQuestionnaireRequest(request);

            var existingUserQuestionnaire = await FetchUserQuestionnaireById(request.Id);
            if (existingUserQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "UserQuestionnaire not found.");
            }

            var productCategories = await FetchProductCategories(request.ProductCategoriesId);
            if (productCategories.Count != request.ProductCategoriesId.Count)
            {
                throw new BusinessException("DP-422", "One or more ProductCategories do not exist.");
            }

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

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string updateUserQuestionnaireQuery = @"
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
                        WHERE Id = @Id;
                    ";
                    await _dbConnection.ExecuteAsync(updateUserQuestionnaireQuery, existingUserQuestionnaire, transaction);

                    const string deleteProductProductCategoriesQuery = @"
                        DELETE FROM ProductProductCategories WHERE ProductId = @ProductId;
                    ";
                    await _dbConnection.ExecuteAsync(deleteProductProductCategoriesQuery, new { ProductId = existingUserQuestionnaire.Id }, transaction);

                    var productProductCategories = request.ProductCategoriesId.Select(pcId => new ProductProductCategory
                    {
                        Id = Guid.NewGuid(),
                        ProductId = existingUserQuestionnaire.Id,
                        ProductCategoryId = pcId
                    }).ToList();

                    const string insertProductProductCategoriesQuery = @"
                        INSERT INTO ProductProductCategories (Id, ProductId, ProductCategoryId)
                        VALUES (@Id, @ProductId, @ProductCategoryId);
                    ";
                    await _dbConnection.ExecuteAsync(insertProductProductCategoriesQuery, productProductCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            return existingUserQuestionnaire.Id.ToString();
        }

        public async Task<bool> DeleteUserQuestionnaire(DeleteUserQuestionnaireDto request)
        {
            ValidateDeleteUserQuestionnaireRequest(request);

            var existingUserQuestionnaire = await FetchUserQuestionnaireById(request.Id);
            if (existingUserQuestionnaire == null)
            {
                throw new BusinessException("DP-404", "UserQuestionnaire not found.");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string deleteProductProductCategoriesQuery = @"
                        DELETE FROM ProductProductCategories WHERE ProductId = @ProductId;
                    ";
                    await _dbConnection.ExecuteAsync(deleteProductProductCategoriesQuery, new { ProductId = existingUserQuestionnaire.Id }, transaction);

                    const string deleteUserQuestionnaireQuery = @"
                        DELETE FROM UserQuestionnaire WHERE Id = @Id;
                    ";
                    await _dbConnection.ExecuteAsync(deleteUserQuestionnaireQuery, new { existingUserQuestionnaire.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            return true;
        }

        private void ValidateCreateUserQuestionnaireRequest(CreateUserQuestionnaireDto request)
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
                request.ProductCategoriesId == null ||
                !request.ProductCategoriesId.Any())
            {
                throw new BusinessException("DP-422", "One or more mandatory fields are missing or invalid.");
            }
        }

        private void ValidateListUserQuestionnaireRequest(ListUserQuestionnaireRequestDto request)
        {
            if (request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "PageLimit and PageOffset must be non-negative integers.");
            }
        }

        private void ValidateUserQuestionnaireRequest(UserQuestionnaireRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "UserQuestionnaire ID is required.");
            }
        }

        private void ValidateUpdateUserQuestionnaireRequest(UpdateUserQuestionnaireDto request)
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
                request.ProductCategoriesId == null ||
                !request.ProductCategoriesId.Any())
            {
                throw new BusinessException("DP-422", "One or more mandatory fields are missing or invalid.");
            }
        }

        private void ValidateDeleteUserQuestionnaireRequest(DeleteUserQuestionnaireDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "UserQuestionnaire ID is required.");
            }
        }

        private async Task<List<ProductCategory>> FetchProductCategories(List<Guid> productCategoryIds)
        {
            const string query = @"
                SELECT * FROM ProductCategory WHERE Id IN @Ids;
            ";

            return (await _dbConnection.QueryAsync<ProductCategory>(query, new { Ids = productCategoryIds })).ToList();
        }

        private async Task<UserQuestionnaire> FetchUserQuestionnaireById(Guid id)
        {
            const string query = @"
                SELECT * FROM UserQuestionnaire WHERE Id = @Id;
            ";

            return await _dbConnection.QuerySingleOrDefaultAsync<UserQuestionnaire>(query, new { Id = id });
        }
    }
}
