-- File:CreateTable_APIEndpoints.sql
CREATE TABLE APIEndpoints (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
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
    ProviderId nvarchar(200) NULL,
    Swagger nvarchar(max) NULL,
    Tour nvarchar(max) NULL,
    Updated timestamp NULL,
    Version nvarchar(200) NULL
);