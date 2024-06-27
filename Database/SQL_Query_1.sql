-- Stories Table
CREATE TABLE Stories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StoryNumber INT NOT NULL,
    CompletedConditionId UNIQUEIDENTIFIER NULL,
    Created DATETIME2(7) NOT NULL,
    Changed DATETIME2(7) NOT NULL
);