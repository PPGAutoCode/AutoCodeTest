
    -- organizations.sql
    CREATE TABLE Organizations (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL UNIQUE,
        Image NVARCHAR(MAX) NULL,
        Description NVARCHAR(MAX) NULL,
        Status BIT NOT NULL,
        Created DATETIME2(7) NOT NULL,
        Changed DATETIME2(7) NOT NULL
    );
    