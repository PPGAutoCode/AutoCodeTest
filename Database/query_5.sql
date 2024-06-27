

-- APIEndpoints Table
CREATE TABLE APIEndpoints (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ApiContext nvarchar(200) NULL,
    ApiReferenceId nvarchar(200) NULL,
    ApiResource nvarchar(max) NULL,
    ApiScope nvarchar(200) NULL,
    ApiScopeProduction nvarchar(200) NULL,
    ApiSecurity nvarchar(max) NULL,
    ApiTags uniqueidentifier NULL,
    Deprecated bit NOT NULL,
    Description nvarchar(max) NULL,
    Documentation nvarchar(max) NULL,
    EndpointUrls nvarchar(200) NULL,
    EnvironmentId nvarchar(200) NULL,
    Swagger nvarchar(max) NULL,
    Tour nvarchar
)