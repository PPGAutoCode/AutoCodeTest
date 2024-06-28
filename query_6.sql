-- APIEndpoints Table
CREATE TABLE APIEndpoints (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ApiContext NVARCHAR(200) NULL,
    ApiReferenceId NVARCHAR(200) NULL,
    ApiResource NVARCHAR(MAX) NULL,
    ApiScope NVARCHAR(200) NULL,
    ApiScopeProduction NVARCHAR(200) NULL,
    ApiSecurity NVARCHAR(MAX) NULL,
    ApiTags UNIQUEIDENTIFIER NULL,
    Deprecated BIT NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Documentation NVARCHAR(MAX) NULL,
    EndpointUrls NVARCHAR(200) NULL,
    EnvironmentId NVARCHAR(200) NULL,
    Swagger NVARCHAR(MAX) NULL,
    Tour NVARCHAR