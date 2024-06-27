userroles.sql
CREATE TABLE UserRoles (
    id uniqueidentifier NOT NULL PRIMARY KEY,
    Label nvarchar(100) NOT NULL UNIQUE,
    HelpText nvarchar(max) NULL,
    ReferenceMethod nvarchar(100) NOT NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL
);