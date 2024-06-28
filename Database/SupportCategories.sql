
CREATE TABLE SupportCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NULL
);
