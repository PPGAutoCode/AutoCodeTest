

-- ProductFeatures Table
CREATE TABLE ProductFeatures (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Title nvarchar(200) NULL,
    Description nvarchar(max) NULL,
    Icon nvarchar(max) NULL
)