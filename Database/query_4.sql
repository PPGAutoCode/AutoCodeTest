

-- ProductCategories Table
CREATE TABLE ProductCategories (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL UNIQUE,
    UserQuestionnaire bit NULL,
    Description nvarchar(max) NULL,
    Parent nvarchar(max) NULL,
    UrlAlias nvarchar(200) NULL,
    Weight int NULL
)