

-- ProductProductCategories Table
CREATE TABLE ProductProductCategories (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    ProductCategoryId uniqueidentifier NOT NULL
)