

-- AttachmentFile Table
CREATE TABLE AttachmentFile (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    MessageId UNIQUEIDENTIFIER NOT NULL,
    FileId UNIQUEIDENTIFIER NOT NULL
)