
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
        private readonly IBlogTagService _blogTagService;

        public ArticleService(IDbConnection dbConnection, IBlogCategoryService blogCategoryService, IBlogTagService blogTagService)
        {
            _dbConnection = dbConnection;
            _blogCategoryService = blogCategoryService;
            _blogTagService = blogTagService;
        }

        public async Task<string> CreateArticle(CreateArticleDto request)
        {
            // Step 1: Validate all fields of request.payload are not null except from [summary, body, ImageID, PDF, blogCategoryId, googleDriveId]
            if (string.IsNullOrEmpty(request.Title) || request.AuthorId == Guid.Empty || string.IsNullOrEmpty(request.Langcode) || !request.Status || !request.Sticky || !request.Promote || request.BlogCategories == null || request.BlogTags == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch author from database by id from argument ID
            var author = await _dbConnection.QueryFirstOrDefaultAsync<Author>("SELECT * FROM Authors WHERE Id = @AuthorId", new { AuthorId = request.AuthorId });
            if (author == null)
            {
                throw new BusinessException("DP-404", "Author does not exist");
            }

            // Step 4: Fetch BlogTag by using the service that implements the IblogTagService interface to fetch the corresponding tag details
            var tagsList = new List<BlogTag>();
            foreach (var tagName in request.BlogTags)
            {
                var tag = await _blogTagService.GetBlogTag(new BlogTagRequestDto { Id = tagName });
                if (tag == null)
                {
                    throw new BusinessException("DP-404", "Tag does not exist");
                }
                tagsList.Add(tag);
            }

            // Step 3: Fetch blog category by using the service that implements the IBlogCategoryService interface to fetch the corresponding blog category details
            var categoriesList = new List<BlogCategory>();
            foreach (var categoryId in request.BlogCategories)
            {
                var category = await _blogCategoryService.GetBlogCategory(new BlogCategoryRequestDto { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Blog category does not exist");
                }
                categoriesList.Add(category);
            }

            // Step 5: Create new Article object (article) as follows from arguments
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

            // Step 6: Create new list of ArticleBlogCategories objects (articleBlogCategories) as follows
            var articleBlogCategories = categoriesList.Select(category => new ArticleBlogCategory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogCategoryId = category.Id
            }).ToList();

            // Step 7: Create new list of ArticleBlogTags objects (articleBlogTags) as follows
            var articleBlogTags = tagsList.Select(tag => new ArticleBlogTag
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogTagId = tag.Id
            }).ToList();

            // Step 8: In a single SQL transaction
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    // Insert article in database table Article
                    await _dbConnection.ExecuteAsync("INSERT INTO Articles (Id, Title, AuthorId, Summary, Body, GoogleDriveID, HideScrollSpy, ImageId, PDF, Langcode, Status, Sticky, Promote, Version, Created, Changed) VALUES (@Id, @Title, @AuthorId, @Summary, @Body, @GoogleDriveID, @HideScrollSpy, @ImageId, @PDF, @Langcode, @Status, @Sticky, @Promote, @Version, @Created, @Changed)", article, transaction);

                    // Insert articleBlogCategories in database table ArticleBlogCategories
                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", articleBlogCategories, transaction);

                    // Insert articleBlogTags in database table ArticleBlogTags
                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", articleBlogTags, transaction);

                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
            }

            return article.Id.ToString();
        }

        public async Task<Article> GetArticle(ArticleRequestDto request)
        {
            // Step 1: If request.payload.id is null and request.payload.name is empty
            if (request.Id == null && string.IsNullOrEmpty(request.Title))
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }

            Article article;
            if (request.Id != null)
            {
                // Step 2: Fetch article from database by id, providing request.payload.id
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                // Step 3: Fetch article from database by name, providing request.payload.name
                article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Title = @Title", new { Title = request.Title });
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
                var category = await _blogCategoryService.GetBlogCategory(new BlogCategoryRequestDto { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Blog category not found");
                }
                blogCategories.Add(category);
            }

            // Step 5: Fetch Blogtags
            var blogTagIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
            var blogTags = await _dbConnection.QueryAsync<BlogTag>("SELECT * FROM BlogTags WHERE Id IN @BlogTagIds", new { BlogTagIds = blogTagIds });

            if (blogTags.Any(tag => tag == null))
            {
                throw new BusinessException("DP-404", "Blog tag not found");
            }

            article.BlogCategories = blogCategories.Select(bc => bc.Id).ToList();
            article.Tags = blogTags.ToList();

            return article;
        }

        public async Task<string> UpdateArticle(UpdateArticleDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Title) || request.AuthorId == Guid.Empty || string.IsNullOrEmpty(request.Langcode) || !request.Status)
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
                var pdf = await _dbConnection.QueryFirstOrDefaultAsync<Attachment>("SELECT * FROM Attachments WHERE Id = @Pdf", new { Pdf = request.PDF });
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

            // Step 7: Save Changes to Database
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
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
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
            }

            return existingArticle.Id.ToString();
        }

        public async Task<bool> DeleteArticle(DeleteArticleDto request)
        {
            // Step 1: Validate the request
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }

            // Step 2: Fetch the article to ensure it exists
            var article = await _dbConnection.QueryFirstOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            if (article == null)
            {
                throw new BusinessException("DP-404", "Article not found");
            }

            // Step 3: Delete the article in a transaction
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    // Delete from ArticleBlogCategories
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = request.Id }, transaction);

                    // Delete from ArticleBlogTags
                    await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = request.Id }, transaction);

                    // Delete from Articles
                    await _dbConnection.ExecuteAsync("DELETE FROM Articles WHERE Id = @Id", new { Id = request.Id }, transaction);

                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
            }

            return true;
        }
    }
}
