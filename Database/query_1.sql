

-- File: CreateProductsTable.sql
CREATE TABLE Products (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ApicHostname NVARCHAR(200) NULL,
    attachments NVARCHAR(MAX) NULL,
    Deprecated BIT NOT NULL,
    DisableDocumentation BIT NOT NULL,
    EnvironmentId NVARCHAR(200) NULL,
    HeaderImage NVARCHAR(MAX) NULL,
    Label NVARCHAR(200) NULL,
    OverviewDisplay BIT NOT NULL,
    Description NVARCHAR(MAX) NULL,
    RelatedProducts UNIQUEIDENTIFIER NULL,
    Domain NVARCHAR(200) NULL,
    Enabled BIT NOT NULL,
    Name NVARCHAR(200) NULL,
    Image NVARCHAR(MAX) NULL,
    Visible BIT NOT NULL,
    Weight INT NULL,
    Langcode NVARCHAR(4) NOT NULL,
    Sticky BIT NOT NULL,
    Status BIT NOT NULL,
    Promote BIT NOT NULL,
    CommercialProduct BIT NOT NULL,
    Version INT NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    ChangedUser UNIQUEIDENTIFIER NOT NULL
)