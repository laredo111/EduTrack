CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(100) NOT NULL,
    MajorID INT NOT NULL,
    AcademicYear INT NOT NULL,
    Credits INT NOT NULL,
    Description NVARCHAR(255),

    FOREIGN KEY (MajorID) REFERENCES Majors(MajorID)
);
