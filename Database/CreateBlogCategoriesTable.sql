
-- File: CreateBlogCategoriesTable.sql

CREATE TABLE BlogCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Parent UNIQUEIDENTIFIER NULL,
    Name NVARCHAR(200) NOT NULL,
    CONSTRAINT FK_BlogCategories_Parent FOREIGN KEY (Parent) REFERENCES BlogCategories(Id)
);
