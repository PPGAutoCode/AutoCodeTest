

-- Applications Table
CREATE TABLE Applications (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    AllowedGrantTypes nvarchar(max) NULL,
    APICHostname nvarchar(200) NULL,
    CredentialsDev nvarchar(max) NULL,
    CredentialsProd nvarchar(max) NULL,
    Description nvarchar(max) NULL,
    IdDev uniqueidentifier NULL,
    IdProd uniqueidentifier NULL,
    Image nvarchar(max) NULL,
    Status nvarchar(200) NULL,
    ClientIdDev uniqueidentifier NULL,
    ClientIdProd uniqueidentifier NULL,
    ClientSecret nvarchar(max) NULL,
    ClientUriDev nvarchar(200) NULL,
    ClientUriProd nvarchar(200) NULL,
    CreatedEvent nvarchar(max) NULL,
    EnvironmentId uniqueidentifier NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL
)