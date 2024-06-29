
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IDbConnection _dbConnection;

        public AttachmentService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAttachment(CreateAttachmentDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.FileName) || request.FileUrl == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new Attachment object
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                Timestamp = DateTime.UtcNow
            };

            // Step 3: Save the newly created Attachment object to the database
            const string sql = "INSERT INTO Attachments (Id, FileName, FileUrl, Timestamp) VALUES (@Id, @FileName, @FileUrl, @Timestamp)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, attachment);

            if (affectedRows > 0)
            {
                return attachment.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<Attachment> GetAttachment(AttachmentRequestDto request)
        {
            // Step 1: Validate that request.payload.Id is not null
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Attachment from the database based on the provided file ID
            const string sql = "SELECT * FROM Attachments WHERE Id = @Id";
            var attachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(sql, new { Id = request.Id });

            if (attachment != null)
            {
                return attachment;
            }
            else
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        public async Task<string> UpdateAttachment(UpdateAttachmentDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters
            if (request.Id == null || string.IsNullOrEmpty(request.FileName) || request.FileUrl == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Attachment from the database by Id
            const string selectSql = "SELECT * FROM Attachments WHERE Id = @Id";
            var existingAttachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(selectSql, new { Id = request.Id });

            if (existingAttachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the Attachment object with the provided changes
            existingAttachment.FileName = request.FileName;
            existingAttachment.FileUrl = request.FileUrl;
            existingAttachment.Timestamp = DateTime.UtcNow;

            // Step 4: Insert the updated Attachment object to the database
            const string updateSql = "UPDATE Attachments SET FileName = @FileName, FileUrl = @FileUrl, Timestamp = @Timestamp WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, existingAttachment);

            if (affectedRows > 0)
            {
                return existingAttachment.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteAttachment(DeleteAttachmentDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the Attachment from the database by Id
            const string selectSql = "SELECT * FROM Attachments WHERE Id = @Id";
            var existingAttachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(selectSql, new { Id = request.Id });

            if (existingAttachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the Attachment object from the database
            const string deleteSql = "DELETE FROM Attachments WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id });

            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<Attachment>> GetListAttachment(ListAttachmentRequestDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameters
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the list of Attachments from the database based on the provided pagination parameters
            var sql = $"SELECT * FROM Attachments ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";
            var attachments = await _dbConnection.QueryAsync<Attachment>(sql);

            if (attachments != null)
            {
                return attachments.AsList();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
