
CREATE TABLE GoLive (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Address NVARCHAR(200) NULL,
    ApplicationId UNIQUEIDENTIFIER NULL,
    CompanyName NVARCHAR(200) NULL,
    DeveloperType NVARCHAR(200) NULL,
    Email NVARCHAR(200) NULL UNIQUE,
    FirstName NVARCHAR(200) NULL,
    LastName NVARCHAR(200) NULL,
    SiteUrlDownloadLocation NVARCHAR(200) NULL,
    UseCases NVARCHAR(MAX) NULL
);
