
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

        public async Task CreateSupportTicketAsync(CreateSupportTicketDto createSupportTicketDto)
        {
            ValidateCreateSupportTicketDto(createSupportTicketDto);

            var sql = @"
                INSERT INTO SupportTickets (
                    ReportedBy, AssigneDto, ContactDetails, FileId, EnvironmentImpacted, 
                    NameOfReportingOrganization, Priority, SeverityId, ShortDescription, 
                    State, Message, SupportCategories
                ) VALUES (
                    @ReportedBy, @AssigneDto, @ContactDetails, @FileId, @EnvironmentImpacted, 
                    @NameOfReportingOrganization, @Priority, @SeverityId, @ShortDescription, 
                    @State, @Message, @SupportCategories
                );
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, createSupportTicketDto);
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

        public async Task UpdateSupportTicketAsync(UpdateSupportTicketDto updateSupportTicketDto)
        {
            ValidateUpdateSupportTicketDto(updateSupportTicketDto);

            var sql = @"
                UPDATE SupportTickets SET
                    ReportedBy = @ReportedBy, AssigneDto = @AssigneDto, ContactDetails = @ContactDetails, 
                    DateClosed = @DateClosed, EnvironmentImpacted = @EnvironmentImpacted, 
                    NameOfReportingOrganization = @NameOfReportingOrganization, Priority = @Priority, 
                    Severity = @Severity, ShortDescription = @ShortDescription, State = @State, 
                    SupportCategories = @SupportCategories, Messages = @Messages, Version = @Version, 
                    Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, updateSupportTicketDto);
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

        public async Task<IEnumerable<SupportTicket>> ListSupportTicketsAsync(ListSupportTicketRequestDto listSupportTicketRequestDto)
        {
            ValidateListSupportTicketRequestDto(listSupportTicketRequestDto);

            var sql = @"
                SELECT * FROM SupportTickets
                ORDER BY @SortField @SortOrder
                OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY;
            ";

            try
            {
                return await _dbConnection.QueryAsync<SupportTicket>(sql, listSupportTicketRequestDto);
            }
            catch (Exception ex)
            {
                throw new TechnicalException("1001", "A technical exception has occurred, please contact your system administrator");
            }
        }

        private void ValidateCreateSupportTicketDto(CreateSupportTicketDto createSupportTicketDto)
        {
            if (createSupportTicketDto == null)
            {
                throw new BusinessException("1002", "CreateSupportTicketDto cannot be null");
            }

            if (createSupportTicketDto.ReportedBy == Guid.Empty)
            {
                throw new BusinessException("1003", "ReportedBy cannot be empty");
            }

            if (createSupportTicketDto.AssigneDto == Guid.Empty)
            {
                throw new BusinessException("1004", "AssigneDto cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(createSupportTicketDto.ContactDetails))
            {
                throw new BusinessException("1005", "ContactDetails cannot be empty");
            }

            if (createSupportTicketDto.EnvironmentImpacted == Guid.Empty)
            {
                throw new BusinessException("1006", "EnvironmentImpacted cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(createSupportTicketDto.NameOfReportingOrganization))
            {
                throw new BusinessException("1007", "NameOfReportingOrganization cannot be empty");
            }

            if (createSupportTicketDto.SeverityId == Guid.Empty)
            {
                throw new BusinessException("1008", "SeverityId cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(createSupportTicketDto.ShortDescription))
            {
                throw new BusinessException("1009", "ShortDescription cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(createSupportTicketDto.State))
            {
                throw new BusinessException("1010", "State cannot be empty");
            }

            if (createSupportTicketDto.Message == null)
            {
                throw new BusinessException("1011", "Message cannot be null");
            }
        }

        private void ValidateUpdateSupportTicketDto(UpdateSupportTicketDto updateSupportTicketDto)
        {
            if (updateSupportTicketDto == null)
            {
                throw new BusinessException("1012", "UpdateSupportTicketDto cannot be null");
            }

            if (updateSupportTicketDto.Id == Guid.Empty)
            {
                throw new BusinessException("1013", "Id cannot be empty");
            }

            if (updateSupportTicketDto.ReportedBy == Guid.Empty)
            {
                throw new BusinessException("1014", "ReportedBy cannot be empty");
            }

            if (updateSupportTicketDto.AssigneDto == Guid.Empty)
            {
                throw new BusinessException("1015", "AssigneDto cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(updateSupportTicketDto.ContactDetails))
            {
                throw new BusinessException("1016", "ContactDetails cannot be empty");
            }

            if (updateSupportTicketDto.EnvironmentImpacted == Guid.Empty)
            {
                throw new BusinessException("1017", "EnvironmentImpacted cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(updateSupportTicketDto.NameOfReportingOrganization))
            {
                throw new BusinessException("1018", "NameOfReportingOrganization cannot be empty");
            }

            if (updateSupportTicketDto.Severity == Guid.Empty)
            {
                throw new BusinessException("1019", "Severity cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(updateSupportTicketDto.ShortDescription))
            {
                throw new BusinessException("1020", "ShortDescription cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(updateSupportTicketDto.State))
            {
                throw new BusinessException("1021", "State cannot be empty");
            }

            if (updateSupportTicketDto.Messages == null || !updateSupportTicketDto.Messages.Any())
            {
                throw new BusinessException("1022", "Messages cannot be null or empty");
            }

            if (updateSupportTicketDto.Changed == default)
            {
                throw new BusinessException("1023", "Changed cannot be default");
            }

            if (updateSupportTicketDto.ChangedUser == Guid.Empty)
            {
                throw new BusinessException("1024", "ChangedUser cannot be empty");
            }
        }

        private void ValidateListSupportTicketRequestDto(ListSupportTicketRequestDto listSupportTicketRequestDto)
        {
            if (listSupportTicketRequestDto == null)
            {
                throw new BusinessException("1025", "ListSupportTicketRequestDto cannot be null");
            }

            if (listSupportTicketRequestDto.PageLimit <= 0)
            {
                throw new BusinessException("1026", "PageLimit must be greater than 0");
            }

            if (listSupportTicketRequestDto.PageOffset < 0)
            {
                throw new BusinessException("1027", "PageOffset cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(listSupportTicketRequestDto.SortField))
            {
                throw new BusinessException("1028", "SortField cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(listSupportTicketRequestDto.SortOrder))
            {
                throw new BusinessException("1029", "SortOrder cannot be empty");
            }
        }
    }
}
