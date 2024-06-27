-- Articles Table
CREATE TABLE Articles (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    AuthorId UNIQUEIDENTIFIER NOT NULL,
    Summary NVARCHAR(500),
    Body NVARCHAR(MAX),
    GoogleDriveID NVARCHAR(50),
    HideScrollSpy BIT NOT NULL,
    ImageId UNIQUEIDENTIFIER,
    PDF UNIQUEIDENTIFIER,
    Langcode NVARCHAR(4) NOT NULL,
    Status BIT NOT NULL,
    Sticky BIT NOT NULL,
    Promote BIT NOT NULL,
    UserRoles NVARCHAR(MAX),
    Version INT,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
);