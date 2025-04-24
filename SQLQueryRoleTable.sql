USE EduTrackDB;

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
INSERT INTO Roles (RoleName) VALUES 
('Admin'),
('Teacher'),
('Student');
