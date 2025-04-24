CREATE TABLE TeacherCourses (
    TeacherCourseID INT PRIMARY KEY IDENTITY(1,1),
    TeacherID INT NOT NULL,
    CourseID INT NOT NULL,

    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),

    CONSTRAINT UC_TeacherCourse UNIQUE (TeacherID, CourseID)
);
