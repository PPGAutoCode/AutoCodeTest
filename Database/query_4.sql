

-- ProductApiEndpoints Table
CREATE TABLE ProductApiEndpoints (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    ApiEndpointId uniqueidentifier NOT NULL
)