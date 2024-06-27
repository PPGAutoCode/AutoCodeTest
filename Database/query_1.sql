
-- Products Table
CREATE TABLE Products (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ApicHostname nvarchar(200) NULL,
    attachments nvarchar(max) NULL,
    Deprecated bit NULL,
    DisableDocumentation bit NULL,
    HeaderImage nvarchar(max) NULL,
    Label nvarchar(200) NULL,
    OverviewDisplay bit NULL,
    Description nvarchar(max) NULL,
    DomainId uniqueidentifier NULL,
    Enabled bit NULL,
    Name nvarchar(200) NOT NULL,
    ImageId uniqueidentifier NULL,
    Visible bit NULL,
    Weight int NULL,
    Langcode nvarchar(4) NULL,
    Sticky bit NULL,
    Status bit NULL,
    Promote bit NULL,
    CommercialProduct bit NULL,
    Version int NOT NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NOT NULL,
    ChangedUser uniqueidentifier NOT NULL
)