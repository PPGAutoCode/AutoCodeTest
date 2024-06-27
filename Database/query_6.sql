userrolesusers.sql
CREATE TABLE UserRolesUsers (
    id uniqueidentifier NOT NULL PRIMARY KEY,
    UsersId uniqueidentifier NOT NULL UNIQUE,
    UserRolesId uniqueidentifier NOT NULL UNIQUE,
    Version int NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NOT NULL,
    ChangedUser uniqueidentifier NOT NULL
);