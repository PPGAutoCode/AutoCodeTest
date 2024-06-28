
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

        public async Task<string> CreateGoLive(CreateGoLiveDto request)
        {
            // Step 1: Validate the request payload
            if (request == null || 
                string.IsNullOrEmpty(request.Address) || 
                request.ApplicationId == Guid.Empty || 
                string.IsNullOrEmpty(request.CompanyName) || 
                string.IsNullOrEmpty(request.DeveloperType) || 
                string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.FirstName) || 
                string.IsNullOrEmpty(request.LastName) || 
                string.IsNullOrEmpty(request.SiteUrlDownloadLocation) || 
                string.IsNullOrEmpty(request.UseCases))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new GoLive object with the provided details
            var goLive = new GoLive
            {
                Id = Guid.NewGuid(),
                Address = request.Address,
                ApplicationId = request.ApplicationId,
                CompanyName = request.CompanyName,
                DeveloperType = request.DeveloperType,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                SiteUrlDownloadLocation = request.SiteUrlDownloadLocation,
                UseCases = request.UseCases
            };

            // Step 3: Insert the newly created GoLive object to the database
            const string sql = @"
                INSERT INTO GoLive (Id, Address, ApplicationId, CompanyName, DeveloperType, Email, FirstName, LastName, SiteUrlDownloadLocation, UseCases)
                VALUES (@Id, @Address, @ApplicationId, @CompanyName, @DeveloperType, @Email, @FirstName, @LastName, @SiteUrlDownloadLocation, @UseCases)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, goLive);
                return goLive.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
