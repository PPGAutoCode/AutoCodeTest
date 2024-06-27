
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class BlogTagService : IBlogTagService
    {
        private readonly IDbConnection _dbConnection;

        public BlogTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateBlogTag(CreateBlogTagDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var blogTag = new BlogTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a default creator ID or replace with actual logic
                ChangedUser = Guid.NewGuid() // Assuming a default changed user ID or replace with actual logic
            };

            const string sql = @"
                INSERT INTO BlogTags (Id, Name, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Created, @Changed, @CreatorId, @ChangedUser)";

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

        public async Task<BlogTag> GetBlogTag(BlogTagRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"
                SELECT * FROM BlogTags WHERE Id = @Id";

            try
            {
                var blogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(sql, new { request.Id });
                if (blogTag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return blogTag;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateBlogTag(UpdateBlogTagDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM BlogTags WHERE Id = @Id";

            var existingBlogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(selectSql, new { request.Id });
            if (existingBlogTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingBlogTag.Name = request.Name;
            existingBlogTag.Changed = DateTime.UtcNow;

            const string updateSql = @"
                UPDATE BlogTags SET Name = @Name, Changed = @Changed WHERE Id = @Id";

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

        public async Task<bool> DeleteBlogTag(DeleteBlogTagDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"
                SELECT * FROM BlogTags WHERE Id = @Id";

            var existingBlogTag = await _dbConnection.QuerySingleOrDefaultAsync<BlogTag>(selectSql, new { request.Id });
            if (existingBlogTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"
                DELETE FROM BlogTags WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteSql, new { request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<BlogTag>> GetListBlogTag(GetListBlogTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = @"
                SELECT * FROM BlogTags
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var blogTags = await _dbConnection.QueryAsync<BlogTag>(sql, new
                {
                    request.PageLimit,
                    request.PageOffset,
                    SortField = request.SortField ?? "Created",
                    SortOrder = request.SortOrder ?? "ASC"
                });
                return blogTags.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
