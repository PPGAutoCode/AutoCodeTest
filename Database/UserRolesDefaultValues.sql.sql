-- File: UserRolesDefaultValues.sql
CREATE TABLE UserRolesDefaultValues (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    UserRolesId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    DefaultValueId UNIQUEIDENTIFIER NOT NULL UNIQUE
);