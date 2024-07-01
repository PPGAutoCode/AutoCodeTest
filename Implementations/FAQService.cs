
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
                string.IsNullOrEmpty(request.Langcode) || request.Order == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Validate the FAQCategoryId if it is provided
            if (request.FaqCategoryId.HasValue)
            {
                var categoryExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM FAQCategories WHERE Id = @FaqCategoryId",
                    new { FaqCategoryId = request.FaqCategoryId.Value });
                if (!categoryExists)
                {
                    throw new BusinessException("DP-404", "Technical Error");
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
                Status = request.Status,
                Order = request.Order,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 4: Insert the newly created FAQ object to the database
            const string insertQuery = @"
                INSERT INTO FAQs (Id, Question, Answer, FaqCategoryId, Langcode, Status, Created, Changed, [Order])
                VALUES (@Id, @Question, @Answer, @FaqCategoryId, @Langcode, @Status, @Created, @Changed, @Order)";

            try
            {
                await _dbConnection.ExecuteAsync(insertQuery, newFaq);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 5: Return the ID of the newly created FAQ
            return newFaq.Id.ToString();
        }

        public async Task<FAQ> GetFAQ(FAQRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter (Id)
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the FAQ from the database based on the provided FAQ ID
            const string selectQuery = "SELECT * FROM FAQs WHERE Id = @Id";
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>(selectQuery, new { Id = request.Id });

            // Step 3: Return the FAQ or throw an exception if not found
            if (faq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            return faq;
        }

        public async Task<string> UpdateFAQ(UpdateFAQDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) ||
                string.IsNullOrEmpty(request.Langcode) || request.Order == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing FAQ object from the database based on the provided FAQ ID
            var existingFaq = await GetFAQ(new FAQRequestDto { Id = request.Id });
            if (existingFaq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Validate the FAQCategoryId if it is provided
            if (request.FaqCategoryId.HasValue)
            {
                var categoryExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM FAQCategories WHERE Id = @FaqCategoryId",
                    new { FaqCategoryId = request.FaqCategoryId.Value });
                if (!categoryExists)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
            }

            // Step 4: Update the FAQ object with the provided changes
            existingFaq.Question = request.Question;
            existingFaq.Answer = request.Answer;
            existingFaq.FaqCategoryId = request.FaqCategoryId;
            existingFaq.Langcode = request.Langcode;
            existingFaq.Status = request.Status;
            existingFaq.Order = request.Order;
            existingFaq.Changed = DateTime.UtcNow;

            // Step 5: Insert the updated FAQ object back to the database
            const string updateQuery = @"
                UPDATE FAQs 
                SET Question = @Question, Answer = @Answer, FaqCategoryId = @FaqCategoryId, Langcode = @Langcode, 
                    Status = @Status, [Order] = @Order, Changed = @Changed
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateQuery, existingFaq);
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 6: Return the ID of the updated FAQ
            return existingFaq.Id.ToString();
        }

        public async Task<bool> DeleteFAQ(DeleteFAQDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter (Id)
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing FAQ object from the database based on the provided FAQ ID
            var existingFaq = await GetFAQ(new FAQRequestDto { Id = request.Id });
            if (existingFaq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Delete the FAQ object from the database
            const string deleteQuery = "DELETE FROM FAQs WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(deleteQuery, new { Id = request.Id });
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            // Step 4: Return true if the deletion was successful
            return true;
        }

        public async Task<List<FAQ>> GetListFAQ(ListFAQRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary pagination parameters
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of FAQs from the database based on the provided pagination parameters
            var query = "SELECT * FROM FAQs";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                query += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            query += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var faqs = await _dbConnection.QueryAsync<FAQ>(query, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });
                return faqs.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
