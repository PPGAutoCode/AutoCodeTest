

-- ProductEnvironments Table
CREATE TABLE ProductEnvironments (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    EnvironmentId uniqueidentifier NOT NULL
)