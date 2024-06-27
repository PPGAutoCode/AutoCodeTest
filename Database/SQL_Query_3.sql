-- StoriesSteps Table
CREATE TABLE StoriesSteps (
    Id INT NOT NULL PRIMARY KEY,
    StoriesId NVARCHAR(255) NOT NULL,
    StepsId NVARCHAR(MAX) NOT NULL
);