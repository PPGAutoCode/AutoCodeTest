
CREATE TABLE Contact (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(500) NOT NULL,
    Mail NVARCHAR(500) NOT NULL UNIQUE,
    Subject NVARCHAR(500) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Created DATETIME2(7) NOT NULL
);
