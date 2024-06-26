-- File: CreateChangeLogsTable.sql
CREATE TABLE ChangeLogs (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    ReleaseDate DATETIME2(7) NOT NULL,
    ChangeLogVersion NVARCHAR(500) NOT NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
)