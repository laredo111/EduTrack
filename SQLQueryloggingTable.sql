CREATE TABLE LoginLog (
    LoginLogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    LoginTime DATETIME NOT NULL DEFAULT GETDATE(),
    IPAddress NVARCHAR(50),
    Success BIT NOT NULL, -- 1 = הצלחה, 0 = כשלון

    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
