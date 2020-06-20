CREATE TABLE [dbo].[Employee] (
    [Id]     NCHAR(10)             NOT NULL,
    [Login]  VARCHAR (50)    NOT NULL,
    [Name]   NVARCHAR (50)   NOT NULL,
    [Salary] DECIMAL (10, 2) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

