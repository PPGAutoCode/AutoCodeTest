

-- ProductProductTags Table
CREATE TABLE ProductProductTags (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    TagId uniqueidentifier NOT NULL
)