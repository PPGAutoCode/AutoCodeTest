
    -- documents.sql
    CREATE TABLE Documents (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Label NVARCHAR(200) NOT NULL UNIQUE,
        Helptext NVARCHAR(500) NULL,
        [Required Field] BIT NULL,
        AllowedFileExtensions NVARCHAR(200) NOT NULL,
        FileDirectory NVARCHAR(200) NULL,
        MaxUploadSize INT NULL,
        EnableDescriptionField BIT NULL
    );
    