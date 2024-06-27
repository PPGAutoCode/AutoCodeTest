
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class ImageService : IImageService
    {
        private readonly IDbConnection _dbConnection;

        public ImageService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateImage(CreateImageDto request)
        {
            // Step 1: Validate the request payload
            if (request.FileName == null || request.Image == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Modify the file name
            string modifiedFileName = request.FileName + "_original";

            // Step 3: Create an Image object
            var image = new Image
            {
                Id = Guid.NewGuid(),
                FileName = modifiedFileName,
                ImageData = request.Image,
                AltText = request.AltText,
                Version = 1, // Assuming initial version is 1
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.ChangedUser
            };

            // Step 4: Insert the newly created Image object to the database
            const string sql = @"
                INSERT INTO Images (Id, FileName, ImageData, AltText, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @FileName, @ImageData, @AltText, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, image);
                return image.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        // Implement other methods from IImageService interface similarly
    }
}
