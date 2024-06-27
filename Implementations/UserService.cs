
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
    public class UserService : IUserService
    {
        private readonly IDbConnection _dbConnection;

        public UserService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateUser(CreateUserRequestDTO request)
        {
            // Step 1: Validate all required fields
            if (request == null || !ValidateCreateUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Request Validation
            if (!await ValidateUniqueFields(request.Nickname, request.Email, request.Phone, request.IBM_UId))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Password Handling
            if (request.Password != request.ConfirmPassword)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 4: Create new User object
            var user = new User
            {
                Id = Guid.NewGuid(),
                Nickname = request.Nickname,
                Status = request.Status,
                Email = request.Email,
                ContactSettings = request.ContactSettings,
                UserRoles = request.UserRoles,
                SiteLanguage = request.SiteLanguage,
                LocaleSettings = request.LocaleSettings,
                Image = request.Image,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Company = request.Company,
                Phone = request.Phone,
                IBM_UId = request.IBM_UId,
                MaxNumApps = request.MaxNumApps,
                CompletedStories = request.CompletedStories,
                UserType = request.UserType,
                Questionnaire = request.Questionnaire,
                Created = DateTime.UtcNow,
                LastAccess = DateTime.UtcNow
            };

            // Step 5: In a single SQL transaction
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    const string sql = @"
                        INSERT INTO Users (Id, Nickname, Status, Email, ContactSettings, SiteLanguage, LocaleSettings, Image, FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, Questionnaire, Created, LastAccess)
                        VALUES (@Id, @Nickname, @Status, @Email, @ContactSettings, @SiteLanguage, @LocaleSettings, @Image, @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @Questionnaire, @Created, @LastAccess);
                    ";
                    await _dbConnection.ExecuteAsync(sql, user, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
            finally
            {
                _dbConnection.Close();
            }

            // Step 6: Return UserId from database
            return user.Id.ToString();
        }

        public async Task<User> GetUser(UserRequestDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                const string sql = "SELECT * FROM Users WHERE Id = @Id";
                var user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { request.Id });

                if (user == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                return user;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<string> UpdateUser(UpdateUserRequestDTO request)
        {
            // Step 1: Validate Necessary Parameters
            if (request == null || !ValidateUpdateUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing User
            var existingUser = await GetUser(new UserRequestDTO { Id = request.Id });
            if (existingUser == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Update User Object
            existingUser.Nickname = request.Nickname;
            existingUser.Status = request.Status;
            existingUser.Email = request.Email;
            existingUser.ContactSettings = request.ContactSettings;
            existingUser.UserRoles = request.UserRoles;
            existingUser.SiteLanguage = request.SiteLanguage;
            existingUser.LocaleSettings = request.LocaleSettings;
            existingUser.Image = request.Image;
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Company = request.Company;
            existingUser.Phone = request.Phone;
            existingUser.IBM_UId = request.IBM_UId;
            existingUser.MaxNumApps = request.MaxNumApps;
            existingUser.CompletedStories = request.CompletedStories;
            existingUser.UserType = request.UserType;
            existingUser.Questionnaire = request.Questionnaire;
            existingUser.LastAccess = DateTime.UtcNow;

            // Step 4: Save Changes to Database
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    const string sql = @"
                        UPDATE Users 
                        SET Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, Image = @Image, FirstName = @FirstName, LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, CompletedStories = @CompletedStories, UserType = @UserType, Questionnaire = @Questionnaire, LastAccess = @LastAccess
                        WHERE Id = @Id;
                    ";
                    await _dbConnection.ExecuteAsync(sql, existingUser, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
            finally
            {
                _dbConnection.Close();
            }

            // Step 5: Return UserId from database
            return existingUser.Id.ToString();
        }

        public async Task<bool> DeleteUser(DeleteUserRequestDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                const string sql = "DELETE FROM Users WHERE Id = @Id";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { request.Id });

                if (rowsAffected == 0)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }

                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        private bool ValidateCreateUserRequest(CreateUserRequestDTO request)
        {
            return request.Id != Guid.Empty &&
                   !string.IsNullOrEmpty(request.Nickname) &&
                   !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.SiteLanguage) &&
                   !string.IsNullOrEmpty(request.LocaleSettings) &&
                   !string.IsNullOrEmpty(request.FirstName) &&
                   !string.IsNullOrEmpty(request.LastName) &&
                   !string.IsNullOrEmpty(request.Phone) &&
                   !string.IsNullOrEmpty(request.IBM_UId) &&
                   request.UserType != Guid.Empty;
        }

        private bool ValidateUpdateUserRequest(UpdateUserRequestDTO request)
        {
            return request.Id != Guid.Empty &&
                   !string.IsNullOrEmpty(request.Nickname) &&
                   !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.SiteLanguage) &&
                   !string.IsNullOrEmpty(request.LocaleSettings) &&
                   !string.IsNullOrEmpty(request.FirstName) &&
                   !string.IsNullOrEmpty(request.LastName) &&
                   !string.IsNullOrEmpty(request.Phone) &&
                   !string.IsNullOrEmpty(request.IBM_UId) &&
                   request.UserType != Guid.Empty;
        }

        private async Task<bool> ValidateUniqueFields(string nickname, string email, string phone, string ibmUId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Nickname = @Nickname OR Email = @Email OR Phone = @Phone OR IBM_UId = @IBM_UId;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Nickname = nickname, Email = email, Phone = phone, IBM_UId = ibmUId });
            return count == 0;
        }
    }
}
