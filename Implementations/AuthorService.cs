
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
            // Validation
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Create a new Author object
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ImageId = request.ImageId,
                Details = request.Details,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            // Check if ImageId exists
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

            // Save to the database
            const string sql = @"
                INSERT INTO Authors (Id, Name, ImageId, Details, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @ImageId, @Details, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, author);
                return author.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<Author> GetAuthor(AuthorRequestDto request)
        {
            // Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Fetch from the database
            const string sql = "SELECT * FROM Authors WHERE Id = @Id";
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Fetch associated image if exists
            if (author.ImageId.HasValue)
            {
                const string imageSql = "SELECT * FROM Images WHERE Id = @ImageId";
                author.Image = await _dbConnection.QuerySingleOrDefaultAsync<Image>(imageSql, new { ImageId = author.ImageId.Value });
            }

            return author;
        }

        public async Task<string> UpdateAuthor(UpdateAuthorDto request)
        {
            // Validation
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Fetch the Author from the database
            const string fetchSql = "SELECT * FROM Authors WHERE Id = @Id";
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(fetchSql, new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Update the Author object
            author.Name = request.Name;
            author.ImageId = request.ImageId;
            author.Details = request.Details;
            author.Version += 1;
            author.Changed = DateTime.UtcNow;
            author.ChangedUser = request.ChangedUser;

            // Check if ImageId exists
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

            // Save to the database
            const string updateSql = @"
                UPDATE Authors 
                SET Name = @Name, ImageId = @ImageId, Details = @Details, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser 
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, author);
                return author.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAuthor(DeleteAuthorDto request)
        {
            // Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Fetch the Author from the database
            const string fetchSql = "SELECT * FROM Authors WHERE Id = @Id";
            var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(fetchSql, new { Id = request.Id });

            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Delete associated image if exists
            if (author.ImageId.HasValue)
            {
                const string deleteImageSql = "DELETE FROM Images WHERE Id = @ImageId";
                await _dbConnection.ExecuteAsync(deleteImageSql, new { ImageId = author.ImageId.Value });
            }

            // Delete the Author
            const string deleteSql = "DELETE FROM Authors WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<Author>> GetListAuthor(ListAuthorRequestDto request)
        {
            // Validation
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Fetch the list of Authors from the database
            var sql = $@"
                SELECT * FROM Authors 
                ORDER BY {request.SortField} {request.SortOrder} 
                OFFSET {request.PageOffset} ROWS 
                FETCH NEXT {request.PageLimit} ROWS ONLY";

            var authors = await _dbConnection.QueryAsync<Author>(sql);

            // Fetch associated images if exists
            foreach (var author in authors)
            {
                if (author.ImageId.HasValue)
                {
                    const string imageSql = "SELECT * FROM Images WHERE Id = @ImageId";
                    author.Image = await _dbConnection.QuerySingleOrDefaultAsync<Image>(imageSql, new { ImageId = author.ImageId.Value });
                }
            }

            return authors.ToList();
        }
    }
}
