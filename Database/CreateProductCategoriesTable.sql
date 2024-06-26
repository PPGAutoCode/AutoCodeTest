
CREATE TABLE ProductCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    UserQuestionnaire BIT NULL,
    Description NVARCHAR(MAX) NULL,
    Parent NVARCHAR(MAX) NULL,
    UrlAlias NVARCHAR(200) NULL,
    Weight INT NULL
);
