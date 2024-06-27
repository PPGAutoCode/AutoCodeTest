
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
    public class SupportTicketService : ISupportTicketService
    {
        private readonly IDbConnection _dbConnection;

        public SupportTicketService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportTicket(CreateSupportTicketDTO request)
        {
            // Step 1: Validate all fields of request.payload are not null except for Priority and SupportCategories.
            if (request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.EnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State) ||
                request.Message == null)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null.");
            }

            // Step 2: Fetch user from the database by id from argument ReportedBy.
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 3: Fetch user from the database by id from argument AssignedTo.
            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 4: Fetch environment from the database by id from argument EnvironmentImpacted.
            var environment = await _dbConnection.QueryFirstOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = request.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-422", "Environment not found.");
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId.
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found.");
            }

            // Step 6: Foreach item in SupportCategories argument:
            var supportCategories = new List<SupportCategory>();
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var category = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (category == null)
                    {
                        throw new BusinessException("DP-422", "Support category not found.");
                    }
                    supportCategories.Add(category);
                }
            }

            // Step 7: Create a new SupportTicket object (supportTicket) as follows from arguments:
            var supportTicket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                ReportedBy = request.ReportedBy,
                AssignedTo = request.AssignedTo,
                ContactDetails = request.ContactDetails,
                EnvironmentImpacted = request.EnvironmentImpacted,
                NameOfReportingOrganization = request.NameOfReportingOrganization,
                Priority = request.Priority,
                SeverityId = request.SeverityId,
                ShortDescription = request.ShortDescription,
                State = request.State,
                Message = request.Message,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.ReportedBy,
                ChangedUser = request.ReportedBy
            };

            // Step 24: Create a new list of SupportTicketCategories objects (supportTicketCategories) as follows:
            var supportTicketCategories = new List<SupportTicketCategory>();
            foreach (var categoryId in request.SupportCategories)
            {
                supportTicketCategories.Add(new SupportTicketCategory
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SupportCategoryId = categoryId
                });
            }

            // Step 26: Create a new list of SupportTicketEnvironments objects (supportTicketEnvironments) as follows:
            var supportTicketEnvironments = new List<SupportTicketEnvironments>
            {
                new SupportTicketEnvironments
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    EnvironmentsId = request.EnvironmentImpacted
                }
            };

            // Step 28: Create a new SupportTicketSeverities object (supportTicketSeverities) as follows:
            var supportTicketSeverities = new List<SupportTicketSeverities>
            {
                new SupportTicketSeverities
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeveritiesId = request.SeverityId
                }
            };

            // Step 30: In a single SQL transaction:
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Step 31: Insert supportTicket in the database table SupportTicket.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicket (Id, ReportedBy, AssignedTo, ContactDetails, EnvironmentImpacted, NameOfReportingOrganization, Priority, SeverityId, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssignedTo, @ContactDetails, @EnvironmentImpacted, @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

                    // Step 32: Insert supportTicketCategories in the database table SupportTicketCategories.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);

                    // Step 33: Insert supportTicketEnvironments in the database table SupportTicketEnvironments.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentsId) VALUES (@Id, @SupportTicketId, @EnvironmentsId)", supportTicketEnvironments, transaction);

                    // Step 34: Insert supportTicketSeverities in the database table SupportTicketSeverities.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiesId) VALUES (@Id, @SupportTicketId, @SeveritiesId)", supportTicketSeverities, transaction);

                    // Step 35: Commit the transaction.
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            // Step 37: Return SupportTicket.Id from the database.
            return supportTicket.Id.ToString();
        }

        public async Task<SupportTicket> GetSupportTicket(RequestSupportTicketDTO request)
        {
            // Step 1: If request.payload.id is null:
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Id is null.");
            }

            // Step 2: Fetch support ticket from the database by id, providing request.payload.id.
            var supportTicket = await _dbConnection.QueryFirstOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTicket WHERE Id = @Id", new { Id = request.Id });
            if (supportTicket == null)
            {
                throw new BusinessException("DP-404", "Support ticket not found.");
            }

            // Step 4: Fetch the user who reported the ticket by ReportedBy.
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 5: Fetch the user assigned to the ticket by AssignedTo.
            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 6: Fetch the environment impacted by the ticket by EnvironmentImpacted.
            var environment = await _dbConnection.QueryFirstOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = supportTicket.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-404", "Environment not found.");
            }

            // Step 7: Fetch the severity level by SeverityId.
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = supportTicket.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-404", "Severity not found.");
            }

            // Step 9: Fetch support categories:
            var supportCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id IN @Ids", new { Ids = supportCategoryIds });
            if (supportCategories.Count() != supportCategoryIds.Count())
            {
                throw new BusinessException("DP-404", "Support category not found.");
            }

            // Step 11: Fetch messages associated with the support ticket:
            var messageIds = await _dbConnection.QueryAsync<Guid>("SELECT Id FROM Messages WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var messages = await _dbConnection.QueryAsync<Message>("SELECT * FROM Messages WHERE Id IN @Ids", new { Ids = messageIds });
            if (messages.Count() != messageIds.Count())
            {
                throw new BusinessException("DP-404", "Message not found.");
            }

            // Step 12: Map the database object to SupportTicket and return the SupportTicket.
            supportTicket.SupportCategories = supportCategories.ToList();
            supportTicket.Messages = messages.ToList();
            return supportTicket;
        }

        public async Task<string> UpdateSupportTicket(UpdateSupportTicketDTO request)
        {
            // Step 1: Validate Necessary Parameters:
            if (request.Id == Guid.Empty || request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.EnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State))
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null.");
            }

            // Step 2: Fetch Existing Support Ticket:
            var existingTicket = await _dbConnection.QueryFirstOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTicket WHERE Id = @Id", new { Id = request.Id });
            if (existingTicket == null)
            {
                throw new BusinessException("DP-404", "Support ticket not found.");
            }

            // Step 3: Validate Related Entities:
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-422", "User not found.");
            }

            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-422", "User not found.");
            }

            var environment = await _dbConnection.QueryFirstOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = request.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-422", "Environment not found.");
            }

            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found.");
            }

            // Step 4: Update the SupportTicket object with the provided changes:
            existingTicket.ReportedBy = request.ReportedBy;
            existingTicket.AssignedTo = request.AssignedTo;
            existingTicket.ContactDetails = request.ContactDetails;
            existingTicket.DateClosed = request.DateClosed;
            existingTicket.EnvironmentImpacted = request.EnvironmentImpacted;
            existingTicket.NameOfReportingOrganization = request.NameOfReportingOrganization;
            existingTicket.Priority = request.Priority;
            existingTicket.SeverityId = request.SeverityId;
            existingTicket.ShortDescription = request.ShortDescription;
            existingTicket.State = request.State;
            existingTicket.SupportCategories = request.SupportCategories;
            existingTicket.Messages = request.Messages;
            existingTicket.Version += 1;
            existingTicket.Changed = DateTime.UtcNow;
            existingTicket.ChangedUser = request.ChangedUser;

            // Step 5: Update SupportTicketCategories:
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = existingTicket.Id });
            var categoriesToRemove = existingCategories.Except(request.SupportCategories or []).ToList()
            var categoriesToAdd = (request.SupportCategories or []).Except(existingCategories).ToList()

            // Step 6: Update SupportTicketEnvironments:
            var existingEnvironments = await _dbConnection.QueryAsync<Guid>("SELECT EnvironmentsId FROM SupportTicketEnvironments WHERE SupportTicketId = @Id", new { Id = existingTicket.Id });
            var environmentsToRemove = existingEnvironments.Except([request.EnvironmentImpacted]).ToList()
            var environmentsToAdd = [request.EnvironmentImpacted].Except(existingEnvironments).ToList()

            // Step 7: Update SupportTicketSeverities:
            var existingSeverities = await _dbConnection.QueryAsync<Guid>("SELECT SeveritiesId FROM SupportTicketSeverities WHERE SupportTicketId = @Id", new { Id = existingTicket.Id });
            var severitiesToRemove = existingSeverities.Except([request.SeverityId]).ToList()
            var severitiesToAdd = [request.SeverityId].Except(existingSeverities).ToList()

            // Step 8: Save Changes to Database:
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Update SupportTicket Table:
                    await _dbConnection.ExecuteAsync("UPDATE SupportTicket SET ReportedBy = @ReportedBy, AssignedTo = @AssignedTo, ContactDetails = @ContactDetails, DateClosed = @DateClosed, EnvironmentImpacted = @EnvironmentImpacted, NameOfReportingOrganization = @NameOfReportingOrganization, Priority = @Priority, SeverityId = @SeverityId, ShortDescription = @ShortDescription, State = @State, SupportCategories = @SupportCategories, Messages = @Messages, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser WHERE Id = @Id", existingTicket, transaction);

                    // Update SupportTicketCategories Table:
                    if (categoriesToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketCategories WHERE SupportTicketId = @Id AND SupportCategoryId IN @Ids", new { Id = existingTicket.Id, Ids = categoriesToRemove }, transaction);
                    }
                    for categoryId in categoriesToAdd:
                        await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", new { Id = Guid.NewGuid(), SupportTicketId = existingTicket.Id, SupportCategoryId = categoryId }, transaction);

                    // Update SupportTicketEnvironments Table:
                    if environmentsToRemove:
                        await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketEnvironments WHERE SupportTicketId = @Id AND EnvironmentsId IN @Ids", new { Id = existingTicket.Id, Ids = environmentsToRemove }, transaction);
                    for environmentId in environmentsToAdd:
                        await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentsId) VALUES (@Id, @SupportTicketId, @EnvironmentsId)", new { Id = Guid.NewGuid(), SupportTicketId = existingTicket.Id, EnvironmentsId = environmentId }, transaction);

                    // Update SupportTicketSeverities Table:
                    if severitiesToRemove:
                        await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketSeverities WHERE SupportTicketId = @Id AND SeveritiesId IN @Ids", new { Id = existingTicket.Id, Ids = severitiesToRemove }, transaction);
                    for severityId in severitiesToAdd:
                        await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiesId) VALUES (@Id, @SupportTicketId, @SeveritiesId)", new { Id = Guid.NewGuid(), SupportTicketId = existingTicket.Id, SeveritiesId = severityId }, transaction);

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            // Return SupportTicket.Id from the database.
            return existingTicket.Id.ToString();
        }
    }
}
