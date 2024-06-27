

-- Users Table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Nickname NVARCHAR(100) NOT NULL UNIQUE,
    Status BIT NOT NULL,
    Password NVARCHAR(MAX) NULL,
    ConfirmPassword NVARCHAR(MAX) NULL,
    Email NVARCHAR(MAX) NOT NULL UNIQUE,
    ContactSettings BIT NULL,
    SiteLanguage NVARCHAR(MAX) NOT NULL,
    LocaleSettings NVARCHAR(MAX) NOT NULL,
    ImageId UNIQUEIDENTIFIER NULL,
    FirstName NVARCHAR(MAX) NOT NULL,
    LastName NVARCHAR(MAX) NOT NULL,
    Company NVARCHAR(MAX) NOT NULL,
    Phone NVARCHAR(MAX) NULL UNIQUE,
    IBM_UId NVARCHAR(MAX) NULL UNIQUE,
    MaxNumApps INT NULL,
    UserType NVARCHAR(MAX) NOT NULL,
    Questionnaire NVARCHAR(MAX) NULL,
    LastAccess DATETIME NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
)