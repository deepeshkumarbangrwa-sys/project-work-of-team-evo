-- 1. Create the Database (Check if exists first)
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'GrapheneTraceDB')
BEGIN
    CREATE DATABASE GrapheneTraceDB;
END
GO

USE GrapheneTraceDB;
GO

-- 2. USERS TABLE (Stores Login Info - uses Email as unique key)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY IDENTITY(1,1),
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(50) NOT NULL,
        Role NVARCHAR(20) NOT NULL,
        FullName NVARCHAR(100)
    );
END
GO

-- 3. COMMENTS TABLE (Chat storage)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' AND xtype='U')
BEGIN
    CREATE TABLE Comments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PatientID INT NOT NULL,
        AuthorID INT NOT NULL,
        AuthorName NVARCHAR(100),
        Role NVARCHAR(20),
        Content NVARCHAR(MAX),
        Timestamp DATETIME DEFAULT GETDATE(),
        IsRead BIT DEFAULT 0,
        FOREIGN KEY (PatientID) REFERENCES Users(UserID),
        FOREIGN KEY (AuthorID) REFERENCES Users(UserID)
    );
END
GO

-- 4. SEED DATA (Test Logins)
IF NOT EXISTS (SELECT * FROM Users)
BEGIN
    INSERT INTO Users (Email, Password, Role, FullName) VALUES 
    ('john@email.com', 'pass123', 'Patient', 'John Doe'),
    ('alice@email.com', 'pass123', 'Patient', 'Alice Wonderland'),
    ('drsmith@email.com', 'admin123', 'Clinician', 'Dr. Smith');
END
GO