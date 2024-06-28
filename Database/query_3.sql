

-- SupportTicketCategories Table
CREATE TABLE SupportTicketCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    SupportTicketId UNIQUEIDENTIFIER NOT NULL,
    SupportCategoriesId UNIQUEIDENTIFIER NOT NULL
)