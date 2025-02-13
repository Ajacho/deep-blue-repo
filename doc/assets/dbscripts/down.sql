ALTER TABLE [Reports] DROP CONSTRAINT FK_Reports_UserID;
ALTER TABLE [Reports] DROP CONSTRAINT FK_Reports_CategoryID;
ALTER TABLE [UserAccounts] DROP CONSTRAINT FK_UserAccounts_RoleID;

DROP TABLE [Reports];
DROP TABLE [ReportCategories];
DROP TABLE [UserAccounts];
DROP TABLE [Roles];