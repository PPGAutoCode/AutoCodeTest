
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class BlogTagService : IBlogTagService
    {
        private readonly IDbConnection _dbConnection;

        public BlogTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateBlogTag(CreateBlogTagDto createBlogTagDto)
        {
            if (string.IsNullOrEmpty(createBlogTagDto.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var blogTag = new BlogTag
            {
                Id = Guid.NewGuid(),
                Name = createBlogTagDto.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming UserId is available in headers
                ChangedUser = Guid.NewGuid() // Assuming UserId is available in headers
            };

            const string sql = @"
                INSERT INTO BlogTags (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser);
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, blogTag);
                return blogTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<BlogTag> GetBlogTag(BlogTagRequestDto blogTagRequestDto)
        {
            if (blogTagRequestDto.Id == null && string.IsNullOrEmpty(blogTagRequestDto.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            BlogTag blogTag;

            if (blogTagRequestDto.Id != null)
            {
                const string sql = "SELECT * FROM BlogTags WHERE Id = @Id;";
                blogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(sql, new { Id = blogTagRequestDto.Id });
            }
            else
            {
                const string sql = "SELECT * FROM BlogTags WHERE Name = @Name;";
                blogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(sql, new { Name = blogTagRequestDto.Name });
            }

            if (blogTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return blogTag;
        }

        public async Task<string> UpdateBlogTag(UpdateBlogTagDto updateBlogTagDto)
        {
            if (updateBlogTagDto.Id == null || string.IsNullOrEmpty(updateBlogTagDto.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BlogTags WHERE Id = @Id;";
            var existingBlogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(selectSql, new { Id = updateBlogTagDto.Id });

            if (existingBlogTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingBlogTag.Name = updateBlogTagDto.Name;
            existingBlogTag.Version += 1;
            existingBlogTag.Changed = DateTime.UtcNow;
            existingBlogTag.ChangedUser = Guid.NewGuid(); // Assuming UserId is available in headers

            const string updateSql = @"
                UPDATE BlogTags
                SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingBlogTag);
                return existingBlogTag.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteBlogTag(DeleteBlogTagDto deleteBlogTagDto)
        {
            if (deleteBlogTagDto.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM BlogTags WHERE Id = @Id;";
            var existingBlogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(selectSql, new { Id = deleteBlogTagDto.Id });

            if (existingBlogTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM BlogTags WHERE Id = @Id;";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { Id = deleteBlogTagDto.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<BlogTag>> GetListBlogTag(ListBlogTagRequestDto listBlogTagRequestDto)
        {
            if (listBlogTagRequestDto.PageLimit <= 0 || listBlogTagRequestDto.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM BlogTags
                ORDER BY 
                    CASE WHEN @SortField = 'Name' AND @SortOrder = 'ASC' THEN Name END ASC,
                    CASE WHEN @SortField = 'Name' AND @SortOrder = 'DESC' THEN Name END DESC
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                var blogTags = await _dbConnection.QueryAsync<BlogTag>(sql, listBlogTagRequestDto);
                return blogTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
