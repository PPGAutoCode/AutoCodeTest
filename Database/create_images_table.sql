
CREATE TABLE Images (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FileName NVARCHAR(100) NOT NULL UNIQUE,
    ImageData VARBINARY(MAX) NOT NULL,
    AltText NVARCHAR(500) NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
);
