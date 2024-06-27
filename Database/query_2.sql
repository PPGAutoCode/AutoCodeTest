users.sql
CREATE TABLE Users (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Nickname nvarchar(100) NOT NULL UNIQUE,
    Status bit NOT NULL,
    Password nvarchar(100) NOT NULL,
    ConfirmPassword nvarchar(100) NOT NULL,
    Email nvarchar(100) NOT NULL UNIQUE,
    ContactSettings bit NULL,
    SiteLanguage nvarchar(100) NOT NULL,
    LocaleSettings nvarchar(100) NOT NULL,
    ImageId uniqueidentifier NULL,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    Company nvarchar(100) NOT NULL,
    Phone nvarchar(100) NOT NULL UNIQUE,
    IBM_UId nvarchar(100) NOT NULL UNIQUE,
    MaxNumApps int NULL,
    UserTypeId uniqueidentifier NULL,
    UserQuestionnaireId uniqueidentifier NULL,
    Created datetime NULL,
    LastAccess datetime NULL
);