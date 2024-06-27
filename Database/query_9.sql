

-- ProductProductFeatures Table
CREATE TABLE ProductProductFeatures (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    FeatureId uniqueidentifier NOT NULL
)