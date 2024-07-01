
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
                    INSERT INTO Users (Id, Nickname, Status, Email, ContactSettings, UserRoles, SiteLanguage, LocaleSettings, Image, FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, UserQuestionnaire, Created, LastAccess)
                    VALUES (@Id, @Nickname, @Status, @Email, @ContactSettings, @UserRoles, @SiteLanguage, @LocaleSettings, @Image, @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @UserQuestionnaire, @Created, @LastAccess);
                ";
                await _dbConnection.ExecuteAsync(sql, user);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 6: Return UserId from database
            return user.Id.ToString();
        }

        public async Task<User> GetUser(UserRequestDto request)
        {
            // Step 1: Validate UserRequestDto fields
            if (request == null || !request.AnyFieldSet())
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user from database by the requested field
            var user = await FetchUserByRequest(request);

            // Step 3: If the user exists, return the User
            if (user != null)
            {
                return user;
            }

            // Step 4: If the user does not exist, throw exception
            throw new TechnicalException("DP-404", "Technical Error");
        }

        public async Task<string> UpdateUser(UpdateUserRequestDto request)
        {
            // Step 1: Validate the incoming request
            if (request == null || !ValidateUpdateUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Verify unique fields
            if (!await ValidateUniqueFields(request.Nickname, request.Email, request.Phone, request.IBM_UId))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Fetch the existing User object from the database
            var existingUser = await FetchUserById(request.Id);
            if (existingUser == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 4: Update the User object with the provided changes
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

            // Step 5: Save the updated User object back to the database
            try
            {
                const string sql = @"
                    UPDATE Users 
                    SET Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, UserRoles = @UserRoles, SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, Image = @Image, FirstName = @FirstName, LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, CompletedStories = @CompletedStories, UserType = @UserType, UserQuestionnaire = @UserQuestionnaire, LastAccess = @LastAccess
                    WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, existingUser);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 6: Return UserId from database
            return existingUser.Id.ToString();
        }

        public async Task<bool> DeleteUser(DeleteUserRequestDto request)
        {
            // Step 1: Validate that the request.payload.id contains the necessary parameter
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Get the existing User object from the database
            var existingUser = await FetchUserById(request.Id);
            if (existingUser == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
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
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 4: Return true if the transaction is successful
            return true;
        }

        public async Task<List<User>> GetListUser(ListUserRequestDto request)
        {
            // Step 1: Validate the incoming request
            if (request == null || !ValidateListUserRequest(request))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Apply sorting and pagination
            var sortField = request.SortField ?? "Nickname";
            var sortOrder = request.SortOrder ?? "ASC";
            var pageLimit = request.PageLimit ?? 10;
            var pageOffset = request.PageOffset ?? 0;

            // Step 3: Fetching User Data
            var users = await FetchUsersByRequest(request, sortField, sortOrder, pageLimit, pageOffset);

            // Step 4: Constructing the Response
            return users;
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
                   !string.IsNullOrEmpty(request.IBM_UId) &&
                   request.UserType != Guid.Empty;
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
                   !string.IsNullOrEmpty(request.IBM_UId) &&
                   request.UserType != Guid.Empty;
        }

        private bool ValidateListUserRequest(ListUserRequestDto request)
        {
            return request.PageLimit > 0 && request.PageOffset >= 0;
        }

        private async Task<bool> ValidateUniqueFields(string nickname, string email, string phone, string ibmUId)
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM Users 
                WHERE Nickname = @Nickname OR Email = @Email OR Phone = @Phone OR IBM_UId = @IBM_UId;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Nickname = nickname, Email = email, Phone = phone, IBM_UId = ibmUId });
            return count == 0;
        }

        private async Task<User> FetchUserById(Guid id)
        {
            const string sql = @"
                SELECT * 
                FROM Users 
                WHERE Id = @Id;
            ";
            return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        private async Task<User> FetchUserByRequest(UserRequestDto request)
        {
            var sql = @"
                SELECT * 
                FROM Users 
                WHERE 1=1";

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

            return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, parameters);
        }

        private async Task<List<User>> FetchUsersByRequest(ListUserRequestDto request, string sortField, string sortOrder, int pageLimit, int pageOffset)
        {
            var sql = @"
                SELECT * 
                FROM Users 
                WHERE 1=1";

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

            if (request.Status != null)
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", request.Status);
            }

            if (request.UserRoles != null && request.UserRoles.Any())
            {
                sql += " AND UserRoles IN @UserRoles";
                parameters.Add("UserRoles", request.UserRoles);
            }

            sql += $" ORDER BY {sortField} {sortOrder}";
            sql += " LIMIT @PageLimit OFFSET @PageOffset";
            parameters.Add("PageLimit", pageLimit);
            parameters.Add("PageOffset", pageOffset);

            return (await _dbConnection.QueryAsync<User>(sql, parameters)).ToList();
        }
    }
}
