
CREATE TABLE Environments (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Version int,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7),
    CreatorId uniqueidentifier NOT NULL,
    ChangedUser uniqueidentifier NOT NULL
);
