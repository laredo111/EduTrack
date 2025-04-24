CREATE TABLE Grades (
    GradeID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    Grade DECIMAL(5,2) NOT NULL CHECK (Grade >= 0 AND Grade <= 100),
    GradeDate DATE NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),

    CONSTRAINT UC_StudentGrade UNIQUE (StudentID, CourseID)
);
