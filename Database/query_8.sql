

-- SupportTicketEnvironments Table
CREATE TABLE SupportTicketEnvironments (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SupportTicketId UNIQUEIDENTIFIER NOT NULL,
    EnvironmentsId UNIQUEIDENTIFIER NOT NULL
)