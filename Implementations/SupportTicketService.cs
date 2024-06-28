
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
            // Step 1: Validate all fields of request.payload are not null except for Priority and SupportCategories.
            if (request.ReportedBy == Guid.Empty || request.AssigneDto == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
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

            // Step 3: Fetch user from the database by id from argument AssigneDto.
            var assigneDtoUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssigneDto });
            if (assigneDtoUser == null)
            {
                throw new BusinessException("DP-404", "Assigned user not found.");
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

            // Step 6: Foreach item in SupportCategories argument: Fetch support category from the database by id.
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var category = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (category == null)
                    {
                        throw new BusinessException("DP-422", "Support category not found.");
                    }
                }
            }

            // Step 7: Create a new SupportTicket object (supportTicket) as follows from arguments:
            var supportTicket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                ReportedBy = request.ReportedBy,
                AssigneDto = request.AssigneDto,
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

            // Step 8: Create a new list of SupportTicketCategories objects (supportTicketCategories) as follows:
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

            // Step 9: Create a new list of SupportTicketEnvironments objects (supportTicketEnvironments) as follows:
            var supportTicketEnvironments = new List<SupportTicketEnvironment>
            {
                new SupportTicketEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    EnvironmentId = request.EnvironmentImpacted
                }
            };

            // Step 10: Create a new SupportTicketSeverities objects (supportTicketSeverities) as follows:
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeverityId = request.SeverityId
                }
            };

            // Step 11: In a single SQL transaction:
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert supportTicket in the database table SupportTicket.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicket (Id, ReportedBy, AssigneDto, ContactDetails, EnvironmentImpacted, NameOfReportingOrganization, Priority, SeverityId, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssigneDto, @ContactDetails, @EnvironmentImpacted, @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

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

        public async Task<SupportTicket> GetSupportTicket(RequestSupportTicketDto request)
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

            // Step 3: Fetch related data:
            // Fetch the user who reported the ticket by ReportedBy.
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-404", "Reported user not found.");
            }

            // Fetch the user assigned to the ticket by AssigneDto.
            var assigneDtoUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = supportTicket.AssigneDto });
            if (assigneDtoUser == null)
            {
                throw new BusinessException("DP-404", "Assigned user not found.");
            }

            // Fetch the environment impacted by the ticket by EnvironmentImpacted.
            var environment = await _dbConnection.QueryFirstOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = supportTicket.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-404", "Environment not found.");
            }

            // Fetch the severity level by SeverityId.
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = supportTicket.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-404", "Severity not found.");
            }

            // Fetch support categories:
            var supportCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id IN @Ids", new { Ids = supportCategoryIds });
            if (supportCategories.Any(sc => sc == null))
            {
                throw new BusinessException("DP-404", "Support category not found.");
            }

            // Fetch messages associated with the support ticket:
            var messageIds = await _dbConnection.QueryAsync<Guid>("SELECT Id FROM Messages WHERE SupportTicketId = @Id", new { Id = supportTicket.Id });
            var messages = await _dbConnection.QueryAsync<Message>("SELECT * FROM Messages WHERE Id IN @Ids", new { Ids = messageIds });
            if (messages.Any(m => m == null))
            {
                throw new BusinessException("DP-404", "Message not found.");
            }

            // Map the database object to SupportTicket and return the SupportTicket.
            supportTicket.SupportCategories = supportCategories.ToList();
            supportTicket.Messages = messages.ToList();

            return supportTicket;
        }

        public async Task<string> UpdateSupportTicket(UpdateSupportTicketDto request)
        {
            // Step 1: Validate Necessary Parameters:
            if (request.Id == Guid.Empty || request.ReportedBy == Guid.Empty || request.AssigneDto == Guid.Empty || string.IsNullOrEmpty(request.ContactDetails) ||
                request.EnvironmentImpacted == Guid.Empty || string.IsNullOrEmpty(request.NameOfReportingOrganization) || string.IsNullOrEmpty(request.ShortDescription) ||
                string.IsNullOrEmpty(request.State) || request.SeverityId == Guid.Empty)
            {
                throw new BusinessException("DP-422", "One of the mandatory arguments is null.");
            }

            // Step 2: Fetch Existing Support Ticket:
            var existingSupportTicket = await _dbConnection.QueryFirstOrDefaultAsync<SupportTicket>("SELECT * FROM SupportTicket WHERE Id = @Id", new { Id = request.Id });
            if (existingSupportTicket == null)
            {
                throw new BusinessException("DP-404", "Support ticket not found.");
            }

            // Step 3: Validate Related Entities:
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new BusinessException("DP-422", "Reported user not found.");
            }

            var assigneDtoUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssigneDto });
            if (assigneDtoUser == null)
            {
                throw new BusinessException("DP-422", "Assigned user not found.");
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
            existingSupportTicket.ReportedBy = request.ReportedBy;
            existingSupportTicket.AssigneDto = request.AssigneDto;
            existingSupportTicket.ContactDetails = request.ContactDetails;
            existingSupportTicket.DateClosed = request.DateClosed;
            existingSupportTicket.EnvironmentImpacted = request.EnvironmentImpacted;
            existingSupportTicket.NameOfReportingOrganization = request.NameOfReportingOrganization;
            existingSupportTicket.Priority = request.Priority;
            existingSupportTicket.SeverityId = request.SeverityId;
            existingSupportTicket.ShortDescription = request.ShortDescription;
            existingSupportTicket.State = request.State;
            existingSupportTicket.SupportCategories = request.SupportCategories;
            existingSupportTicket.Messages = request.Messages;
            existingSupportTicket.Version += 1;
            existingSupportTicket.Changed = DateTime.UtcNow;
            existingSupportTicket.ChangedUser = request.ChangedUser;

            // Step 5: Update SupportTicketCategories:
            var existingCategories = await _dbConnection.QueryAsync<Guid>("SELECT SupportCategoryId FROM SupportTicketCategories WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var categoriesToRemove = existingCategories.Except(request.SupportCategories ?? new List<Guid>()).ToList();
            var categoriesToAdd = (request.SupportCategories ?? new List<Guid>()).Except(existingCategories).ToList();

            if (categoriesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketCategories WHERE SupportTicketId = @Id AND SupportCategoryId IN @Ids", new { Id = existingSupportTicket.Id, Ids = categoriesToRemove });
            }

            if (categoriesToAdd.Any())
            {
                var newCategories = categoriesToAdd.Select(categoryId => new { Id = Guid.NewGuid(), SupportTicketId = existingSupportTicket.Id, SupportCategoryId = categoryId });
                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", newCategories);
            }

            // Step 6: Update SupportTicketEnvironments:
            var existingEnvironments = await _dbConnection.QueryAsync<Guid>("SELECT EnvironmentId FROM SupportTicketEnvironments WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var environmentsToRemove = existingEnvironments.Except(new List<Guid> { request.EnvironmentImpacted }).ToList();
            var environmentsToAdd = new List<Guid> { request.EnvironmentImpacted }.Except(existingEnvironments).ToList();

            if (environmentsToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketEnvironments WHERE SupportTicketId = @Id AND EnvironmentId IN @Ids", new { Id = existingSupportTicket.Id, Ids = environmentsToRemove });
            }

            if (environmentsToAdd.Any())
            {
                var newEnvironments = environmentsToAdd.Select(environmentId => new { Id = Guid.NewGuid(), SupportTicketId = existingSupportTicket.Id, EnvironmentId = environmentId });
                await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentId) VALUES (@Id, @SupportTicketId, @EnvironmentId)", newEnvironments);
            }

            // Step 7: Update SupportTicketSeverities:
            var existingSeverities = await _dbConnection.QueryAsync<Guid>("SELECT SeverityId FROM SupportTicketSeverities WHERE SupportTicketId = @Id", new { Id = existingSupportTicket.Id });
            var severitiesToRemove = existingSeverities.Except(new List<Guid> { request.SeverityId }).ToList();
            var severitiesToAdd = new List<Guid> { request.SeverityId }.Except(existingSeverities).ToList();

            if (severitiesToRemove.Any())
            {
                await _dbConnection.ExecuteAsync("DELETE FROM SupportTicketSeverities WHERE Support