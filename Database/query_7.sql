

-- SupportTicketEnvironments Table
CREATE TABLE SupportTicketEnvironments (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    SupportTicketId UNIQUEIDENTIFIER NOT NULL,
    EnvironmentsId UNIQUEIDENTIFIER NOT NULL
)