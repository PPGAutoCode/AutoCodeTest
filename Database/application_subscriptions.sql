
CREATE TABLE ApplicationSubscriptions (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ApplicationId uniqueidentifier NOT NULL,
    SubscriptionId uniqueidentifier NOT NULL
);
