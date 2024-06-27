
    -- authors.sql
    CREATE TABLE Authors (
        Id uniqueidentifier NOT NULL PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        ImageId uniqueidentifier NULL,
        Details nvarchar(max) NULL,
        Version int NULL,
        Created datetime2(7) NOT NULL,
        Changed datetime2(7) NOT NULL,
        CreatorId uniqueidentifier NOT NULL,
        ChangedUser uniqueidentifier NOT NULL
    );
    