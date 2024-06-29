
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
    public class ContactService : IContactService
    {
        private readonly IDbConnection _dbConnection;

        public ContactService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateContact(CreateContactDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Mail) ||
                string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Message))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new Contact object
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Mail = request.Mail,
                Subject = request.Subject,
                Message = request.Message,
                Created = DateTime.UtcNow
            };

            // Step 3: Insert the newly created Contact object into the database
            const string sql = @"
                INSERT INTO Contacts (Id, Name, Mail, Subject, Message, Created)
                VALUES (@Id, @Name, @Mail, @Subject, @Message, @Created)";

            try
            {
                await _dbConnection.ExecuteAsync(sql, contact);
                return contact.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<Contact> GetContact(ContactRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve the Contact object from the database using the provided Id
            const string sql = "SELECT * FROM Contacts WHERE Id = @Id";

            try
            {
                var contact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(sql, new { request.Id });
                if (contact == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return contact;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdateContact(UpdateContactDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Mail) ||
                string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Message))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve the existing Contact object from the database using the provided Id
            const string selectSql = "SELECT * FROM Contacts WHERE Id = @Id";
            var existingContact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(selectSql, new { request.Id });

            if (existingContact == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the Contact object with the new details
            existingContact.Name = request.Name;
            existingContact.Mail = request.Mail;
            existingContact.Subject = request.Subject;
            existingContact.Message = request.Message;
            existingContact.Created = request.Created;

            // Step 4: Save the updated Contact object to the database
            const string updateSql = @"
                UPDATE Contacts
                SET Name = @Name, Mail = @Mail, Subject = @Subject, Message = @Message, Created = @Created
                WHERE Id = @Id";

            try
            {
                await _dbConnection.ExecuteAsync(updateSql, existingContact);
                return existingContact.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteContact(DeleteContactDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve the existing Contact object from the database using the provided Id
            const string selectSql = "SELECT * FROM Contacts WHERE Id = @Id";
            var existingContact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(selectSql, new { request.Id });

            if (existingContact == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the Contact object from the database
            const string deleteSql = "DELETE FROM Contacts WHERE Id = @Id";

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

        public async Task<List<Contact>> GetListContact(ListContactRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Query the database to fetch the list of contacts
            var sql = "SELECT * FROM Contacts";

            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }

            sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var contacts = await _dbConnection.QueryAsync<Contact>(sql, new { request.PageOffset, request.PageLimit });
                return contacts.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
