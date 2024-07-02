
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
            if (request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty ||
                string.IsNullOrEmpty(request.ContactDetails) || request.AppEnvironmentImpacted == null ||
                string.IsNullOrEmpty(request.NameOfReportingOrganization) || request.SeverityId == Guid.Empty ||
                string.IsNullOrEmpty(request.ShortDescription) || string.IsNullOrEmpty(request.State) ||
                request.Message == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user from the database by id from argument ReportedBy.
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch user from the database by id from argument AssignedTo.
            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Fetch appenvironment from the database by id from argument AppEnvironmentImpacted.
            foreach (var appEnvironmentId in request.AppEnvironmentImpacted)
            {
                var appEnvironment = await _dbConnection.QueryFirstOrDefaultAsync<AppEnvironment>("SELECT * FROM AppEnvironments WHERE Id = @Id", new { Id = appEnvironmentId });
                if (appEnvironment == null)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId.
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 6: Foreach item in SupportCategories argument:
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var supportCategory = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (supportCategory == null)
                    {
                        throw new BusinessException("DP-422", "Client Error");
                    }
                }
            }

            // Step 7: Create a new SupportTicket object (supportTicket) as follows from arguments:
            var supportTicket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                ReportedBy = request.ReportedBy,
                AssignedTo = request.AssignedTo,
                ContactDetails = request.ContactDetails,
                NameOfReportingOrganization = request.NameOfReportingOrganization,
                Priority = request.Priority,
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

            // Step 9: Create a new list of SupportTicketAppEnvironments objects (supportTicketAppEnvironments) as follows:
            var supportTicketAppEnvironments = new List<SupportTicketAppEnvironment>();
            foreach (var appEnvironmentId in request.AppEnvironmentImpacted)
            {
                supportTicketAppEnvironments.Add(new SupportTicketAppEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    AppEnvironmentsId = appEnvironmentId
                });
            }

            // Step 10: Create a new List of SupportTicketSeverities object (supportTicketSeverities) as follows:
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeveritiyId = request.SeverityId
                }
            };

            // Step 11: In a single SQL transaction:
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert supportTicket in the database table SupportTicket.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicket (Id, ReportedBy, AssignedTo, ContactDetails, NameOfReportingOrganization, Priority, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssignedTo, @ContactDetails, @NameOfReportingOrganization, @Priority, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

                    // Insert supportTicketCategories in the database table SupportTicketCategories.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);

                    // Insert supportTicketAppEnvironments in the database table SupportTicketAppEnvironments.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketAppEnvironments (Id, SupportTicketId, AppEnvironmentsId) VALUES (@Id, @SupportTicketId, @AppEnvironmentsId)", supportTicketAppEnvironments, transaction);

                    // Insert supportTicketSeverities in the database table SupportTicketSeverities.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiyId) VALUES (@Id, @SupportTicketId, @SeveritiyId)", supportTicketSeverities, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 12: Return SupportTicket.Id from the database.
            return supportTicket.Id.ToString();
        }
    }
}
