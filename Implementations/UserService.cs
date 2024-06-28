
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

        public async Task<string> CreateUser(CreateUserRequestDto request)
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
                Id = request.Id,
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
                UserQuestionnaire = request.UserQuestionnaire,
                Created = DateTime.UtcNow,
                LastAccess = DateTime.UtcNow
            };

            // Step 5: In a single SQL transaction
            try
            {
                _dbConnection.Open();
                using var transaction = _dbConnection.BeginTransaction();
                var sql = @"
                    INSERT INTO Users (Id, Nickname, Status, Email, ContactSettings, SiteLanguage, LocaleSettings, Image, FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, UserQuestionnaire, Created, LastAccess)
                    VALUES (@Id, @Nickname, @Status, @Email, @ContactSettings, @SiteLanguage, @LocaleSettings, @Image, @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @UserQuestionnaire, @Created, @LastAccess);
                ";
                await _dbConnection.ExecuteAsync(sql, user, transaction);
                transaction.Commit();
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

        public async Task<User> GetUser(UserRequestDto request)
        {
            if (request == null || !ValidateUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                var sql = @"
                    SELECT * FROM Users WHERE Id = @Id;
                ";
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

        public async Task<string> UpdateUser(UpdateUserRequestDto request)
        {
            if (request == null || !ValidateUpdateUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (!await ValidateUniqueFields(request.Nickname, request.Email, request.Phone, request.IBM_UId))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                var existingUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { request.Id });
                if (existingUser == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

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
                existingUser.UserQuestionnaire = request.UserQuestionnaire;
                existingUser.LastAccess = DateTime.UtcNow;

                using var transaction = _dbConnection.BeginTransaction();
                var sql = @"
                    UPDATE Users SET Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, Image = @Image, FirstName = @FirstName, LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, CompletedStories = @CompletedStories, UserType = @UserType, UserQuestionnaire = @UserQuestionnaire, LastAccess = @LastAccess WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, existingUser, transaction);
                transaction.Commit();
                return existingUser.Id.ToString();
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

        public async Task<bool> DeleteUser(DeleteUserRequestDto request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                var existingUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { request.Id });
                if (existingUser == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                using var transaction = _dbConnection.BeginTransaction();
                var sql = @"
                    DELETE FROM Users WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, new { request.Id }, transaction);
                transaction.Commit();
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

        public async Task<List<User>> GetListUser(ListUserRequestDto request)
        {
            if (request == null || !ValidateListUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            try
            {
                _dbConnection.Open();
                var sql = @"
                    SELECT * FROM Users
                    WHERE (@Id IS NULL OR Id = @Id)
                    AND (@Nickname IS NULL OR Nickname = @Nickname)
                    AND (@Email IS NULL OR Email = @Email)
                    AND (@Status IS NULL OR Status = @Status)
                    ORDER BY 
                        CASE WHEN @SortField = 'Nickname' AND @SortOrder = 'ASC' THEN Nickname END ASC,
                        CASE WHEN @SortField = 'Nickname' AND @SortOrder = 'DESC' THEN Nickname END DESC,
                        CASE WHEN @SortField = 'Email' AND @SortOrder = 'ASC' THEN Email END ASC,
                        CASE WHEN @SortField = 'Email' AND @SortOrder = 'DESC' THEN Email END DESC
                    OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
                ";
                var users = await _dbConnection.QueryAsync<User>(sql, new
                {
                    request.Id,
                    request.Nickname,
                    request.Email,
                    request.Status,
                    SortField = request.SortField,
                    SortOrder = request.SortOrder,
                    PageOffset = request.PageOffset,
                    PageLimit = request.PageLimit
                });
                return users.ToList();
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

        private bool ValidateCreateUserRequest(CreateUserRequestDto request)
        {
            return request.Id != Guid.Empty &&
                   !string.IsNullOrEmpty(request.Nickname) &&
                   request.Status != null &&
                   !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.SiteLanguage) &&
                   !string.IsNullOrEmpty(request.LocaleSettings) &&
                   !string.IsNullOrEmpty(request.FirstName) &&
                   !string.IsNullOrEmpty(request.LastName) &&
                   !string.IsNullOrEmpty(request.Phone) &&
                   request.UserType != Guid.Empty;
        }

        private bool ValidateUserRequest(UserRequestDto request)
        {
            return request.Id != Guid.Empty;
        }

        private bool ValidateUpdateUserRequest(UpdateUserRequestDto request)
        {
            return request.Id != Guid.Empty &&
                   !string.IsNullOrEmpty(request.Nickname) &&
                   request.Status != null &&
                   !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.SiteLanguage) &&
                   !string.IsNullOrEmpty(request.LocaleSettings) &&
                   !string.IsNullOrEmpty(request.FirstName) &&
                   !string.IsNullOrEmpty(request.LastName) &&
                   !string.IsNullOrEmpty(request.Phone) &&
                   request.UserType != Guid.Empty;
        }

        private bool ValidateListUserRequest(ListUserRequestDto request)
        {
            return request.PageLimit > 0 && request.PageOffset >= 0;
        }

        private async Task<bool> ValidateUniqueFields(string nickname, string email, string phone, string ibmUId)
        {
            var sql = @"
                SELECT COUNT(*) FROM Users
                WHERE Nickname = @Nickname OR Email = @Email OR Phone = @Phone OR IBM_UId = @IBM_UId;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Nickname = nickname, Email = email, Phone = phone, IBM_UId = ibmUId });
            return count == 0;
        }
    }
}
