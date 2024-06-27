
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
    public class FAQService : IFAQService
    {
        private readonly IDbConnection _dbConnection;

        public FAQService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateFAQ(CreateFAQDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) ||
                string.IsNullOrEmpty(request.Langcode) || request.Status == null || string.IsNullOrEmpty(request.Order))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Validate the FAQCategoryId if it is provided
            if (request.FaqCategoryId.HasValue)
            {
                var categoryExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM FaqCategories WHERE Id = @FaqCategoryId",
                    new { FaqCategoryId = request.FaqCategoryId.Value });
                if (!categoryExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 3: Create a new FAQ object with the provided details
            var newFaq = new FAQ
            {
                Id = Guid.NewGuid(),
                Question = request.Question,
                Answer = request.Answer,
                FaqCategoryId = request.FaqCategoryId,
                Langcode = request.Langcode,
                Status = request.Status.Value,
                Order = request.Order,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 4: Insert the newly created FAQ object to the database
            var sql = @"
                INSERT INTO FAQs (Id, Question, Answer, FaqCategoryId, Langcode, Status, Created, Changed, Order)
                VALUES (@Id, @Question, @Answer, @FaqCategoryId, @Langcode, @Status, @Created, @Changed, @Order)";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, newFaq);

            // Step 5: Check if the transaction is successful
            if (rowsAffected > 0)
            {
                return newFaq.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<FAQ> GetFAQ(FAQRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the FAQ from the database based on the provided FAQ ID
            var sql = "SELECT * FROM FAQs WHERE Id = @Id";
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>(sql, new { Id = request.Id });

            // Step 3: Check if the FAQ exists
            if (faq != null)
            {
                return faq;
            }
            else
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateFAQ(UpdateFAQDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) ||
                string.IsNullOrEmpty(request.Langcode) || request.Status == null || string.IsNullOrEmpty(request.Order))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing FAQ object from the database based on the provided FAQ ID
            var existingFaq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>(
                "SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (existingFaq == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Validate the FAQCategoryId if it is provided
            if (request.FaqCategoryId.HasValue)
            {
                var categoryExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM FaqCategories WHERE Id = @FaqCategoryId",
                    new { FaqCategoryId = request.FaqCategoryId.Value });
                if (!categoryExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 4: Update the FAQ object with the provided changes
            existingFaq.Question = request.Question;
            existingFaq.Answer = request.Answer;
            existingFaq.FaqCategoryId = request.FaqCategoryId;
            existingFaq.Langcode = request.Langcode;
            existingFaq.Status = request.Status.Value;
            existingFaq.Order = request.Order;
            existingFaq.Changed = DateTime.UtcNow;

            // Step 5: Insert the updated FAQ object back to the database
            var sql = @"
                UPDATE FAQs 
                SET Question = @Question, Answer = @Answer, FaqCategoryId = @FaqCategoryId, Langcode = @Langcode, 
                    Status = @Status, Order = @Order, Changed = @Changed
                WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, existingFaq);

            // Step 6: Check if the transaction is successful
            if (rowsAffected > 0)
            {
                return existingFaq.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteFAQ(DeleteFAQDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing FAQ object from the database based on the provided FAQ ID
            var existingFaq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>(
                "SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (existingFaq == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the FAQ object from the database
            var sql = "DELETE FROM FAQs WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });

            // Step 4: Check if the transaction is successful
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<FAQ>> GetListFAQ(ListFAQRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of FAQs from the database based on the provided pagination parameters and optional sorting
            var sql = @"
                SELECT * FROM FAQs
                ORDER BY 
                    CASE WHEN @SortField = 'Created' AND @SortOrder = 'ASC' THEN Created END ASC,
                    CASE WHEN @SortField = 'Created' AND @SortOrder = 'DESC' THEN Created END DESC
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var faqs = await _dbConnection.QueryAsync<FAQ>(sql, new
            {
                PageLimit = request.PageLimit,
                PageOffset = request.PageOffset,
                SortField = request.SortField,
                SortOrder = request.SortOrder
            });

            // Step 3: Check if the transaction is successful
            if (faqs != null)
            {
                return faqs.ToList();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
