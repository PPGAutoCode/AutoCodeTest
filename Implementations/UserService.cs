
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
            ValidateCreateUserRequest(request);

            // Step 2: Request Validation
            await ValidateUserUniqueness(request.Nickname, request.Email, request.Phone, request.IBM_UId);

            // Step 3: Password Handling
            if (request.Password != request.ConfirmPassword)
            {
                throw new BusinessException("DP-422", "Password and Confirm Password do not match.");
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
                UserQuestionnaire = request.UserQuestionnaire,
                Created = DateTime.UtcNow,
                LastAccess = DateTime.UtcNow
            };

            // Step 5: In a single SQL transaction
            try
            {
                const string sql = @"
                    INSERT INTO Users (Id, Nickname, Status, Email, ContactSettings, SiteLanguage, LocaleSettings, Image, FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, UserQuestionnaire, Created, LastAccess)
                    VALUES (@Id, @Nickname, @Status, @Email, @ContactSettings, @SiteLanguage, @LocaleSettings, @Image, @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @UserQuestionnaire, @Created, @LastAccess);
                ";
                await _dbConnection.ExecuteAsync(sql, user);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
            }

            // Step 6: Return UserId from database
            return user.Id.ToString();
        }

        public async Task<User> GetUser(UserRequestDto request)
        {
            // Step 1: Validate UserRequestDto fields
            if (request.Id == Guid.Empty && string.IsNullOrEmpty(request.Nickname) && string.IsNullOrEmpty(request.Email))
            {
                throw new BusinessException("DP-422", "At least one field must be provided.");
            }

            // Step 2: Fetch user from database by the requested field
            var user = await FetchUserFromDatabase(request);

            // Step 3: If the user exists, return the User
            if (user != null)
            {
                return user;
            }

            // Step 4: If the user does not exist, throw an exception
            throw new TechnicalException("DP-404", "User not found.");
        }

        public async Task<string> UpdateUser(UpdateUserRequestDto request)
        {
            // Step 1: Validate the incoming request
            ValidateUpdateUserRequest(request);

            // Step 2: Fetch the existing User object from the database
            var existingUser = await FetchUserFromDatabase(new UserRequestDto { Id = request.Id });
            if (existingUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 3: Update the User object with the provided changes
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

            // Step 4: Save the updated User object back to the database
            try
            {
                const string sql = @"
                    UPDATE Users 
                    SET Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, Image = @Image, FirstName = @FirstName, LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, CompletedStories = @CompletedStories, UserType = @UserType, UserQuestionnaire = @UserQuestionnaire, LastAccess = @LastAccess
                    WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, existingUser);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
            }

            // Step 5: Return the updated User ID
            return existingUser.Id.ToString();
        }

        public async Task<bool> DeleteUser(DeleteUserRequestDto request)
        {
            // Step 1: Validate that the request contains the necessary parameter
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "User ID is required.");
            }

            // Step 2: Get the existing User object from the database
            var existingUser = await FetchUserFromDatabase(new UserRequestDto { Id = request.Id });
            if (existingUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 3: Delete the User object from the database
            try
            {
                const string sql = @"
                    DELETE FROM Users 
                    WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
            }

            // Step 4: Return true if the transaction is successful
            return true;
        }

        public async Task<List<User>> GetListUsers(ListUserRequestDto request)
        {
            // Step 1: Validate the incoming request
            ValidateListUserRequest(request);

            // Step 2: Fetching User Data
            var users = await FetchUsersFromDatabase(request);

            // Step 3: Return the list of Users
            return users;
        }

        private void ValidateCreateUserRequest(CreateUserRequestDto request)
        {
            if (request == null)
            {
                throw new BusinessException("DP-422", "Request payload is null.");
            }

            if (string.IsNullOrEmpty(request.Nickname) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.SiteLanguage) || string.IsNullOrEmpty(request.LocaleSettings) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) || string.IsNullOrEmpty(request.Phone))
            {
                throw new BusinessException("DP-422", "All required fields must be provided.");
            }
        }

        private void ValidateUpdateUserRequest(UpdateUserRequestDto request)
        {
            if (request == null)
            {
                throw new BusinessException("DP-422", "Request payload is null.");
            }

            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Nickname) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.SiteLanguage) || string.IsNullOrEmpty(request.LocaleSettings) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) || string.IsNullOrEmpty(request.Phone))
            {
                throw new BusinessException("DP-422", "All required fields must be provided.");
            }
        }

        private void ValidateListUserRequest(ListUserRequestDto request)
        {
            if (request == null)
            {
                throw new BusinessException("DP-422", "Request payload is null.");
            }

            if (request.PageLimit < 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "PageLimit and PageOffset must be positive integers.");
            }
        }

        private async Task ValidateUserUniqueness(string nickname, string email, string phone, string ibmUId)
        {
            if (await IsUserFieldUnique("Nickname", nickname) == false)
            {
                throw new BusinessException("DP-422", "Nickname already exists.");
            }

            if (await IsUserFieldUnique("Email", email) == false)
            {
                throw new BusinessException("DP-422", "Email already exists.");
            }

            if (await IsUserFieldUnique("Phone", phone) == false)
            {
                throw new BusinessException("DP-422", "Phone already exists.");
            }

            if (await IsUserFieldUnique("IBM_UId", ibmUId) == false)
            {
                throw new BusinessException("DP-422", "IBM UId already exists.");
            }
        }

        private async Task<bool> IsUserFieldUnique(string field, string value)
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM Users 
                WHERE {0} = @Value;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(string.Format(sql, field), new { Value = value });
            return count == 0;
        }

        private async Task<User> FetchUserFromDatabase(UserRequestDto request)
        {
            var sql = @"
                SELECT * 
                FROM Users 
                WHERE 1=1
            ";

            var parameters = new DynamicParameters();

            if (request.Id != Guid.Empty)
            {
                sql += " AND Id = @Id";
                parameters.Add("Id", request.Id);
            }

            if (!string.IsNullOrEmpty(request.Nickname))
            {
                sql += " AND Nickname = @Nickname";
                parameters.Add("Nickname", request.Nickname);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                sql += " AND Email = @Email";
                parameters.Add("Email", request.Email);
            }

            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }

        private async Task<List<User>> FetchUsersFromDatabase(ListUserRequestDto request)
        {
            var sql = @"
                SELECT * 
                FROM Users 
                WHERE 1=1
            ";

            var parameters = new DynamicParameters();

            if (request.Id != Guid.Empty)
            {
                sql += " AND Id = @Id";
                parameters.Add("Id", request.Id);
            }

            if (!string.IsNullOrEmpty(request.Nickname))
            {
                sql += " AND Nickname = @Nickname";
                parameters.Add("Nickname", request.Nickname);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                sql += " AND Email = @Email";
                parameters.Add("Email", request.Email);
            }

            if (request.Status.HasValue)
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", request.Status);
            }

            if (request.UserRoles != null && request.UserRoles.Any())
            {
                sql += " AND UserRolesId IN @UserRoles";
                parameters.Add("UserRoles", request.UserRoles);
            }

            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }

            if (request.PageLimit > 0 && request.PageOffset >= 0)
            {
                sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
                parameters.Add("PageOffset", request.PageOffset);
                parameters.Add("PageLimit", request.PageLimit);
            }

            return (await _dbConnection.QueryAsync<User>(sql, parameters)).ToList();
        }
    }
}
