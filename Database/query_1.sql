
-- SupportTicket Table
CREATE TABLE SupportTicket (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ReportedBy UNIQUEIDENTIFIER NOT NULL,
    AssignedTo UNIQUEIDENTIFIER NOT NULL,
    ContactDetails NVARCHAR(MAX) NOT NULL,
    DateClosed NVARCHAR(MAX) NULL,
    NameOfReportingOrganization NVARCHAR(MAX) NOT NULL,
    Priority NVARCHAR(MAX) NULL,
    ShortDescription NVARCHAR(MAX) NOT NULL,
    State NVARCHAR(MAX) NOT NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
)