
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
            var reportedByUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 3: Fetch user from the database by id from argument AssignedTo.
            var assignedToUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            // Step 4: Fetch environment from the database by id from argument EnvironmentImpacted.
            var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = request.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-422", "Environment not found.");
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId.
            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found.");
            }

            // Step 6: Foreach item in SupportCategories argument.
            var supportCategories = new List<SupportCategory>();
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var category = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (category == null)
                    {
                        throw new BusinessException("DP-422", "Support category not found.");
                    }
                    supportCategories.Add(category);
                }
            }

            // Step 7: Create a new SupportTicket object.
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

            // Step 8: Create a new list of SupportTicketCategories objects.
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

            // Step 9: Create a new list of SupportTicketEnvironments objects.
            var supportTicketEnvironments = new List<SupportTicketEnvironment>
            {
                new SupportTicketEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    EnvironmentId = request.EnvironmentImpacted
                }
            };

            // Step 10: Create a new SupportTicketSeverities object.
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeverityId = request.SeverityId
                }
            };

            // Step 11: In a single SQL transaction.
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert supportTicket in the database table SupportTicket.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTickets (Id, ReportedBy, AssignedTo, ContactDetails, EnvironmentImpacted, NameOfReportingOrganization, Priority, SeverityId, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssignedTo, @ContactDetails, @EnvironmentImpacted, @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

                    // Insert supportTicketCategories in the database table SupportTicketCategories.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);

                    // Insert supportTicketEnvironments in the database table SupportTicketEnvironments.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentId) VALUES (@Id, @SupportTicketId, @EnvironmentId)", supportTicketEnvironments, transaction);

                    // Insert supportTicketSeverities in the database table SupportTicketSeverities.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeverityId) VALUES (@Id, @SupportTicketId, @SeverityId)", supportTicketSeverities, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            // Step 12: Return SupportTicket.Id from the database.
            return supportTicket.Id.ToString();
        }

        public async Task<SupportTicket> GetSupportTicket(RequestSupportTicketDTO request)
        {
            // Step 1: If request.payload.id is null.
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Id is null.");
            }

            // Step 2: Fetch support ticket from the database by id, providing request.payload.id.
            var supportTicket = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTickets WHERE Id = @Id", new { Id = request.Id });
            if (supportTicket == null)
            {
                throw new BusinessException("DP-404", "Support ticket not found.");
            }

            // Step 3: Fetch related data.
            var reportedByUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            var assignedToUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found.");
            }

            var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = supportTicket.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-404", "Environment not found.");
            }

            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = supportTicket.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-404", "Severity not found.");
            }

            var supportCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id IN @Ids", new { Ids = supportCategoryIds });
            if (supportCategories.Any(sc => sc == null))
            {
                throw new BusinessException("DP-404", "Support category not found.");
            }

            var messageIds = await _dbConnection.QueryAsync<Guid>("SELECT Id FROM Messages WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var messages = await _dbConnection.QueryAsync<Message>("SELECT * FROM Messages WHERE Id IN @Ids", new { Ids = messageIds });
            if (messages.Any(m => m == null))
            {
                throw new BusinessException("DP-404", "Message not found.");
            }

            // Step 4: Map the database object to SupportTicket and return the SupportTicket.
            supportTicket.SupportCategories = supportCategories.ToList();
            supportTicket.Messages = messages.ToList();
            return supportTicket;
        }

        public async Task<string> UpdateSupportTicket(UpdateSupportTicketDTO request)
        {
            // Step 1: Validate Necessary Parameters.
            if (request.Id == Guid.Empty || request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.EnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State))
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null.");
            }

            // Step 2: Fetch Existing Support Ticket.
            var existingTicket = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTickets WHERE Id = @Id", new { Id = request.Id });
            if (existingTicket == null)
            {
                throw new BusinessException("DP-404", "Support ticket not found.");
            }

            // Step 3: Validate Related Entities.
            var reportedByUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-422", "User not found.");
            }

            var assignedToUser = await _dbConnection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-422", "User not found.");
            }

            var environment = await _dbConnection.QuerySingleOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = request.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-422", "Environment not found.");
            }

            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found.");
            }

            // Step 4: Update the SupportTicket object with the provided changes.
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
            existingTicket.Version++;
            existingTicket.Changed = DateTime.UtcNow;
            existingTicket.ChangedUser = request.ChangedUser;

            // Step 5: Update SupportTicketCategories.
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = request.Id });
            var categoriesToRemove = existingCategories.Except(request.SupportCategories or new List<Guid>()).ToList();
            var categoriesToAdd = (request.SupportCategories or new List<Guid>()).Except(existingCategories).ToList();

            if (categoriesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketCategories WHERE SupportTicketId = @SupportTicketId AND SupportCategoryId IN @SupportCategoryIds", new { SupportTicketId = request.Id, SupportCategoryIds = categoriesToRemove });
            }

            var newCategories = categoriesToAdd.Select(categoryId => new SupportTicketCategory
            {
                Id = Guid.NewGuid(),
                SupportTicketId = request.Id,
                SupportCategoryId = categoryId
            }).ToList();

            if (newCategories.Any())
            {
                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", newCategories);
            }

            // Step 6: Update SupportTicketEnvironments.
            var existingEnvironments = await _dbConnection.QueryAsync<Guid>("SELECT EnvironmentId FROM SupportTicketEnvironments WHERE SupportTicketId = @Id", new { Id = request.Id });
            var environmentsToRemove = existingEnvironments.Except(new List<Guid> { request.EnvironmentImpacted }).ToList();
            var environmentsToAdd = new List<Guid> { request.EnvironmentImpacted }.Except(existingEnvironments).ToList();

            if (environmentsToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketEnvironments WHERE SupportTicketId = @SupportTicketId AND EnvironmentId IN @EnvironmentIds", new { SupportTicketId = request.Id, EnvironmentIds = environmentsToRemove });
            }

            var newEnvironments = environmentsToAdd.Select(environmentId => new SupportTicketEnvironment
            {
                Id = Guid.NewGuid(),
                SupportTicketId = request.Id,
                EnvironmentId = environmentId
            }).ToList();

            if (newEnvironments.Any())
            {
                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentId) VALUES (@Id, @SupportTicketId, @EnvironmentId)", newEnvironments);
            }

            // Step 7: Update SupportTicketSeverities.
            var existingSeverities = await _dbConnection.QueryAsync<Guid>("SELECT SeverityId FROM SupportTicketSeverities WHERE SupportTicketId = @Id", new { Id = request.Id });
            var severitiesToRemove = existingSeverities.Except(new List<Guid> { request.SeverityId }).ToList();
            var severitiesToAdd = new List<Guid> { request.SeverityId }.Except(existingSeverities).ToList();

            if (severitiesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketSeverities WHERE SupportTicketId = @SupportTicketId AND SeverityId IN @SeverityIds", new { SupportTicketId = request.Id, SeverityIds = severitiesToRemove });
            }

            var newSeverities = severitiesToAdd.Select(severityId => new SupportTicketSeverity
            {
                Id = Guid.NewGuid(),
                SupportTicketId = request.Id,
                SeverityId = severityId
            }).ToList();

            if (newSeverities.Any())
            {
                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeverityId) VALUES (@Id, @SupportTicketId, @SeverityId)", newSeverities);
            }

            // Step 8: Return Success Message.
            return "Support ticket updated successfully.";
        }
    }
}
