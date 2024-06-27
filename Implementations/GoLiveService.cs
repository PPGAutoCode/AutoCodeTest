
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class GoLiveService : IGoLiveService
    {
        private readonly IDbConnection _dbConnection;

        public GoLiveService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateGoLive(CreateGoLiveDTO request)
        {
            // Step 1: Validate the request payload
            if (request.Address == null || request.ApplicationId == null || request.CompanyName == null ||
                request.DeveloperType == null || request.Email == null || request.FirstName == null ||
                request.LastName == null || request.SiteUrlDownloadLocation == null || request.UseCases == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new GoLive object with the provided details
            var goLive = new GoLive
            {
                Address = request.Address,
                ApplicationId = request.ApplicationId,
                CompanyName = request.CompanyName,
                DeveloperType = request.DeveloperType,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                SiteUrlDownloadLocation = request.SiteUrlDownloadLocation,
                UseCases = request.UseCases,
                Created = DateTime.UtcNow
            };

            // Step 3: Insert the newly created GoLive object to the database
            const string sql = @"
                INSERT INTO GoLive (Id, Address, ApplicationId, CompanyName, DeveloperType, Email, FirstName, LastName, SiteUrlDownloadLocation, UseCases, Created)
                VALUES (@Id, @Address, @ApplicationId, @CompanyName, @DeveloperType, @Email, @FirstName, @LastName, @SiteUrlDownloadLocation, @UseCases, @Created);
                SELECT @Id;
            ";

            try
            {
                var id = await _dbConnection.QuerySingleAsync<Guid>(sql, goLive);
                return id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
