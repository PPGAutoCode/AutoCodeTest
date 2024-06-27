
CREATE TABLE Images (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    FileName nvarchar(100) NOT NULL UNIQUE,
    ImageData varbinary(max) NOT NULL,
    AltText nvarchar(500),
    Version int,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7),
    CreatorId uniqueidentifier NOT NULL,
    ChangedUser uniqueidentifier NOT NULL
);
