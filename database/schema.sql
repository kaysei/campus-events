IF DB_ID(N'CampusEvents') IS NULL CREATE DATABASE CampusEvents;
GO
USE CampusEvents;
GO

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) NOT NULL,
    RoleName NVARCHAR(30) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName),
    CONSTRAINT CK_Roles_RoleName CHECK (RoleName IN (N'Student', N'Admin'))
);

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) NOT NULL,
    RoleId INT NOT NULL,
    FullName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(254) NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (RoleId) ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT CK_Users_Email CHECK (Email LIKE N'%@univ.edu.ph'),
    CONSTRAINT CK_Users_FullName CHECK (LEN(LTRIM(RTRIM(FullName))) > 0)
);

CREATE TABLE dbo.Venues (
    VenueId INT IDENTITY(1,1) NOT NULL,
    VenueName NVARCHAR(120) NOT NULL,
    Capacity INT NOT NULL,
    CONSTRAINT PK_Venues PRIMARY KEY CLUSTERED (VenueId),
    CONSTRAINT UQ_Venues_VenueName UNIQUE (VenueName),
    CONSTRAINT CK_Venues_Capacity CHECK (Capacity > 0)
);

CREATE TABLE dbo.Events (
    EventId INT IDENTITY(1,1) NOT NULL,
    VenueId INT NOT NULL,
    CreatedByUserId INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NULL,
    StartsAt DATETIME2(0) NOT NULL,
    EndsAt DATETIME2(0) NOT NULL,
    MaxSeats INT NOT NULL,
    CONSTRAINT PK_Events PRIMARY KEY CLUSTERED (EventId),
    CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES dbo.Venues (VenueId) ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Events_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT CK_Events_Dates CHECK (EndsAt > StartsAt),
    CONSTRAINT CK_Events_MaxSeats CHECK (MaxSeats > 0)
);

CREATE TABLE dbo.Registrations (
    RegistrationId INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL,
    EventId INT NOT NULL,
    RegisteredAt DATETIME2(0) NOT NULL CONSTRAINT DF_Registrations_RegisteredAt DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Registrations_Status DEFAULT N'Confirmed',
    CONSTRAINT PK_Registrations PRIMARY KEY CLUSTERED (RegistrationId),
    CONSTRAINT UQ_Registrations_User_Event UNIQUE (UserId, EventId),
    CONSTRAINT FK_Registrations_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId) ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT FK_Registrations_Events FOREIGN KEY (EventId) REFERENCES dbo.Events (EventId) ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT CK_Registrations_Status CHECK (Status IN (N'Confirmed', N'Cancelled', N'Waitlisted'))
);
GO

CREATE NONCLUSTERED INDEX IX_Users_RoleId ON dbo.Users (RoleId);
CREATE NONCLUSTERED INDEX IX_Events_VenueId ON dbo.Events (VenueId);
CREATE NONCLUSTERED INDEX IX_Events_CreatedByUserId ON dbo.Events (CreatedByUserId);
CREATE NONCLUSTERED INDEX IX_Events_StartsAt ON dbo.Events (StartsAt);
CREATE NONCLUSTERED INDEX IX_Registrations_UserId ON dbo.Registrations (UserId);
CREATE NONCLUSTERED INDEX IX_Registrations_EventId ON dbo.Registrations (EventId);
GO

INSERT INTO dbo.Roles (RoleName) VALUES (N'Student'), (N'Admin');
GO
