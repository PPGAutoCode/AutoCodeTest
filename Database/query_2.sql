

-- Products Table
CREATE TABLE Products (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ApicHostname nvarchar(200) NULL,
    attachments nvarchar(max) NULL,
    CategoryId uniqueidentifier NULL,
    Deprecated bit NOT NULL,
    DisableDocumentation bit NOT NULL,
    EnvironmentId nvarchar(200) NULL,
    HeaderImage nvarchar(max) NULL,
    Label nvarchar(200) NULL,
    OverviewDisplay bit NOT NULL,
    Description nvarchar(max) NULL,
    RelatedProducts uniqueidentifier NULL,
    Domain nvarchar(200) NULL,
    Enabled bit NOT NULL,
    Name nvarchar(200) NULL,
    Image nvarchar(max) NULL,
    Version nvarchar(200) NULL,
    Visible bit NOT NULL,
    Weight int NULL,
    Langcode nvarchar(4) NOT NULL,
    Sticky bit NOT NULL,
    Status bit NOT NULL,
    Promote bit NOT NULL,
    CommercialProduct bit NOT NULL
)