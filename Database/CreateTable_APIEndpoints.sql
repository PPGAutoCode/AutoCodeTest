CreateTable_APIEndpoints.sql
CREATE TABLE APIEndpoints (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    ApiContext NVARCHAR(200),
    ApiReferenceId NVARCHAR(200),
    ApiResource NVARCHAR(MAX),
    ApiScope NVARCHAR(200),
    ApiScopeProduction NVARCHAR(200),
    ApiSecurity NVARCHAR(MAX),
    ApiTags UNIQUEIDENTIFIER,
    Deprecated BIT NOT NULL,
    Description NVARCHAR(MAX),
    Documentation NVARCHAR(MAX),
    EndpointUrls NVARCHAR(200),
    EnvironmentId NVARCHAR(200),
    ProviderId NVARCHAR(200),
    Swagger NVARCHAR(MAX),
    Tour NVARCHAR(MAX),
    Updated TIMESTAMP,
    Version NVARCHAR(200)
);