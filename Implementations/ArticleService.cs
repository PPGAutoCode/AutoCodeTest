
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IBlogCategoryService _blogCategoryService;

        public ArticleService(IDbConnection dbConnection, IBlogCategoryService blogCategoryService)
        {
            _dbConnection = dbConnection;
            _blogCategoryService = blogCategoryService;
        }

        public async Task<string> CreateArticle(CreateArticleDto request)
        {
            // Step 1: Validate all fields of request.payload are not null except from [summary, body, ImageID, PDF, blogCategoryId, googleDriveId]
            if (string.IsNullOrEmpty(request.Title) || request.AuthorId == Guid.Empty || string.IsNullOrEmpty(request.Langcode) || request.Status == null || request.Sticky == null || request.Promote == null || request.BlogCategories == null || request.BlogTags == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch author from database by id from argument ID
            var author = await _dbConnection.QueryFirstOrDefaultAsync<Author>("SELECT * FROM Authors WHERE Id = @AuthorId", new { AuthorId = request.AuthorId });
            if (author == null)
            {
                throw new BusinessException("DP-404", "Author does not exist");
            }

            // Step 3: Validate BlogCategories
            var articleBlogCategories = new List<ArticleBlogCategory>();
            foreach (var categoryId in request.BlogCategories)
            {
                var blogCategoryRequestDto = new BlogCategoryRequestDto { Id = categoryId };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequestDto);
                if (blogCategory == null)
                {
                    throw new BusinessException("DP-404", "Blog category does not exist");
                }
                articleBlogCategories.Add(new ArticleBlogCategory { Id = Guid.NewGuid(), ArticleId = Guid.NewGuid(), BlogCategoryId = categoryId });
            }

            // Step 4: Validate BlogTags
            var articleBlogTags = new List<ArticleBlogTag>();
            foreach (var tagName in request.BlogTags)
            {
                var blogTag = await _dbConnection.QueryFirstOrDefaultAsync<BlogTag>("SELECT * FROM BlogTags WHERE Name = @Name", new { Name = tagName });
                if (blogTag == null)
                {
                    throw new BusinessException("DP-404", "Blog tag does not exist");
                }
                articleBlogTags.Add(new ArticleBlogTag { Id = Guid.NewGuid(), ArticleId = Guid.NewGuid(), BlogTagId = blogTag.Id });
            }

            // Step 5: Create new Article object
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                AuthorId = request.AuthorId,
                Summary = request.Summary,
                Body = request.Body,
                GoogleDriveID = request.GoogleDriveID,
                HideScrollSpy = request.HideScrollSpy,
                ImageId = request.ImageId,
                PDF = request.PDF,
                Langcode = request.Langcode,
                Status = request.Status,
                Sticky = request.Sticky,
                Promote = request.Promote,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 6: Insert article in database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO Articles (Id, Title, AuthorId, Summary, Body, GoogleDriveID, HideScrollSpy, ImageId, PDF, Langcode, Status, Sticky, Promote, Version, Created, Changed) VALUES (@Id, @Title, @AuthorId, @Summary, @Body, @GoogleDriveID, @HideScrollSpy, @ImageId, @PDF, @Langcode, @Status, @Sticky, @Promote, @Version, @Created, @Changed)", article, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", articleBlogCategories, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", articleBlogTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return article.Id.ToString();
        }

        public async Task<Article> GetArticle(ArticleRequestDto request)
        {
            if (request.Id == null && string.IsNullOrEmpty(request.Title))
            {
                throw new BusinessException("DP-422", "Id and Title are both null or empty");
            }

            Article article;
            if (request.Id != null)
            {
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Title = @Title", new { Title = request.Title });
            }

            if (article == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Fetch Blog Categories
            var blogCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
            var blogCategories = new List<BlogCategory>();
            foreach (var categoryId in blogCategoryIds)
            {
                var blogCategoryRequestDto = new BlogCategoryRequestDto { Id = categoryId };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequestDto);
                if (blogCategory == null)
                {
                    throw new BusinessException("DP-404", "Blog category not found");
                }
                blogCategories.Add(blogCategory);
            }

            // Fetch Blog Tags
            var blogTagIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
            var blogTags = await _dbConnection.QueryAsync<BlogTag>("SELECT * FROM BlogTags WHERE Id IN @Ids", new { Ids = blogTagIds });
            if (blogTags.Count() != blogTagIds.Count())
            {
                throw new BusinessException("DP-404", "Some blog tags not found");
            }

            article.BlogCategories = blogCategories.Select(bc => bc.Id).ToList();
            article.Tags = blogTags.ToList();

            return article;
        }

        public async Task<string> UpdateArticle(UpdateArticleDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Title) || request.AuthorId == Guid.Empty || string.IsNullOrEmpty(request.Langcode) || request.Status == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch Existing Article
            var existingArticle = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            if (existingArticle == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Step 3: Validate Related Entities
            var author = await _dbConnection.QueryFirstOrDefaultAsync<Author>("SELECT * FROM Authors WHERE Id = @AuthorId", new { AuthorId = request.AuthorId });
            if (author == null)
            {
                throw new BusinessException("DP-422", "Author does not exist");
            }

            if (request.ImageId != null)
            {
                var image = await _dbConnection.QueryFirstOrDefaultAsync<Image>("SELECT * FROM Images WHERE Id = @ImageId", new { ImageId = request.ImageId });
                if (image == null)
                {
                    throw new BusinessException("DP-422", "Image does not exist");
                }
            }

            if (request.PDF != null)
            {
                var pdf = await _dbConnection.QueryFirstOrDefaultAsync<File>("SELECT * FROM Files WHERE Id = @Pdf", new { Pdf = request.PDF });
                if (pdf == null)
                {
                    throw new BusinessException("DP-422", "PDF does not exist");
                }
            }

            // Step 4: Update the Article object with the provided changes
            existingArticle.Title = request.Title;
            existingArticle.AuthorId = request.AuthorId;
            existingArticle.Summary = request.Summary;
            existingArticle.Body = request.Body;
            existingArticle.GoogleDriveID = request.GoogleDriveID;
            existingArticle.HideScrollSpy = request.HideScrollSpy;
            existingArticle.ImageId = request.ImageId;
            existingArticle.PDF = request.PDF;
            existingArticle.Langcode = request.Langcode;
            existingArticle.Status = request.Status;
            existingArticle.Sticky = request.Sticky;
            existingArticle.Promote = request.Promote;
            existingArticle.Version += 1;
            existingArticle.Changed = DateTime.UtcNow;

            // Step 5: Update ArticleBlogCategories
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id });
            var categoriesToRemove = existingCategories.Except(request.BlogCategories).ToList();
            var categoriesToAdd = request.BlogCategories.Except(existingCategories).ToList();

            // Step 6: Update ArticleBlogTags
            var existingTags = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id });
            var tagsToRemove = existingTags.Except(request.BlogTags.Select(bt => bt.Id)).ToList();
            var tagsToAdd = request.BlogTags.Select(bt => bt.Id).Except(existingTags).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Update Article Table
                    await _dbConnection.ExecuteAsync("UPDATE Articles SET Title = @Title, AuthorId = @AuthorId, Summary = @Summary, Body = @Body, GoogleDriveID = @GoogleDriveID, HideScrollSpy = @HideScrollSpy, ImageId = @ImageId, PDF = @PDF, Langcode = @Langcode, Status = @Status, Sticky = @Sticky, Promote = @Promote, Version = @Version, Changed = @Changed WHERE Id = @Id", existingArticle, transaction);

                    // Remove old categories
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId AND BlogCategoryId IN @CategoriesToRemove", new { ArticleId = existingArticle.Id, CategoriesToRemove = categoriesToRemove }, transaction);

                    // Add new categories
                    foreach (var categoryId in categoriesToAdd)
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", new { Id = Guid.NewGuid(), ArticleId = existingArticle.Id, BlogCategoryId = categoryId }, transaction);
                    }

                    // Remove old tags
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId AND BlogTagId IN @TagsToRemove", new { ArticleId = existingArticle.Id, TagsToRemove = tagsToRemove }, transaction);

                    // Add new tags
                    foreach (var tagId in tagsToAdd)
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", new { Id = Guid.NewGuid(), ArticleId = existingArticle.Id, BlogTagId = tagId }, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return existingArticle.Id.ToString();
        }

        public async Task<bool> DeleteArticle(DeleteArticleDto request)
        {
            // Step 1: Validate Input
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Id is missing or invalid");
            }

            // Step 2: Fetch Existing Article
            var existingArticle = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            if (existingArticle == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Step 3: Delete Article
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM Articles WHERE Id = @Id", new { Id = existingArticle.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return true;
        }
    }
}
