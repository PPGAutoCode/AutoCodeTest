
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
    public class AuthorService : IAuthorService
    {
        private readonly IDbConnection _dbConnection;

        public AuthorService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAuthor(CreateAuthorDto request)
        {
            // Step 1: Validation
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new Author object
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ImageId = request.ImageId,
                Details = request.Details
            };

            // Step 3: Check if ImageId exists
            if (request.ImageId.HasValue)
            {
                var imageExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM Images WHERE Id = @ImageId",
                    new { ImageId = request.ImageId.Value });

                if (!imageExists)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Save the Author to the database
            try
            {
                await _dbConnection.ExecuteAsync(
                    "INSERT INTO Authors (Id, Name, ImageId, Details) VALUES (@Id, @Name, @ImageId, @Details)",
                    author);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return author.Id.ToString();
        }

        public async Task<Author> GetAuthor(AuthorRequestDto request)
        {
            // Step 1: Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Author from the database
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(
                "SELECT * FROM Authors WHERE Id = @Id",
                new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch the Image if ImageId is not null
            if (author.ImageId.HasValue)
            {
                author.Image = await _dbConnection.QuerySingleOrDefaultAsync<Image>(
                    "SELECT * FROM Images WHERE Id = @ImageId",
                    new { ImageId = author.ImageId.Value });
            }

            return author;
        }

        public async Task<string> UpdateAuthor(UpdateAuthorDto request)
        {
            // Step 1: Validation
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Author from the database
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(
                "SELECT * FROM Authors WHERE Id = @Id",
                new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the Author object
            author.Name = request.Name;
            author.ImageId = request.ImageId;
            author.Details = request.Details;

            // Step 4: Check if ImageId exists
            if (request.ImageId.HasValue)
            {
                var imageExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM Images WHERE Id = @ImageId",
                    new { ImageId = request.ImageId.Value });

                if (!imageExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 5: Save the updated Author to the database
            try
            {
                await _dbConnection.ExecuteAsync(
                    "UPDATE Authors SET Name = @Name, ImageId = @ImageId, Details = @Details WHERE Id = @Id",
                    author);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return author.Id.ToString();
        }

        public async Task<bool> DeleteAuthor(DeleteAuthorDto request)
        {
            // Step 1: Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Author from the database
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(
                "SELECT * FROM Authors WHERE Id = @Id",
                new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the Author from the database
            try
            {
                if (author.ImageId.HasValue)
                {
                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM Images WHERE Id = @ImageId",
                        new { ImageId = author.ImageId.Value });
                }

                await _dbConnection.ExecuteAsync(
                    "DELETE FROM Authors WHERE Id = @Id",
                    new { Id = request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<Author>> GetListAuthor(ListAuthorRequestDto request)
        {
            // Step 1: Validation
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of Authors from the database
            var authors = await _dbConnection.QueryAsync<Author>(
                "SELECT * FROM Authors ORDER BY @SortField @SortOrder LIMIT @PageLimit OFFSET @PageOffset",
                new
                {
                    SortField = request.SortField,
                    SortOrder = request.SortOrder,
                    PageLimit = request.PageLimit,
                    PageOffset = request.PageOffset
                });

            var authorList = authors.ToList();

            // Step 3: Fetch the Images for each Author if ImageId is not null
            foreach (var author in authorList)
            {
                if (author.ImageId.HasValue)
                {
                    author.Image = await _dbConnection.QuerySingleOrDefaultAsync<Image>(
                        "SELECT * FROM Images WHERE Id = @ImageId",
                        new { ImageId = author.ImageId.Value });
                }
            }

            return authorList;
        }
    }
}
