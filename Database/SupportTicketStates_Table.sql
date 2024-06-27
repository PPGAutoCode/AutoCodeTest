
-- File: SupportTicketStates_Table.sql

CREATE TABLE SupportTicketStates (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
);