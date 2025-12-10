-- 1. Switch context to the correct database
USE GrapheneTraceDB;
GO

-- 2. Create USERS Table (For Login)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL, 
    Role NVARCHAR(20) NOT NULL, -- 'Patient' or 'Clinician'
    FullName NVARCHAR(100)
);
GO

-- 3. Create COMMENTS Table (For Chat History)
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
GO

-- 4. Insert Test Data (Seed Data)
INSERT INTO Users (Username, Password, Role, FullName) VALUES 
('john', 'pass123', 'Patient', 'John Doe'),
('alice', 'pass123', 'Patient', 'Alice Wonderland'),
('drsmith', 'admin123', 'Clinician', 'Dr. Smith');
GO