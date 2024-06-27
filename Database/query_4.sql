

-- SupportTicketCategories Table
CREATE TABLE SupportTicketCategories (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SupportTicketId UNIQUEIDENTIFIER NOT NULL,
    SupportCategoriesId UNIQUEIDENTIFIER NOT NULL
)