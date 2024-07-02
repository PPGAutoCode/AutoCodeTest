
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

        public async Task<string> CreateSupportTicket(CreateSupportTicketDto request)
        {
            // Step 1: Validate all fields of request.payload are not null except for Priority and SupportCategories
            if (request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.AppEnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State) ||
                request.Message == null || !request.Message.Any())
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch user from the database by id from argument ReportedBy
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found");
            }

            // Step 3: Fetch user from the database by id from argument AssignedTo
            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found");
            }

            // Step 4: Fetch appenvironment from the database by id from argument AppEnvironmentImpacted
            var appEnvironment = await _dbConnection.QueryFirstOrDefaultAsync<AppEnvironment>("SELECT * FROM AppEnvironments WHERE Id = @Id", new { Id = request.AppEnvironmentImpacted });
            if (appEnvironment == null)
            {
                throw new BusinessException("DP-422", "AppEnvironment not found");
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found");
            }

            // Step 6: Foreach item in SupportCategories argument
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var supportCategory = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (supportCategory == null)
                    {
                        throw new BusinessException("DP-422", "SupportCategory not found");
                    }
                }
            }

            // Step 7: Create a new SupportTicket object
            var supportTicket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                ReportedBy = request.ReportedBy,
                AssignedTo = request.AssignedTo,
                ContactDetails = request.ContactDetails,
                AppEnvironmentImpacted = request.AppEnvironmentImpacted,
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

            // Step 8: Create a new list of SupportTicketCategories objects
            var supportTicketCategories = new List<SupportTicketCategory>();
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    supportTicketCategories.Add(new SupportTicketCategory
                    {
                        Id = Guid.NewGuid(),
                        SupportTicketId = supportTicket.Id,
                        SupportCategoryId = categoryId
                    });
                }
            }

            // Step 9: Create a new list of SupportTicketAppEnvironments objects
            var supportTicketAppEnvironments = new List<SupportTicketAppEnvironment>
            {
                new SupportTicketAppEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    AppEnvironmentsId = request.AppEnvironmentImpacted
                }
            };

            // Step 10: Create a new List of SupportTicketSeverities object
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeveritiyId = request.SeverityId
                }
            };

            // Step 11: In a single SQL transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert supportTicket in the database table SupportTicket
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTickets (Id, ReportedBy, AssignedTo, ContactDetails, AppEnvironmentImpacted, NameOfReportingOrganization, Priority, SeverityId, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssignedTo, @ContactDetails, @AppEnvironmentImpacted, @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

                    // Insert supportTicketCategories in the database table SupportTicketCategories
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);

                    // Insert supportTicketAppEnvironments in the database table SupportTicketAppEnvironments
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketAppEnvironments (Id, SupportTicketId, AppEnvironmentsId) VALUES (@Id, @SupportTicketId, @AppEnvironmentsId)", supportTicketAppEnvironments, transaction);

                    // Insert supportTicketSeverities in the database table SupportTicketSeverities
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiyId) VALUES (@Id, @SupportTicketId, @SeveritiyId)", supportTicketSeverities, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            // Step 12: Return SupportTicket.Id from the database
            return supportTicket.Id.ToString();
        }

        public async Task<SupportTicket> GetSupportTicket(RequestSupportTicketDto request)
        {
            // Step 1: If request.payload.id is null
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Id is null");
            }

            // Step 2: Fetch support ticket from the database by id, providing request.payload.id
            var supportTicket = await _dbConnection.QueryFirstOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTickets WHERE Id = @Id", new { Id = request.Id });
            if (supportTicket == null)
            {
                throw new BusinessException("DP-404", "SupportTicket not found");
            }

            // Step 3: Fetch related data
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "User not found");
            }

            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-404", "User not found");
            }

            var appEnvironment = await _dbConnection.QueryFirstOrDefaultAsync<AppEnvironment>("SELECT * FROM AppEnvironments WHERE Id = @Id", new { Id = supportTicket.AppEnvironmentImpacted });
            if (appEnvironment == null)
            {
                throw new BusinessException("DP-404", "AppEnvironment not found");
            }

            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = supportTicket.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-404", "Severity not found");
            }

            // Step 4: Fetch support categories
            var supportCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id IN @Ids", new { Ids = supportCategoryIds });
            if (supportCategories == null || supportCategories.Count() != supportCategoryIds.Count())
            {
                throw new BusinessException("DP-404", "SupportCategory not found");
            }

            // Step 5: Fetch messages associated with the support ticket
            var messageIds = supportTicket.Message;
            var messages = await _dbConnection.QueryAsync<Message>("SELECT * FROM Messages WHERE Id IN @Ids", new { Ids = messageIds });
            if (messages == null || messages.Count() != messageIds.Count())
            {
                throw new BusinessException("DP-404", "Message not found");
            }

            // Step 6: Map the database object to SupportTicket and return the SupportTicket
            supportTicket.SupportCategories = supportCategories.ToList();
            supportTicket.Message = messages.ToList();

            return supportTicket;
        }

        public async Task<string> UpdateSupportTicket(UpdateSupportTicketDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.AppEnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State))
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null");
            }

            // Step 2: Fetch Existing Support Ticket
            var existingSupportTicket = await _dbConnection.QueryFirstOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTickets WHERE Id = @Id", new { Id = request.Id });
            if (existingSupportTicket == null)
            {
                throw new BusinessException("DP-404", "SupportTicket not found");
            }

            // Step 3: Validate Related Entities
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-422", "User not found");
            }

            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new BusinessException("DP-422", "User not found");
            }

            var appEnvironment = await _dbConnection.QueryFirstOrDefaultAsync<AppEnvironment>("SELECT * FROM AppEnvironments WHERE Id = @Id", new { Id = request.AppEnvironmentImpacted });
            if (appEnvironment == null)
            {
                throw new BusinessException("DP-422", "AppEnvironment not found");
            }

            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Severity not found");
            }

            // Step 4: Update the SupportTicket object with the provided changes
            existingSupportTicket.ReportedBy = request.ReportedBy;
            existingSupportTicket.AssignedTo = request.AssignedTo;
            existingSupportTicket.ContactDetails = request.ContactDetails;
            existingSupportTicket.DateClosed = request.DateClosed;
            existingSupportTicket.AppEnvironmentImpacted = request.AppEnvironmentImpacted;
            existingSupportTicket.NameOfReportingOrganization = request.NameOfReportingOrganization;
            existingSupportTicket.Priority = request.Priority;
            existingSupportTicket.SeverityId = request.SeverityId;
            existingSupportTicket.ShortDescription = request.ShortDescription;
            existingSupportTicket.State = request.State;
            existingSupportTicket.SupportCategories = request.SupportCategories;
            existingSupportTicket.Message = request.Message;
            existingSupportTicket.Version += 1;
            existingSupportTicket.Changed = DateTime.UtcNow;
            existingSupportTicket.ChangedUser = request.ChangedUser;

            // Step 5: Update SupportTicketCategories
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var categoriesToRemove = existingCategories.Except(request.SupportCategories ?? new List<Guid>()).ToList();
            var categoriesToAdd = (request.SupportCategories ?? new List<Guid>()).Except(existingCategories).ToList();

            if (categoriesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketCategories WHERE SupportTicketId = @SupportTicketId AND SupportCategoryId IN @Ids", new { SupportTicketId = existingSupportTicket.Id, Ids = categoriesToRemove });
            }

            if (categoriesToAdd.Any())
            {
                var newCategories = categoriesToAdd.Select(categoryId => new SupportTicketCategory
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = existingSupportTicket.Id,
                    SupportCategoryId = categoryId
                }).ToList();

                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", newCategories);
            }

            // Step 6: Update SupportTicketAppEnvironments
            var existingAppEnvironments = await _dbConnection.QueryAsync<Guid>("SELECT AppEnvironmentsId FROM SupportTicketAppEnvironments WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var appEnvironmentsToRemove = existingAppEnvironments.Except(new List<Guid> { request.AppEnvironmentImpacted }).ToList();
            var appEnvironmentsToAdd = new List<Guid> { request.AppEnvironmentImpacted }.Except(existingAppEnvironments).tolist();

            if (appEnvironmentsToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketAppEnvironments WHERE SupportTicketId = @SupportTicketId AND AppEnvironmentsId IN @Ids", new { SupportTicketId = existingSupportTicket.Id, Ids = appEnvironmentsToRemove });
            }

            if (appEnvironmentsToAdd.Any())
            {
                var newAppEnvironments = appEnvironmentsToAdd.Select(appEnvironmentId => new SupportTicketAppEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = existingSupportTicket.Id,
                    AppEnvironmentsId = appEnvironmentId
                }).ToList();

                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketAppEnvironments (Id, SupportTicketId, AppEnvironmentsId) VALUES (@Id, @SupportTicketId, @AppEnvironmentsId)", newAppEnvironments);
            }

            // Step 7: Update SupportTicketSeverities
            var existingSeverities = await _dbConnection.QueryAsync<Guid>("SELECT SeveritiyId FROM SupportTicketSeverities WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var severitiesToRemove = existingSeverities.Except(new List<Guid> { request.SeverityId }).ToList();
            var severitiesToAdd = new List<Guid> { request.SeverityId }.Except(existingSeverities).ToList();

            if (severitiesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketSeverities WHERE SupportTicketId = @SupportTicketId AND SeveritiyId IN @Ids", new { SupportTicketId = existingSupportTicket.Id, Ids = severitiesToRemove });
            }

            if (severitiesToAdd.Any())
            {
                var newSeverities = severitiesToAdd.Select(severityId => new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = existingSupportTicket.Id,
                    SeveritiyId = severityId
                }).ToList();

                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiyId) VALUES (@Id, @SupportTicketId, @SeveritiyId)", newSeverities);
            }

            return existingSupportTicket.Id.ToString();
        }
    }
}
