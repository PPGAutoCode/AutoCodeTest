
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
            if (request == null || string.IsNullOrEmpty(request.Nickname) || string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
                string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.SiteLanguage) ||
                string.IsNullOrEmpty(request.LocaleSettings) || request.UserType == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Request Validation
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Create new User object
            var user = new User
            {
                Id = Guid.NewGuid(),
                Nickname = request.Nickname,
                Status = request.Status,
                Email = request.Email,
                ContactSettings = request.ContactSettings,
                SiteLanguage = request.SiteLanguage,
                LocaleSettings = request.LocaleSettings,
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

            // Step 4: In a single SQL transaction
            try
            {
                const string sql = @"
                    INSERT INTO Users (Id, Nickname, Status, Email, ContactSettings, SiteLanguage, LocaleSettings, FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, UserQuestionnaire, Created, LastAccess)
                    VALUES (@Id, @Nickname, @Status, @Email, @ContactSettings, @SiteLanguage, @LocaleSettings, @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @UserQuestionnaire, @Created, @LastAccess);
                ";
                await _dbConnection.ExecuteAsync(sql, user);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 5: Return UserId from database
            return user.Id.ToString();
        }

        public async Task<User> GetUser(UserRequestDto request)
        {
            // Step 1: Validate UserRequestDto fields
            if (request == null || (request.Id == null && string.IsNullOrEmpty(request.Nickname) && string.IsNullOrEmpty(request.Email) &&
                string.IsNullOrEmpty(request.FirstName) && string.IsNullOrEmpty(request.LastName)))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user from database by the requested field
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(@"
                SELECT * FROM Users WHERE Id = @Id OR Nickname = @Nickname OR Email = @Email OR FirstName = @FirstName OR LastName = @LastName
            ", request);

            // Step 3: If the user exists
            if (user != null)
            {
                return user;
            }

            // Step 4: Else
            throw new TechnicalException("DP-404", "Technical Error");
        }

        public async Task<string> UpdateUser(UpdateUserRequestDto request)
        {
            // Step 1: Validate the incoming request
            if (request == null || request.Id == Guid.Empty || string.IsNullOrEmpty(request.Nickname) || string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
                string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.SiteLanguage) ||
                string.IsNullOrEmpty(request.LocaleSettings) || request.UserType == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing User object from the database
            var existingUser = await _dbConnection.QueryFirstOrDefaultAsync<User>(@"
                SELECT * FROM Users WHERE Id = @Id
            ", new { Id = request.Id });

            if (existingUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the User object with the provided changes
            existingUser.Nickname = request.Nickname;
            existingUser.Status = request.Status;
            existingUser.Email = request.Email;
            existingUser.ContactSettings = request.ContactSettings;
            existingUser.SiteLanguage = request.SiteLanguage;
            existingUser.LocaleSettings = request.LocaleSettings;
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
                    UPDATE Users SET Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, FirstName = @FirstName, LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, CompletedStories = @CompletedStories, UserType = @UserType, UserQuestionnaire = @UserQuestionnaire, LastAccess = @LastAccess WHERE Id = @Id;
                ";
                await _dbConnection.ExecuteAsync(sql, existingUser);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 5: Return UserId from database
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
            var existingUser = await _dbConnection.QueryFirstOrDefaultAsync<User>(@"
                SELECT * FROM Users WHERE Id = @Id
            ", new { Id = request.Id });

            if (existingUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the User object from the database
            try
            {
                const string sql = @"
                    DELETE FROM Users WHERE Id = @Id;
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
            // Step 1: Request Validation
            if (request == null || request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Sorting and Pagination
            var sortField = string.IsNullOrEmpty(request.SortField) ? "Nickname" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "ASC" : request.SortOrder;

            // Step 3: Fetching User Data
            var users = await _dbConnection.QueryAsync<User>(@"
                SELECT * FROM Users ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY
            ", new { SortField = sortField, SortOrder = sortOrder, PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            // Step 4: Constructing the Response
            return users.ToList();
        }
    }
}
