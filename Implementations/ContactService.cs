
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

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

            // Step 2: Create a new Contact object with the provided details
            var contact = new Contact
            {
                Name = request.Name,
                Mail = request.Mail,
                Subject = request.Subject,
                Message = request.Message,
                Created = DateTime.UtcNow
            };

            // Step 3: Insert the newly created Contact object to the database
            const string sql = @"
                INSERT INTO Contacts (Id, Name, Mail, Subject, Message, Created)
                VALUES (@Id, @Name, @Mail, @Subject, @Message, @Created);
                SELECT CAST(SCOPE_IDENTITY() as varchar(50));";

            try
            {
                var id = await _dbConnection.QuerySingleAsync<string>(sql, contact);

                // Step 4: If the transaction is successful
                return id;
            }
            catch (Exception)
            {
                // Step 4.2: If the transaction fails
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
