CREATE TABLE StudentMajors (
    StudentMajorID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    MajorID INT NOT NULL,
    AcademicYear INT NOT NULL, -- ??????: 2024

    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (MajorID) REFERENCES Majors(MajorID)
);
