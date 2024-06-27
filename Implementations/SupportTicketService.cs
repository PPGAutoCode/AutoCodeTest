
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

        public async Task CreateSupportTicketAsync(CreateSupportTicketDTO ticketDTO)
        {
            ValidateCreateSupportTicketDTO(ticketDTO);

            var sql = @"
                INSERT INTO SupportTickets (
                    ReportedBy, AssignedTo, ContactDetails, FileId, EnvironmentImpacted, 
                    NameOfReportingOrganization, Priority, SeverityId, ShortDescription, 
                    State, Message, SupportCategories
                ) VALUES (
                    @ReportedBy, @AssignedTo, @ContactDetails, @FileId, @EnvironmentImpacted, 
                    @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, 
                    @State, @Message, @SupportCategories
                );
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, ticketDTO);
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        public async Task<SupportTicket> GetSupportTicketAsync(Guid id)
        {
            var sql = @"
                SELECT * FROM SupportTickets WHERE Id = @Id;
            ";

            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<SupportTicket>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        public async Task UpdateSupportTicketAsync(UpdateSupportTicketDTO ticketDTO)
        {
            ValidateUpdateSupportTicketDTO(ticketDTO);

            var sql = @"
                UPDATE SupportTickets SET
                    ReportedBy = @ReportedBy, AssignedTo = @AssignedTo, ContactDetails = @ContactDetails, 
                    DateClosed = @DateClosed, EnvironmentImpacted = @EnvironmentImpacted, 
                    NameOfReportingOrganization = @NameOfReportingOrganization, Priority = @Priority, 
                    Severity = @Severity, ShortDescription = @ShortDescription, State = @State, 
                    SupportCategories = @SupportCategories, Messages = @Messages, Version = @Version, 
                    Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, ticketDTO);
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        public async Task DeleteSupportTicketAsync(Guid id)
        {
            var sql = @"
                DELETE FROM SupportTickets WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        public async Task<IEnumerable<SupportTicket>> ListSupportTicketsAsync(ListSupportTicketRequestDTO requestDTO)
        {
            ValidateListSupportTicketRequestDTO(requestDTO);

            var sql = @"
                SELECT * FROM SupportTickets
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                return await _dbConnection.QueryAsync<SupportTicket>(sql, requestDTO);
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        private void ValidateCreateSupportTicketDTO(CreateSupportTicketDTO ticketDTO)
        {
            if (ticketDTO == null)
            {
                throw new BusinessException("1002", "CreateSupportTicketDTO cannot be null");
            }

            if (ticketDTO.ReportedBy == Guid.Empty)
            {
                throw new BusinessException("1003", "ReportedBy cannot be empty");
            }

            if (ticketDTO.AssignedTo == Guid.Empty)
            {
                throw new BusinessException("1004", "AssignedTo cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.ContactDetails))
            {
                throw new BusinessException("1005", "ContactDetails cannot be empty");
            }

            if (ticketDTO.FileId == Guid.Empty)
            {
                throw new BusinessException("1006", "FileId cannot be empty");
            }

            if (ticketDTO.EnvironmentImpacted == Guid.Empty)
            {
                throw new BusinessException("1007", "EnvironmentImpacted cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.NameOfReportingOrganization))
            {
                throw new BusinessException("1008", "NameOfReportingOrganization cannot be empty");
            }

            if (ticketDTO.SeverityId == Guid.Empty)
            {
                throw new BusinessException("1009", "SeverityId cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.ShortDescription))
            {
                throw new BusinessException("1010", "ShortDescription cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.State))
            {
                throw new BusinessException("1011", "State cannot be empty");
            }

            if (ticketDTO.Message == null)
            {
                throw new BusinessException("1012", "Message cannot be null");
            }
        }

        private void ValidateUpdateSupportTicketDTO(UpdateSupportTicketDTO ticketDTO)
        {
            if (ticketDTO == null)
            {
                throw new BusinessException("1013", "UpdateSupportTicketDTO cannot be null");
            }

            if (ticketDTO.Id == Guid.Empty)
            {
                throw new BusinessException("1014", "Id cannot be empty");
            }

            if (ticketDTO.ReportedBy == Guid.Empty)
            {
                throw new BusinessException("1015", "ReportedBy cannot be empty");
            }

            if (ticketDTO.AssignedTo == Guid.Empty)
            {
                throw new BusinessException("1016", "AssignedTo cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.ContactDetails))
            {
                throw new BusinessException("1017", "ContactDetails cannot be empty");
            }

            if (ticketDTO.EnvironmentImpacted == Guid.Empty)
            {
                throw new BusinessException("1018", "EnvironmentImpacted cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.NameOfReportingOrganization))
            {
                throw new BusinessException("1019", "NameOfReportingOrganization cannot be empty");
            }

            if (ticketDTO.Severity == Guid.Empty)
            {
                throw new BusinessException("1020", "Severity cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.ShortDescription))
            {
                throw new BusinessException("1021", "ShortDescription cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticketDTO.State))
            {
                throw new BusinessException("1022", "State cannot be empty");
            }

            if (ticketDTO.Messages == null || !ticketDTO.Messages.Any())
            {
                throw new BusinessException("1023", "Messages cannot be null or empty");
            }

            if (ticketDTO.Changed == default(DateTime))
            {
                throw new BusinessException("1024", "Changed cannot be default");
            }

            if (ticketDTO.ChangedUser == Guid.Empty)
            {
                throw new BusinessException("1025", "ChangedUser cannot be empty");
            }
        }

        private void ValidateListSupportTicketRequestDTO(ListSupportTicketRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                throw new BusinessException("1026", "ListSupportTicketRequestDTO cannot be null");
            }

            if (requestDTO.PageLimit <= 0)
            {
                throw new BusinessException("1027", "PageLimit must be greater than 0");
            }

            if (requestDTO.PageOffset < 0)
            {
                throw new BusinessException("1028", "PageOffset cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(requestDTO.SortField))
            {
                throw new BusinessException("1029", "SortField cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(requestDTO.SortOrder))
            {
                throw new BusinessException("1030", "SortOrder cannot be empty");
            }
        }
    }
}
