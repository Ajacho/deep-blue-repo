DROP DATABASE IF EXISTS UxCheckmate;
CREATE DATABASE UxCheckmate;
USE UxCheckmate;

CREATE TABLE [Roles] 
(
    [RoleID]  INT                      NOT NULL IDENTITY(1,1),
    [Name]    NVARCHAR(50)             NOT NULL UNIQUE,
    PRIMARY KEY ([RoleID])
);

CREATE TABLE [UserAccounts] 
(
    [UserID]      INT                  NOT NULL IDENTITY(1,1),
    [Email]       NVARCHAR(255)        NOT NULL UNIQUE,
    [RoleID]      INT                  NOT NULL DEFAULT 0,
    PRIMARY KEY ([UserID]),
    CONSTRAINT FK_UserAccounts_RoleID FOREIGN KEY ([RoleID]) REFERENCES [Roles]([RoleID]) ON DELETE NO ACTION
);

CREATE TABLE [ReportCategories] 
(
    [CategoryID]      INT              NOT NULL IDENTITY(1,1),
    [Name]            NVARCHAR(255)    NOT NULL UNIQUE,
    [Description]     NVARCHAR(MAX)    NOT NULL,
    [OpenAIPrompt]    NVARCHAR(MAX)    NOT NULL,
    PRIMARY KEY ([CategoryID])
);

CREATE TABLE [Reports] 
(
    [ReportID]        INT             NOT NULL IDENTITY(1,1),
    [UserID]          INT             NOT NULL,
    [CategoryID]      INT             NOT NULL,
    [Date]            DATETIME2       NOT NULL DEFAULT SYSDATETIME(),
    [Recommendations] NVARCHAR(MAX)   NOT NULL,
    PRIMARY KEY ([ReportID]),
    CONSTRAINT FK_Reports_UserID FOREIGN KEY ([UserID]) REFERENCES [UserAccounts]([UserID]) ON DELETE CASCADE,
    CONSTRAINT FK_Reports_CategoryID FOREIGN KEY ([CategoryID]) REFERENCES [ReportCategories]([CategoryID]) ON DELETE NO ACTION
);