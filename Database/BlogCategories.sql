
CREATE TABLE BlogCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Parent UNIQUEIDENTIFIER NULL,
    Name NVARCHAR(200) NOT NULL
);
