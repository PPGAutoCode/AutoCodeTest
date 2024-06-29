
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
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch user from the database by id from argument ReportedBy.
            var reportedByUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.ReportedBy });
            if (reportedByUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch user from the database by id from argument AssigneDto.
            var assigneDtoUser = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = request.AssigneDto });
            if (assigneDtoUser == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Fetch environment from the database by id from argument EnvironmentImpacted.
            var environment = await _dbConnection.QueryFirstOrDefaultAsync<Environment>("SELECT * FROM Environments WHERE Id = @Id", new { Id = request.EnvironmentImpacted });
            if (environment == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 5: Fetch severity from the database by id from argument SeverityId.
            var severity = await _dbConnection.QueryFirstOrDefaultAsync<Severity>("SELECT * FROM Severities WHERE Id = @Id", new { Id = request.SeverityId });
            if (severity == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 6: Foreach item in SupportCategories argument:
            var supportCategories = new List<SupportCategory>();
            foreach (var categoryId in request.SupportCategories ?? new List<Guid>())
            {
                var category = await _dbConnection.QueryFirstOrDefaultAsync<SupportCategory>("SELECT * FROM SupportCategories WHERE Id = @Id", new { Id = categoryId });
                if (category == null)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
                supportCategories.Add(category);
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

            // Step 24: Create a new list of SupportTicketCategories objects (supportTicketCategories) as follows:
            var supportTicketCategories = new List<SupportTicketCategory>();
            foreach (var categoryId in request.SupportCategories ?? new List<Guid>())
            {
                supportTicketCategories.Add(new SupportTicketCategory
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SupportCategoryId = categoryId
                });
            }

            // Step 26: Create a new list of SupportTicketEnvironments objects (supportTicketEnvironments) as follows:
            var supportTicketEnvironments = new List<SupportTicketEnvironment>
            {
                new SupportTicketEnvironment
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    EnvironmentId = request.EnvironmentImpacted
                }
            };

            // Step 28: Create a new SupportTicketSeverities objects (supportTicketSeverities) as follows:
            var supportTicketSeverities = new List<SupportTicketSeverity>
            {
                new SupportTicketSeverity
                {
                    Id = Guid.NewGuid(),
                    SupportTicketId = supportTicket.Id,
                    SeverityId = request.SeverityId
                }
            };

            // Step 30: In a single SQL transaction:
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Step 31: Insert supportTicket in the database table SupportTicket.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicket (Id, ReportedBy, AssigneDto, ContactDetails, EnvironmentImpacted, NameOfReportingOrganization, Priority, SeverityId, ShortDescription, State, Message, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @ReportedBy, @AssigneDto, @ContactDetails, @EnvironmentImpacted, @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, @State, @Message, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", supportTicket, transaction);

                    // Step 32: Insert supportTicketCategories in the database table SupportTicketCategories.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketCategories (Id, SupportTicketId, SupportCategoryId) VALUES (@Id, @SupportTicketId, @SupportCategoryId)", supportTicketCategories, transaction);

                    // Step 33: Insert supportTicketEnvironments in the database table SupportTicketEnvironments.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketEnvironments (Id, SupportTicketId, EnvironmentId) VALUES (@Id, @SupportTicketId, @EnvironmentId)", supportTicketEnvironments, transaction);

                    // Step 34: Insert supportTicketSeverities in the database table SupportTicketSeverities.
                    await _dbConnection.ExecuteAsync("INSERT INTO SupportTicketSeverities (Id, SupportTicketId, SeverityId) VALUES (@Id, @SupportTicketId, @SeverityId)", supportTicketSeverities, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 37: Return SupportTicket.Id from the database.
            return supportTicket.Id.ToString();
        }

        // Implement other methods from ISupportTicketService interface similarly
    }
}
