

-- ProductTags Table
CREATE TABLE ProductTags (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL UNIQUE
)