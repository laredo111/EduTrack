CREATE TABLE StudentCourses (
    StudentCourseID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,

    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),

    CONSTRAINT UC_StudentCourse UNIQUE (StudentID, CourseID)
);
