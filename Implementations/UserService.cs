
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
                    INSERT INTO Users (
                        Id, Nickname, Status, Email, ContactSettings, UserRoles, SiteLanguage, LocaleSettings, Image, 
                        FirstName, LastName, Company, Phone, IBM_UId, MaxNumApps, CompletedStories, UserType, UserQuestionnaire, 
                        Created, LastAccess
                    ) VALUES (
                        @Id, @Nickname, @Status, @Email, @ContactSettings, @UserRoles, @SiteLanguage, @LocaleSettings, @Image, 
                        @FirstName, @LastName, @Company, @Phone, @IBM_UId, @MaxNumApps, @CompletedStories, @UserType, @UserQuestionnaire, 
                        @Created, @LastAccess
                    );
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
            if (request == null || !ValidateUserRequestDto(request))
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
            var user = await FetchUserById(request.Id);
            if (user == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Update the User object with the provided changes
            user.Nickname = request.Nickname;
            user.Status = request.Status;
            user.Email = request.Email;
            user.ContactSettings = request.ContactSettings;
            user.UserRoles = request.UserRoles;
            user.SiteLanguage = request.SiteLanguage;
            user.LocaleSettings = request.LocaleSettings;
            user.Image = request.Image;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Company = request.Company;
            user.Phone = request.Phone;
            user.IBM_UId = request.IBM_UId;
            user.MaxNumApps = request.MaxNumApps;
            user.CompletedStories = request.CompletedStories;
            user.UserType = request.UserType;
            user.UserQuestionnaire = request.UserQuestionnaire;
            user.LastAccess = DateTime.UtcNow;

            // Step 5: Save the updated User object back to the database
            try
            {
                const string sql = @"
                    UPDATE Users SET
                        Nickname = @Nickname, Status = @Status, Email = @Email, ContactSettings = @ContactSettings, UserRoles = @UserRoles, 
                        SiteLanguage = @SiteLanguage, LocaleSettings = @LocaleSettings, Image = @Image, FirstName = @FirstName, 
                        LastName = @LastName, Company = @Company, Phone = @Phone, IBM_UId = @IBM_UId, MaxNumApps = @MaxNumApps, 
                        CompletedStories = @CompletedStories, UserType = @UserType, UserQuestionnaire = @UserQuestionnaire, 
                        LastAccess = @LastAccess
                    WHERE Id = @Id;
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

        public async Task<bool> DeleteUser(DeleteUserRequestDto request)
        {
            // Step 1: Validate that the request.payload.id contains the necessary parameter
            if (request == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Get the existing User object from the database
            var user = await FetchUserById(request.Id);
            if (user == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the User object from the database
            try
            {
                const string sql = "DELETE FROM Users WHERE Id = @Id;";
                await _dbConnection.ExecuteAsync(sql, new { user.Id });
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
            var sortField = string.IsNullOrEmpty(request.SortField) ? "Nickname" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "ASC" : request.SortOrder;
            var pageLimit = request.PageLimit <= 0 ? 10 : request.PageLimit;
            var pageOffset = request.PageOffset < 0 ? 0 : request.PageOffset;

            // Step 3: Fetching User Data
            var sql = $@"
                SELECT * FROM Users
                WHERE (@Id IS NULL OR Id = @Id)
                AND (@Nickname IS NULL OR Nickname = @Nickname)
                AND (@Email IS NULL OR Email = @Email)
                AND (@Status IS NULL OR Status = @Status)
                ORDER BY {sortField} {sortOrder}
                LIMIT {pageLimit} OFFSET {pageOffset};
            ";

            var users = await _dbConnection.QueryAsync<User>(sql, new
            {
                request.Id,
                request.Nickname,
                request.Email,
                request.Status
            });

            // Step 4: Return the list of Users
            return users.ToList();
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

        private bool ValidateUserRequestDto(UserRequestDto request)
        {
            return request.Id != Guid.Empty ||
                   !string.IsNullOrEmpty(request.Nickname) ||
                   !string.IsNullOrEmpty(request.Email) ||
                   !string.IsNullOrEmpty(request.Phone) ||
                   !string.IsNullOrEmpty(request.IBM_UId);
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
                SELECT COUNT(1) FROM Users
                WHERE Nickname = @Nickname OR Email = @Email OR Phone = @Phone OR IBM_UId = @IBM_UId;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { nickname, email, phone, ibmUId });
            return count == 0;
        }

        private async Task<User> FetchUserByRequest(UserRequestDto request)
        {
            var sql = @"
                SELECT * FROM Users
                WHERE Id = @Id OR Nickname = @Nickname OR Email = @Email OR Phone = @Phone OR IBM_UId = @IBM_UId;
            ";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new
            {
                request.Id,
                request.Nickname,
                request.Email,
                request.Phone,
                request.IBM_UId
            });
        }

        private async Task<User> FetchUserById(Guid id)
        {
            var sql = "SELECT * FROM Users WHERE Id = @Id;";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }
    }
}
