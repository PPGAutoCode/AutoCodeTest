
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
    public class UserRolesService : IUserRolesService
    {
        private readonly IDbConnection _dbConnection;

        public UserRolesService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateUserRole(CreateUserRoleDTO request)
        {
            // Step 1: Validate Fields
            if (string.IsNullOrEmpty(request.Label) || string.IsNullOrEmpty(request.ReferenceMethod))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch DefaultValue
            foreach (var defaultValueId in request.DefaultValue)
            {
                var defaultValue = await _dbConnection.QueryFirstOrDefaultAsync<DefaultValue>("SELECT * FROM DefaultValues WHERE Id = @Id", new { Id = defaultValueId });
                if (defaultValue == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 3: Create UserRole Object
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                Label = request.Label,
                HelpText = request.HelpText,
                ReferenceMethod = request.ReferenceMethod,
                DefaultValue = request.DefaultValue,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 4: Insert UserRole into Database
            const string sql = @"
                INSERT INTO UserRoles (Id, Label, HelpText, ReferenceMethod, DefaultValue, Created, Changed)
                VALUES (@Id, @Label, @HelpText, @ReferenceMethod, @DefaultValue, @Created, @Changed)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, userRole);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 5: Return the UserRole.Id from the database
            return userRole.Id.ToString();
        }

        public async Task<UserRole> GetUserRole(UserRolesRequestDTO request)
        {
            // Step 1: Validate Fields
            if (request.Id == null && string.IsNullOrEmpty(request.Label))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            UserRole userRole;

            // Step 2: Fetch UserRole from Database
            if (request.Id != null)
            {
                userRole = await _dbConnection.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                userRole = await _dbConnection.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Label = @Label", new { Label = request.Label });
            }

            // Step 3: Fetch Default Values
            if (userRole != null)
            {
                foreach (var defaultValueId in userRole.DefaultValue)
                {
                    var defaultValue = await _dbConnection.QueryFirstOrDefaultAsync<DefaultValue>("SELECT * FROM DefaultValues WHERE Id = @Id", new { Id = defaultValueId });
                    if (defaultValue == null)
                    {
                        throw new TechnicalException("DP-404", "Technical Error");
                    }
                }
            }
            else
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Return UserRole
            return userRole;
        }

        public async Task<string> UpdateUserRole(UpdateUserRoleDTO request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == null || string.IsNullOrEmpty(request.Label) || string.IsNullOrEmpty(request.ReferenceMethod) || request.DefaultValue == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing UserRole
            var userRole = await _dbConnection.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Id = @Id", new { Id = request.Id });
            if (userRole == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Validate Default Values
            foreach (var defaultValueId in request.DefaultValue)
            {
                var defaultValue = await _dbConnection.QueryFirstOrDefaultAsync<DefaultValue>("SELECT * FROM DefaultValues WHERE Id = @Id", new { Id = defaultValueId });
                if (defaultValue == null)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Update UserRole Object
            userRole.Label = request.Label;
            userRole.HelpText = request.HelpText;
            userRole.ReferenceMethod = request.ReferenceMethod;
            userRole.DefaultValue = request.DefaultValue;
            userRole.Changed = DateTime.UtcNow;

            // Step 5: Save Changes to Database
            const string sql = @"
                UPDATE UserRoles 
                SET Label = @Label, HelpText = @HelpText, ReferenceMethod = @ReferenceMethod, DefaultValue = @DefaultValue, Changed = @Changed
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, userRole);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 6: Return the UserRole.Id
            return userRole.Id.ToString();
        }

        public async Task<bool> DeleteUserRole(DeleteUserRoleDTO request)
        {
            // Step 1: Validate Necessary Parameter
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing UserRole
            var userRole = await _dbConnection.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Id = @Id", new { Id = request.Id });
            if (userRole == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete UserRole Object
            const string sql = "DELETE FROM UserRoles WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 4: Return success status
            return true;
        }

        public async Task<List<UserRole>> GetListUserRoles(UserRolesRequestDTO request)
        {
            // Step 1: Fetch UserRoles from Database
            var userRoles = await _dbConnection.QueryAsync<UserRole>("SELECT * FROM UserRoles");

            // Step 2: Return UserRoles List
            return userRoles.ToList();
        }
    }
}
