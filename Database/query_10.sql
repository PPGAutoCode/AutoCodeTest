

-- ProductAdvantages Table
CREATE TABLE ProductAdvantages (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(max) NULL,
    Title nvarchar(200) NULL,
    Header nvarchar(200) NOT NULL
)