
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
            if (request == null || request.ReportedBy == Guid.Empty || request.AssignedTo == Guid.Empty ||
                string.IsNullOrEmpty(request.ContactDetails) || request.AppEnvironmentImpacted == null ||
                request.AppEnvironmentImpacted.Count == 0 || string.IsNullOrEmpty(request.NameOfReportingOrganization) ||
                request.SeverityId == Guid.Empty || string.IsNullOrEmpty(request.ShortDescription) ||
                string.IsNullOrEmpty(request.State) || request.Message == null ||
                string.IsNullOrEmpty(request.Message.Body))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user from the database by id from argument ReportedBy
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch user from the database by id from argument AssignedTo
            var assignedToUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssignedTo });
            if (assignedToUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: For each item in AppEnvironmentImpacted argument
            foreach (var appEnvironmentId in request.AppEnvironmentImpacted)
            {
                var appEnvironment = await _dbConnection.QueryFirstOrDefaultAsync<AppEnvironment>("SELECT * FROM AppEnvironments WHERE Id = @Id", new { Id = appEnvironmentId });
                if (appEnvironment == null)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 6: Foreach item in SupportCategories argument
            var supportTicketCategories = new List<SupportTicketCategory>();
            if (request.SupportCategories != null)
            {
                foreach (var categoryId in request.SupportCategories)
                {
                    var supportCategory = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                    if (supportCategory == null)
                    {
                        throw new BusinessException("DP-422", "Client Error");
                    }
                    supportTicketCategories.Add(new SupportTicketCategory
                    {
                        Id = Guid.NewGuid(),
                        SupportTicketId = Guid.NewGuid(), // This will be updated later
                        SupportCategoryId = categoryId
                    });
                }
            }

            // Step 7: Create a new SupportTicket object
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
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.ReportedBy,
                ChangedUser = request.ReportedBy
            };

            // Step 8: Create a new Message object
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SupportTicketId = supportTicket.Id,
                Body = request.Message.Body
            };

            // Step 9: Create a new list of SupportTicketCategories objects
            foreach (var category in supportTicketCategories)
            {
                category.SupportTicketId = supportTicket.Id;
            }

            // Step 10: Create a new list of SupportTicketAppEnvironments objects
            var supportTicketAppEnvironments = request.AppEnvironmentImpacted.Select(envId => new SupportTicketAppEnvironment
            {
                Id = Guid.NewGuid(),
                SupportTicketId = supportTicket.Id,
                AppEnvironmentsId = envId
            }).ToList();

            // Step 11: Create a new List of SupportTicketSeverities object
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeveritiyId = request.SeverityId
                }
            };

            // Step 12: In a single SQL transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTickets (Id, ReportedBy, AssignedTo, ContactDetails, NameOfReportingOrganization, Priority, ShortDescription, State, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssignedTo, @ContactDetails, @NameOfReportingOrganization, @Priority, @ShortDescription, @State, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO Messages (Id, SupportTicketId, Body) VALUES (@Id, @SupportTicketId, @Body)", message, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketAppEnvironments (Id, SupportTicketId, AppEnvironmentsId) VALUES (@Id, @SupportTicketId, @AppEnvironmentsId)", supportTicketAppEnvironments, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeveritiyId) VALUES (@Id, @SupportTicketId, @SeveritiyId)", supportTicketSeverities, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return supportTicket.Id.ToString();
        }
    }
}
