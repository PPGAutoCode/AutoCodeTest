
-- File: UserQuestionnaire.sql
CREATE TABLE UserQuestionnaire (
    id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    CompanyErpSolutionName NVARCHAR(255) NOT NULL UNIQUE,
    CompanyErpSolutionVersion NVARCHAR(255) NOT NULL UNIQUE,
    CompanyKad NVARCHAR(255) NOT NULL UNIQUE,
    CompanyLegalName NVARCHAR(255) NOT NULL UNIQUE,
    CompanyOwnsBankAccount BIT NOT NULL UNIQUE,
    CompanyReprEmail NVARCHAR(255) NOT NULL UNIQUE,
    CompanyRepFullName NVARCHAR(255) NOT NULL UNIQUE,
    CompanyRepNumber NVARCHAR(255) NOT NULL UNIQUE,
    CompanyTaxId NVARCHAR(255) NOT NULL UNIQUE,
    CompanyUsesErp BIT NOT NULL UNIQUE,
    CompanyWebsite NVARCHAR(255) NOT NULL UNIQUE,
    CorporateUserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    ErpBankingActivities NVARCHAR(255) NOT NULL,
    ProductCategoriesId UNIQUEIDENTIFIER NOT NULL
);
