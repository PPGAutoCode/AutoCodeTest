-- ApplicationsProducts Table
CREATE TABLE ApplicationsProducts (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProductsId UNIQUEIDENTIFIER NOT NULL,
    ApplicationsId UNIQUEIDENTIFIER NOT NULL
);