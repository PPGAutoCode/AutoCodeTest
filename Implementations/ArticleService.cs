
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
    public class ArticleService : IArticleService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IBlogCategoryService _blogCategoryService;
        private readonly IBlogTagService _blogTagService;

        public ArticleService(IDbConnection dbConnection, IBlogCategoryService blogCategoryService, IBlogTagService blogTagService)
        {
            _dbConnection = dbConnection;
            _blogCategoryService = blogCategoryService;
            _blogTagService = blogTagService;
        }

        public async Task<string> CreateArticle(CreateArticleDto createArticleDto)
        {
            // Step 1: Validate all fields of request.payload are not null except from [summary, body, ImageID, PDF, blogCategoryId, googleDriveId]
            if (createArticleDto.Title == null || createArticleDto.AuthorId == Guid.Empty || createArticleDto.Langcode == null || createArticleDto.Status == null || createArticleDto.Sticky == null || createArticleDto.Promote == null || createArticleDto.BlogCategories == null || createArticleDto.BlogTags == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch author from database by id from argument ID
            var author = await _dbConnection.QueryFirstOrDefaultAsync<Author>("SELECT * FROM Authors WHERE Id = @AuthorId", new { AuthorId = createArticleDto.AuthorId });
            if (author == null)
            {
                throw new BusinessException("DP-404", "Author does not exist");
            }

            // Step 4: Fetch blog category by using the service that implements the IBlogCategoryService interface
            foreach (var categoryId in createArticleDto.BlogCategories)
            {
                var blogCategoryRequestDto = new BlogCategoryRequestDto { Id = categoryId };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequestDto);
                if (blogCategory == null)
                {
                    throw new BusinessException("DP-404", "Blog category does not exist");
                }
            }

            // Step 5: Fetch BlogTag by using the service that implements the IBlogTagService interface
            var tagsList = new List<BlogTag>();
            foreach (var tagName in createArticleDto.BlogTags)
            {
                var blogTagRequestDto = new BlogTagRequestDto { Name = tagName };
                var blogTag = await _blogTagService.GetBlogTag(blogTagRequestDto);
                if (blogTag == null)
                {
                    throw new BusinessException("DP-404", "Blog tag does not exist");
                }
                tagsList.Add(blogTag);
            }

            // Step 6: Create new Article object
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = createArticleDto.Title,
                AuthorId = createArticleDto.AuthorId,
                Summary = createArticleDto.Summary,
                Body = createArticleDto.Body,
                GoogleDriveID = createArticleDto.GoogleDriveID,
                HideScrollSpy = createArticleDto.HideScrollSpy,
                ImageId = createArticleDto.ImageId,
                PDF = createArticleDto.PDF,
                Langcode = createArticleDto.Langcode,
                Status = createArticleDto.Status,
                Sticky = createArticleDto.Sticky,
                Promote = createArticleDto.Promote,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 7: Create new list of ArticleBlogCategories objects
            var articleBlogCategories = createArticleDto.BlogCategories.Select(categoryId => new ArticleBlogCategory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogCategoryId = categoryId
            }).ToList();

            // Step 8: Create new list of ArticleBlogTags objects
            var articleBlogTags = tagsList.Select(tag => new ArticleBlogTag
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogTagId = tag.Id
            }).ToList();

            // Step 9: In a single SQL transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert article in database table Article
                    await _dbConnection.ExecuteAsync("INSERT INTO Articles (Id, Title, AuthorId, Summary, Body, GoogleDriveID, HideScrollSpy, ImageId, PDF, Langcode, Status, Sticky, Promote, Version, Created, Changed) VALUES (@Id, @Title, @AuthorId, @Summary, @Body, @GoogleDriveID, @HideScrollSpy, @ImageId, @PDF, @Langcode, @Status, @Sticky, @Promote, @Version, @Created, @Changed)", article, transaction);

                    // Insert articleBlogCategories in database table ArticleBlogCategories
                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", articleBlogCategories, transaction);

                    // Insert articleBlogTags in database table ArticleBlogTags
                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", articleBlogTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            // Step 10: Return ArticleId from database
            return article.Id.ToString();
        }

        public async Task<Article> GetArticle(ArticleRequestDto articleRequestDto)
        {
            // Step 1: Validate request.payload.id and request.payload.name
            if (articleRequestDto.Id == null && string.IsNullOrEmpty(articleRequestDto.Title))
            {
                throw new BusinessException("DP-422", "Both Id and Title are null or empty");
            }

            Article article;
            if (articleRequestDto.Id != null)
            {
                // Step 2: Fetch article from database by id
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = articleRequestDto.Id });
            }
            else
            {
                // Step 3: Fetch article from database by name
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Title = @Title", new { Title = articleRequestDto.Title });
            }

            if (article == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Step 4: Fetch Blog Categories
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

            // Step 5: Fetch Blogtags
            var blogTagIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
            var blogTags = await _dbConnection.QueryAsync<BlogTag>("SELECT * FROM BlogTags WHERE Id IN @BlogTagIds", new { BlogTagIds = blogTagIds });
            if (blogTags.Any(tag => tag == null))
            {
                throw new BusinessException("DP-404", "Blog tag not found");
            }

            // Map database object to Article and return the Article
            article.BlogCategories = blogCategories.Select(bc => bc.Id).ToList();
            article.Tags = blogTags.ToList();
            return article;
        }

        public async Task<string> UpdateArticle(UpdateArticleDto updateArticleDto)
        {
            // Step 1: Validate Necessary Parameters
            if (updateArticleDto.Id == Guid.Empty || string.IsNullOrEmpty(updateArticleDto.Title) || updateArticleDto.AuthorId == Guid.Empty || string.IsNullOrEmpty(updateArticleDto.Langcode) || updateArticleDto.Status == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null or invalid");
            }

            // Step 2: Fetch Existing Article
            var existingArticle = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = updateArticleDto.Id });
            if (existingArticle == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Step 3: Validate Related Entities
            var author = await _dbConnection.QueryFirstOrDefaultAsync<Author>("SELECT * FROM Authors WHERE Id = @AuthorId", new { AuthorId = updateArticleDto.AuthorId });
            if (author == null)
            {
                throw new BusinessException("DP-422", "Author does not exist");
            }

            if (updateArticleDto.ImageId != null)
            {
                var image = await _dbConnection.QueryFirstOrDefaultAsync<Image>("SELECT * FROM Images WHERE Id = @ImageId", new { ImageId = updateArticleDto.ImageId });
                if (image == null)
                {
                    throw new BusinessException("DP-422", "Image does not exist");
                }
            }

            if (updateArticleDto.PDF != null)
            {
                var pdf = await _dbConnection.QueryFirstOrDefaultAsync<Attachment>("SELECT * FROM Attachments WHERE Id = @Pdf", new { Pdf = updateArticleDto.PDF });
                if (pdf == null)
                {
                    throw new BusinessException("DP-422", "PDF does not exist");
                }
            }

            // Step 4: Update the Article object with the provided changes
            existingArticle.Title = updateArticleDto.Title;
            existingArticle.AuthorId = updateArticleDto.AuthorId;
            existingArticle.Summary = updateArticleDto.Summary;
            existingArticle.Body = updateArticleDto.Body;
            existingArticle.GoogleDriveID = updateArticleDto.GoogleDriveID;
            existingArticle.HideScrollSpy = updateArticleDto.HideScrollSpy;
            existingArticle.ImageId = updateArticleDto.ImageId;
            existingArticle.PDF = updateArticleDto.PDF;
            existingArticle.Langcode = updateArticleDto.Langcode;
            existingArticle.Status = updateArticleDto.Status;
            existingArticle.Sticky = updateArticleDto.Sticky;
            existingArticle.Promote = updateArticleDto.Promote;
            existingArticle.Version += 1;
            existingArticle.Changed = DateTime.UtcNow;

            // Step 5: Update ArticleBlogCategories
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id });
            var categoriesToRemove = existingCategories.Except(updateArticleDto.BlogCategories).ToList();
            var categoriesToAdd = updateArticleDto.BlogCategories.Except(existingCategories).ToList();

            // Step 6: Update ArticleBlogTags
            var existingTags = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = existingArticle.Id });
            var updatedTags = new List<Guid>();
            foreach (var tagName in updateArticleDto.BlogTags)
            {
                var blogTag = await _dbConnection.QueryFirstOrDefaultAsync<BlogTag>("SELECT * FROM BlogTags WHERE Name = @Name", new { Name = tagName });
                if (blogTag == null)
                {
                    blogTag = new BlogTag { Id = Guid.NewGuid(), Name = tagName };
                    await _dbConnection.ExecuteAsync("INSERT INTO BlogTags (Id, Name) VALUES (@Id, @Name)", blogTag);
                }
                updatedTags.Add(blogTag.Id);
            }

            var tagsToRemove = existingTags.Except(updatedTags).ToList();
            var tagsToAdd = updatedTags.Except(existingTags).ToList();

            // Step 7: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Update Article Table
                    await _dbConnection.ExecuteAsync("UPDATE Articles SET Title = @Title, AuthorId = @AuthorId, Summary = @Summary, Body = @Body, GoogleDriveID = @GoogleDriveID, HideScrollSpy = @HideScrollSpy, ImageId = @ImageId, PDF = @PDF, Langcode = @Langcode, Status = @Status, Sticky = @Sticky, Promote = @Promote, Version = @Version, Changed = @Changed WHERE Id = @Id", existingArticle, transaction);

                    // Remove old categories
                    if (categoriesToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId AND BlogCategoryId IN @BlogCategoryIds", new { ArticleId = existingArticle.Id, BlogCategoryIds = categoriesToRemove }, transaction);
                    }

                    // Add new categories
                    if (categoriesToAdd.Any())
                    {
                        var newCategories = categoriesToAdd.Select(categoryId => new ArticleBlogCategory { Id = Guid.NewGuid(), ArticleId = existingArticle.Id, BlogCategoryId = categoryId }).ToList();
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", newCategories, transaction);
                    }

                    // Remove old tags
                    if (tagsToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId AND BlogTagId IN @BlogTagIds", new { ArticleId = existingArticle.Id, BlogTagIds = tagsToRemove }, transaction);
                    }

                    // Add new tags
                    if (tagsToAdd.Any())
                    {
                        var newTags = tagsToAdd.Select(tagId => new ArticleBlogTag { Id = Guid.NewGuid(), ArticleId = existingArticle.Id, BlogTagId = tagId }).ToList();
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", newTags, transaction);
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

        public async Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto)
        {
            if (deleteArticleDto.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Article ID is invalid");
            }

            var article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = deleteArticleDto.Id });
            if (article == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = deleteArticleDto.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = deleteArticleDto.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM Articles WHERE Id = @Id", new { Id = deleteArticleDto.Id }, transaction);

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
