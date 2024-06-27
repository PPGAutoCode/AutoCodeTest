

-- ApplicationsProducts Table
CREATE TABLE ApplicationsProducts (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductsId uniqueidentifier NOT NULL,
    ApplicationsId uniqueidentifier NOT NULL
)