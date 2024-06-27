
-- Subscriptions Table
CREATE TABLE Subscriptions (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductsId uniqueidentifier NOT NULL,
    ApplicationsId uniqueidentifier NOT NULL,
    Version int NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NOT NULL,
    ChangedUser uniqueidentifier NOT NULL
)